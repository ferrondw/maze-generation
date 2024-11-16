using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private MeshRenderer leftArrow, rightArrow, upArrow, downArrow;
    [SerializeField] private Transform playerVisual;

    public UnityEvent<Vector2Int> onPositionUpdate;
    public UnityEvent onMazeComplete;

    [HideInInspector] public bool canMove = true;

    public readonly Stack<Vector2Int> positionHistory = new();
    private bool isMoving;
    private Vector2Int position;
    private Vector2Int previousPosition;
    private Cell[,] currentMaze;
    private Vector2Int currentMazeSize;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    public void SetMaze(Cell[,] maze)
    {
        currentMaze = maze;
        currentMazeSize = new Vector2Int(maze.GetLength(0), maze.GetLength(1));
        Reset();
    }

    public void Reset()
    {
        position = new Vector2Int { x = 0, y = currentMazeSize.y - 1 };
        previousPosition = position;
        positionHistory.Clear();
        SnapToGrid(position);
        UpdateArrows(position, currentMazeSize.x, currentMazeSize.y);
    }

    public void Teleport(Vector2Int pos, bool snap = false)
    {
        position = pos;
        previousPosition = position;
        
        if (snap) // no clue how to make this shorter whoopsie
        {
            SnapToGrid(position);
        }
        else
        {
            MoveToGrid(position);
        }
        
        UpdateArrows(position, currentMazeSize.x, currentMazeSize.y);
    }
    
    private void Update()
    {
        if (isMoving || !canMove) return;

        HandlePC();
        HandleMobile();
    }

    [MenuItem("RAAHHH/Screenshot")]
    public static void Screenshot()
    {
        string directory = @"Z:\Data\Downloads\RAAHHH";

        // Ensure the directory exists
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Create a filename with the current date and time
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filepath = Path.Combine(directory, $"Screenshot_{timestamp}.png");

        // Save the screenshot
        ScreenCapture.CaptureScreenshot(filepath);

        Debug.Log($"Screenshot saved to: {filepath}");
    }

    private void MoveToGrid(Vector2Int pos)
    {
        var desiredPosition = new Vector3(pos.x, 0, pos.y);
        transform.DOMove(desiredPosition, moveSpeed).SetEase(Ease.Linear);

        if(AudioManager.Instance != null) AudioManager.Instance.PlayClip(0, Random.Range(0, 0.3f));

        positionHistory.Push(pos);
        onPositionUpdate.Invoke(pos);

        Vector3 difference = new Vector3(pos.x - position.x, 0, pos.y - position.y);
        if (difference != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(difference);
            playerVisual.DORotate(targetRotation.eulerAngles, moveSpeed);
        };

        position = pos;
    }

    private void SnapToGrid(Vector2Int pos)
    {
        transform.position = new Vector3(pos.x, 0, pos.y);
        position = pos;
    }

    private void HandlePC()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(Move(Vector2Int.down));
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(Move(Vector2Int.up));
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(Move(Vector2Int.left));
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(Move(Vector2Int.right));
    }

    private void HandleMobile()
    {
        if (Input.touchCount <= 0) return;
        var touch = Input.GetTouch(0);

        if (IsTouchOnLayer(touch, "UI")) return;
        if (touch.phase == TouchPhase.Began) startTouchPosition = touch.position;
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

    private static bool IsTouchOnLayer(Touch touch, string layerName)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };

        var uiRaycasters = FindObjectsOfType<GraphicRaycaster>();

        foreach (GraphicRaycaster raycaster in uiRaycasters)
        {
            if (raycaster.gameObject.layer != LayerMask.NameToLayer(layerName)) continue;
            var results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);
            if (results.Count <= 0) continue;
            
            return true;
        }

        return false;
    }

    private IEnumerator Move(Vector2Int direction)
    {
        var initialNextPosition = new Vector2Int { x = position.x + direction.x, y = position.y + direction.y };

        var openNeighbours = GetOpenNeighbours(position, currentMazeSize.x, currentMazeSize.y);
        if (!openNeighbours.Contains(initialNextPosition)) yield break;

        isMoving = true;
        previousPosition = position;
        UpdateArrows(position, currentMazeSize.x, currentMazeSize.y);
        MoveToGrid(initialNextPosition);

        while (true)
        {
            openNeighbours = GetOpenNeighbours(position, currentMazeSize.x, currentMazeSize.y);
            openNeighbours.Remove(previousPosition);

            if (position == new Vector2Int(currentMazeSize.x - 1, 0)) // end of maze
            {
                onMazeComplete.Invoke();
                isMoving = false;
                yield break;
            }

            if (openNeighbours.Count != 1)
            {
                isMoving = false;
                previousPosition = position;
                UpdateArrows(position, currentMazeSize.x, currentMazeSize.y);
                yield break;
            }

            previousPosition = position;
            yield return new WaitForSeconds(moveSpeed);
            MoveToGrid(openNeighbours[0]);
        }
    }


    private List<Vector2Int> GetOpenNeighbours(Vector2Int pos, int width, int height)
    {
        var neighbours = new List<Vector2Int>(4);
        
        // <
        if (pos.x > 0 && !currentMaze[pos.x - 1, pos.y].HasFlag(Cell.RightWall))
        {
            neighbours.Add(new Vector2Int { x = pos.x - 1, y = pos.y });
        }

        // V
        if (pos.y > 0 && !currentMaze[pos.x, pos.y].HasFlag(Cell.LowerWall))
        {
            neighbours.Add(new Vector2Int { x = pos.x, y = pos.y - 1 });
        }

        // ^
        if (pos.y < height - 1 && !currentMaze[pos.x, pos.y + 1].HasFlag(Cell.LowerWall))
        {
            neighbours.Add(new Vector2Int { x = pos.x, y = pos.y + 1 });
        }

        // >
        if (pos.x < width - 1 && !currentMaze[pos.x, pos.y].HasFlag(Cell.RightWall))
        {
            neighbours.Add(new Vector2Int { x = pos.x + 1, y = pos.y });
        }

        return neighbours;
    }
    
    private void UpdateArrows(Vector2Int pos, int width, int height)
    {
        leftArrow.transform.DOScale(0, arrowSpeed);
        rightArrow.transform.DOScale(0, arrowSpeed);
        upArrow.transform.DOScale(0, arrowSpeed);
        downArrow.transform.DOScale(0, arrowSpeed);
        if (isMoving) return;
        
        if (pos.x > 0 && !currentMaze[pos.x - 1, pos.y].HasFlag(Cell.RightWall)) leftArrow.transform.DOScale(0.4f, arrowSpeed);
        if (pos.y > 0 && !currentMaze[pos.x, pos.y].HasFlag(Cell.LowerWall)) downArrow.transform.DOScale(0.4f, arrowSpeed);
        if (pos.y < height - 1 && !currentMaze[pos.x, pos.y + 1].HasFlag(Cell.LowerWall)) upArrow.transform.DOScale(0.4f, arrowSpeed);
        if (pos.x < width - 1 && !currentMaze[pos.x, pos.y].HasFlag(Cell.RightWall)) rightArrow.transform.DOScale(0.4f, arrowSpeed);
    }
}