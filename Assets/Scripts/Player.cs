using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float arrowSpeed;
    
    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private MeshRenderer leftArrow, rightArrow, upArrow, downArrow;

    [HideInInspector] public Cell[,] currentMaze;
    [HideInInspector] public Vector2Int currentMazeSize;

    private Vector2Int position;
    private Vector2Int previousPosition;
    private bool isMoving;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    public void Reset()
    {
        position = new Vector2Int { x = 0, y = currentMazeSize.y - 1 };
        previousPosition = position;
        MoveToGrid(position);
        GetOpenNeighbours(position, currentMazeSize.x, currentMazeSize.y); // for arrows
    }

    private void MoveToGrid(Vector2Int pos)
    {
        var desiredPosition = new Vector3(pos.x, 0, pos.y);
        transform.DOMove(desiredPosition, moveSpeed).SetEase(Ease.Linear);
        position = pos;
    }

    private void Update()
    {
        if (isMoving) return;

        HandlePC();
        HandleMobile();
    }

    private void HandlePC()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(Move(Vector2Int.down));
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(Move(Vector2Int.up));
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StartCoroutine(Move(Vector2Int.left));
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            StartCoroutine(Move(Vector2Int.right));
        }
    }

    private void HandleMobile()
    {
        if (Input.touchCount <= 0) return;
        
        var touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            startTouchPosition = touch.position;
        }

        if (touch.phase != TouchPhase.Ended) return;
        endTouchPosition = touch.position;
        var swipeDelta = endTouchPosition - startTouchPosition;

        if (!(swipeDelta.magnitude > swipeThreshold)) return;
                
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            StartCoroutine(swipeDelta.x > 0 ? Move(Vector2Int.right) : Move(Vector2Int.left));
        }
        else
        {
            StartCoroutine(swipeDelta.y > 0 ? Move(Vector2Int.up) : Move(Vector2Int.down));
        }
    }

    private IEnumerator Move(Vector2Int direction)
    {
        var initNextPos = new Vector2Int { x = position.x + direction.x, y = position.y + direction.y };

        var openNeighbours = GetOpenNeighbours(position, currentMazeSize.x, currentMazeSize.y);
        if (!openNeighbours.Contains(initNextPos)) yield break;

        isMoving = true;
        previousPosition = position;
        MoveToGrid(initNextPos);

        while (true)
        {
            openNeighbours = GetOpenNeighbours(position, currentMazeSize.x, currentMazeSize.y);
            openNeighbours.Remove(previousPosition);

            if (openNeighbours.Count == 1 && !(position.x == currentMazeSize.x - 1 && position.y == 0))
            {
                previousPosition = position;
                yield return new WaitForSeconds(moveSpeed);
                MoveToGrid(openNeighbours[0]);
            }
            else
            {
                isMoving = false;
                previousPosition = position;
                GetOpenNeighbours(position, currentMazeSize.x, currentMazeSize.y);
                yield break;
            }
        }
    }


    private List<Vector2Int> GetOpenNeighbours(Vector2Int pos, int width, int height)
    {
        var neighbours = new List<Vector2Int>(4);

        leftArrow.transform.DOScale(0, 0.1f);
        rightArrow.transform.DOScale(0, 0.1f);
        upArrow.transform.DOScale(0, 0.1f);
        downArrow.transform.DOScale(0, 0.1f);

        // <
        if (pos.x > 0 && !currentMaze[pos.x - 1, pos.y].HasFlag(Cell.RightWall))
        {
            neighbours.Add(new Vector2Int { x = pos.x - 1, y = pos.y });

            if (!isMoving) leftArrow.transform.DOScale(0.06f, arrowSpeed);
        }

        // V
        if (pos.y > 0 && !currentMaze[pos.x, pos.y].HasFlag(Cell.LowerWall))
        {
            neighbours.Add(new Vector2Int { x = pos.x, y = pos.y - 1 });
            if (!isMoving) downArrow.transform.DOScale(0.06f, arrowSpeed);
        }

        // ^
        if (pos.y < height - 1 && !currentMaze[pos.x, pos.y + 1].HasFlag(Cell.LowerWall))
        {
            neighbours.Add(new Vector2Int { x = pos.x, y = pos.y + 1 });
            if (!isMoving) upArrow.transform.DOScale(0.06f, arrowSpeed);
        }

        // >
        if (pos.x < width - 1 && !currentMaze[pos.x, pos.y].HasFlag(Cell.RightWall))
        {
            neighbours.Add(new Vector2Int { x = pos.x + 1, y = pos.y });
            if (!isMoving) rightArrow.transform.DOScale(0.06f, arrowSpeed);
        }

        return neighbours;
    }
}