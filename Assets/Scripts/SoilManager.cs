using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilManager : MonoBehaviour
{
    public int terrainWidth = 7;
    public int initialHeight = 4;
    public float rockSpawnChance = 0.05f;

    List<Tile[]> terrain;
    TerrainManager _tm;

    private void Awake()
    {
        terrain = new List<Tile[]>();
    }

    private void Start()
    {
        _tm = FindObjectOfType<TerrainManager>();

        for (int i = 0; i < initialHeight; ++i) GenerateBlankRow(i > 2);
    }

    public void GenerateBlankRow(bool spawnRocks = true)
    {
        GameObject level = new GameObject("level_" + terrain.Count.ToString());
        Tile[] row = new Tile[terrainWidth];

        float left = (float)(terrainWidth - terrain.Count % 2 - 1) / 2.0f;
        int rock_count = 0;
        for (int i = 0; i < terrainWidth; ++i)
        {
            GameObject go;
            if (!spawnRocks || rock_count > (terrainWidth - 1) / 2 || Random.value > rockSpawnChance) go = _tm.GetInstance(TerrainManager.TerrainType.Blank);
            else { go = _tm.GetInstance(TerrainManager.TerrainType.Rock); rock_count++; }

            go.transform.position = new Vector3(i - left, -terrain.Count, 0);
            go.transform.SetParent(level.transform);
            row[i] = go.GetComponent<Tile>();
            row[i].coords = new Vector2Int(terrain.Count, i);
        }

        terrain.Add(row);
    }

    public void Place(Vector2Int coords, TerrainManager.TerrainType type)
    {
        GameObject go = _tm.GetInstance(type);
        go.transform.position  = terrain[coords.x][coords.y].transform.position;
        go.transform.SetParent(terrain[coords.x][coords.y].transform.parent);

        _tm.RemoveInstance(terrain[coords.x][coords.y].gameObject);
        terrain[coords.x][coords.y] = go.GetComponent<Tile>();
        terrain[coords.x][coords.y].coords = coords;
    }

    public int Depth() { return terrain.Count; }

    public void SetTileAlpha(Vector2Int coords, float value)
    {
        Color c = terrain[coords.x][coords.y].GetComponent<SpriteRenderer>().color;
        c.a = value;
        terrain[coords.x][coords.y].GetComponent<SpriteRenderer>().color = c;
    }

    public TerrainManager.TerrainType GetType(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= terrainWidth)  return TerrainManager.TerrainType.None;
        if (coords.y < 0 || coords.y >= terrain.Count) return TerrainManager.TerrainType.None;

        return terrain[coords.x][coords.y].resourceType;
    }
}
