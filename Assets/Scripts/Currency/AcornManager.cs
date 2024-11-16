using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AcornManager : MonoBehaviour
{
    [SerializeField] private GameObject acornPrefab;

    private List<Acorn> acorns;

    private void Start()
    {
        acorns = new List<Acorn>();
    }

    public List<Acorn> Spawn(ushort width, ushort height)
    {
        int acornCount = width / 2;

        for (int i = 0; i < acornCount; i++)
        {
            Vector2Int acornPosition;
            do
            {
                acornPosition = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
            } while (acorns.Exists(a => a.position == acornPosition));

            var acornGameObject = Instantiate(acornPrefab, new Vector3(acornPosition.x, 0, acornPosition.y), Quaternion.identity);
            var acorn = new Acorn
            {
                position = acornPosition,
                gameObject = acornGameObject
            };

            acorns.Add(acorn);
        }

        return acorns;
    }

    public void Clear()
    {
        foreach (var acorn in acorns.Where(acorn => acorn.gameObject != null))
        {
            Destroy(acorn.gameObject);
        }

        acorns.Clear();
    }
}

[Serializable]
public class Acorn
{
    public Vector2Int position;
    public GameObject gameObject;
}