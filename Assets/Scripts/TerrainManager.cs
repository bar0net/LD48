using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour
{
    public enum TerrainType { None = 0, Blank = 1, Water = 2, Black = 3, Yellow = 4, Green = 5, Purple = 6, Magnet = 7, Soil = 8, Rock = 9 }
    public const int TerrainCount = 10;

    public TilePool blankPool;

    [Header("Tiles")]
    public GameObject waterTile = null;
    public GameObject blackTile = null;
    public GameObject yellowTile = null;
    public GameObject greenTile = null;
    public GameObject purpleTile = null;
    public GameObject magnetTile = null;
    public GameObject soilTile   = null;
    public GameObject rockTile   = null;
    
    GameObject[] map    = new GameObject[TerrainCount];
    SpriteRenderer[] sr = new SpriteRenderer[TerrainCount];

    private void Awake()
    {
        for (int i = 0; i < TerrainCount; ++i) map[i] = null;
        for (int i = 0; i < TerrainCount; ++i) sr[i] = null;

        map[(int)TerrainType.Water]  = waterTile;
        map[(int)TerrainType.Black]  = blackTile;
        map[(int)TerrainType.Yellow] = yellowTile;
        map[(int)TerrainType.Green]  = greenTile;
        map[(int)TerrainType.Purple] = purpleTile;
        map[(int)TerrainType.Magnet] = magnetTile;
        map[(int)TerrainType.Soil]   = soilTile;
        map[(int)TerrainType.Rock]   = rockTile;

        if (waterTile)  sr[(int)TerrainType.Water]  = waterTile.GetComponent<SpriteRenderer>();
        if (blackTile)  sr[(int)TerrainType.Black]  = blackTile.GetComponent<SpriteRenderer>();
        if (yellowTile) sr[(int)TerrainType.Yellow] = yellowTile.GetComponent<SpriteRenderer>();
        if (greenTile)  sr[(int)TerrainType.Green]  = greenTile.GetComponent<SpriteRenderer>();
        if (purpleTile) sr[(int)TerrainType.Purple] = purpleTile.GetComponent<SpriteRenderer>();
        if (magnetTile) sr[(int)TerrainType.Magnet] = magnetTile.GetComponent<SpriteRenderer>();
        if (soilTile)   sr[(int)TerrainType.Soil]   = soilTile.GetComponent<SpriteRenderer>();
        if (rockTile)   sr[(int)TerrainType.Rock]   = rockTile.GetComponent<SpriteRenderer>();
    }

    public GameObject GetInstance(TerrainType type)
    {
        if (type == TerrainType.Blank) return blankPool.Get();

        if (map[(int)type] == null) return null;

        return (GameObject)Instantiate(map[(int)type]);
    }

    public void RemoveInstance(GameObject tile)
    {
        if (tile == null) return;

        Tile t = tile.GetComponent<Tile>();
        if (t == null) Destroy(tile);

        if (t.resourceType == TerrainType.Blank) blankPool.Put(tile);
        else Destroy(tile);
    }

    public Sprite TerrainSprite(TerrainType type)
    {
        return sr[(int)type].sprite;
    }

    public Color TerrainColor(TerrainType type)
    {
        return sr[(int)type].color;
    }
}
