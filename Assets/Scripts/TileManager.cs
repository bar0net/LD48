using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    [Header("Hand UI")]
    public Image[] handTileBackground;
    public Image[] handTiles;
    public Color highlightColor;
    public Color neutralColor;
    public GameObject overlayTile;

    int selectedTile = 0;

    [Header("Tileset")]
    public GameObject[] tileSet;
    
    int[] hand;
    SpriteRenderer overlayTile_sr;
    

    // Start is called before the first frame update
    void Start()
    {
        overlayTile_sr = overlayTile.GetComponent<SpriteRenderer>();
        overlayTile.SetActive(false);

        hand = new int[handTiles.Length];
        HandDeal();
        UpdateSelectedTile(true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelectedTile();
    }

    public void UpdateSelectedTile(bool force = false)
    {
        if (!force && Input.mouseScrollDelta.y == 0) return;

        if (Input.mouseScrollDelta.y < 0)
        {
            selectedTile = (selectedTile + 1) % handTiles.Length;
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            selectedTile = (selectedTile - 1) % handTiles.Length;
            if (selectedTile < 0) selectedTile += handTiles.Length;
        }

        overlayTile_sr.sprite = handTiles[selectedTile].sprite;
        overlayTile_sr.color = handTiles[selectedTile].color;

        for (int i = 0; i < handTileBackground.Length; ++i)
        {
            if (i == selectedTile) handTileBackground[i].color = highlightColor;
            else handTileBackground[i].color = neutralColor;
        }
    }

    public void HandDeal()
    {
        for (int i = 0; i < hand.Length; ++i)
        {
            int index = Random.Range(0, tileSet.Length);
            hand[i] = index;

            SpriteRenderer sr = tileSet[index].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                handTiles[i].sprite = sr.sprite;
                handTiles[i].color = sr.color;
            }
        }

        UpdateSelectedTile(true);
    }

    public GameObject NewSelectedTileInstance() 
    {
        GameObject go = (GameObject)Instantiate(tileSet[hand[selectedTile]]);
        return go;
    }
}
