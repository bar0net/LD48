using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TerrainManager.TerrainType resourceType = TerrainManager.TerrainType.None;

    public bool mined = false;

    [HideInInspector] public Vector2Int coords = Vector2Int.zero;

    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Mine()
    {
        mined = true;
        if (sr != null) sr.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        else Debug.Log(gameObject.name + ": No sprite renderer found");
    }
}
