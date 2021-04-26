using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePool : MonoBehaviour
{
    public GameObject blankTile;

    Stack<GameObject> collection;
    TerrainManager.TerrainType type;

    private void Awake()
    {
        collection = new Stack<GameObject>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        type = blankTile.GetComponent<Tile>().resourceType;
    }

    public GameObject Get()
    {
        GameObject go;
        if (collection.Count > 0) go = collection.Pop();
        else go = (GameObject)Instantiate(blankTile);

        go.SetActive(true);
        return go;
    }

    public void Put(GameObject tile)
    {
        tile.SetActive(false);
        Tile t = tile.GetComponent<Tile>();

        if (t == null || t.resourceType != type)
        {
            Debug.Log("Trying to return an incompatible tile type to the pool.");
            Destroy(tile);
        }
        else
        {
            collection.Push(tile);
            tile.transform.SetParent(this.transform);
        }
    }
}
