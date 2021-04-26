using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootPoint : MonoBehaviour
{
    public TerrainManager.TerrainType desiredType = TerrainManager.TerrainType.None;
    public int amount;
    public int desiredAmount = 4;
    public Vector2Int desiredRange = new Vector2Int(4, 8);

    public RootShoot shoot;
    public Transform mask;

    private void Start()
    {
        int turns = FindObjectOfType<Manager>().turn;
        desiredType = FindObjectOfType<Manager>().RandomResource();
        desiredAmount = Random.Range(desiredRange.x + turns / 5, desiredRange.y + turns /10);
        amount = Mathf.FloorToInt(desiredAmount/2.0f);

        if (mask != null)
        {
            mask.localScale = new Vector3((float)amount / desiredAmount, (float)amount / desiredAmount, 1.0f);
            SpriteRenderer sr = mask.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = FindObjectOfType<Manager>().ResourceColor(desiredType);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tile t = collision.gameObject.GetComponent<Tile>();
        if (t == null) t = collision.transform.GetComponentInParent<Tile>();

        if (t != null)
        {
            if (!t.mined)
            {
                if (t.resourceType == desiredType && amount < desiredAmount) AddResource(1);
                shoot.AddContact(t);
                t.Mine();
            }

            if (t.resourceType == TerrainManager.TerrainType.Magnet) shoot.AddMagnet(t);
            else if (t.resourceType == TerrainManager.TerrainType.Rock) Death();
            else if (t.resourceType == TerrainManager.TerrainType.Blank)
            {
                FindObjectOfType<Manager>().PlaceTile(t.coords, TerrainManager.TerrainType.Soil);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tile t = collision.gameObject.GetComponent<Tile>();
        if (t == null) t = collision.transform.GetComponentInParent<Tile>();

        if (t != null && t.resourceType == TerrainManager.TerrainType.Magnet) shoot.RemoveMagnet(t);
    }

    public void AddResource(int value)
    {
        amount += value;
        value = amount - desiredAmount;

        float ratio = Mathf.Clamp((float)(amount) / desiredAmount, 0, 1.0f);
        if (mask != null) mask.transform.localScale = new Vector3(ratio, ratio, 1.0f);

        if (amount >= desiredAmount) Split();

    }

    void Split()
    {
        mask.gameObject.SetActive(false);
        shoot.Reproduce();
        this.enabled = false;
    }

    void Death()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.4f, 0.4f, 1.0f);
        shoot.Death();
        this.enabled = false;
    }
}
