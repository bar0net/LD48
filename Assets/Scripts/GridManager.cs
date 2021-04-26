using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Params")]
    public int gridWidth = 7;
    public int initialHeight = 3;
    public float yOffset = 0;

    [Header("Sprites")]
    public float cellSize = 1.0f;
    public TilePool blankCellPool;

    List<GameObject[]> terrain;
    TileManager _tm;

    // Start is called before the first frame update
    void Start()
    {
        _tm = FindObjectOfType<TileManager>();
        terrain = new List<GameObject[]>();

        for (int i = 0; i < initialHeight; ++i) GenerateTerrainRow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateTerrainRow()
    {
        GameObject[] row = new GameObject[gridWidth];

        float left = cellSize * (float)(gridWidth - terrain.Count % 2 - 1) / 2.0f;
        for (int i = 0; i < gridWidth; ++i)
        {
            float x_pos = i * cellSize - left;
            float y_pos = terrain.Count + yOffset;
            Vector2 pos = x_pos * Vector2.right - y_pos * Vector2.up;

            row[i] = blankCellPool.Get(); //(GameObject)Instantiate(blankCell, pos, Quaternion.identity, this.transform);
            row[i].transform.position = pos;
            row[i].transform.SetParent(this.transform);

            Tile t = row[i].GetComponent<Tile>();
            if (t != null) t.coords = new Vector2Int(terrain.Count, i);
        }

        terrain.Add(row);
    }

    public void PlaceSelectedTile(Vector2Int coords)
    {
        Vector3 position = terrain[coords.x][coords.y].transform.position;
        blankCellPool.Put(terrain[coords.x][coords.y]);

        terrain[coords.x][coords.y] = _tm.NewSelectedTileInstance();
        terrain[coords.x][coords.y].transform.position = position;
        terrain[coords.x][coords.y].transform.SetParent(this.transform);

        _tm.HandDeal();
    }
}
