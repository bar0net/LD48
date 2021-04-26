using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plant : MonoBehaviour
{
    const int maxHealth = 30;

    [Header("Nutrients")]
    public List<RootShoot> activeRootShoots;
    public GameObject rootShootPrefab;

    public int allowedOffset = 1;
    public int damage = 2;
    public int recovery = 1;

    List<RootShoot> allRootShoot;
    public float depth = 0;

    int[] nutrients = new int[TerrainManager.TerrainCount];
    int[] desiredNutrients = new int[TerrainManager.TerrainCount];
    [SerializeField] int health = maxHealth;

    [Header("UI")]
    public CounterManager waterCounter;
    public CounterManager greenCounter;
    public CounterManager yellowCounter;
    public CounterManager purpleCounter;
    public CounterManager blackCounter;

    public Scrollbar healthBar;

    Manager _m;

    private void Start()
    {
        allRootShoot = new List<RootShoot>();
        foreach (RootShoot s in activeRootShoots) allRootShoot.Add(s);
        for (int i = 0; i < nutrients.Length; ++i) nutrients[i] = 0;
        for (int i = 0; i < desiredNutrients.Length; ++i) desiredNutrients[i] = Random.Range(1,2);
        for (int i = 0; i < desiredNutrients.Length; ++i) UpdateUI((TerrainManager.TerrainType)i);

        _m = FindObjectOfType<Manager>();
        healthBar.size = (float)health / (float)maxHealth;
    }
    
    public void NextTurn()
    {
        foreach (RootShoot s in activeRootShoots)
        {
            depth = -_m.turn;
            s.SetNextObjective();
        }

        for (int i = 0; i < desiredNutrients.Length; ++i)
        {
            if (Mathf.Abs(desiredNutrients[i] - nutrients[i]) > allowedOffset) health -= damage;
            else health += recovery;
        }

        if (health < 0) health = 0;
        if (health > maxHealth) health = maxHealth;

        healthBar.size = (float)health / (float)maxHealth;
        if (health <= 0) _m.GameOver();

        for (int i = 0; i < desiredNutrients.Length; ++i)
        {
            int rng = Random.Range(0, 7);
            if (rng >= 3 && rng < 7) desiredNutrients[i] += 1;
            else if (rng >= 7) desiredNutrients[i] += 2;
        }
        for (int i = 0; i < desiredNutrients.Length; ++i) UpdateUI((TerrainManager.TerrainType)i);
    }

    public void AddNutrient(TerrainManager.TerrainType type)
    {
        nutrients[(int)type] += 1;
        UpdateUI(type);
    }

    public void AddRootShot(RootShoot shoot)
    {
        allRootShoot.Add(shoot);
        activeRootShoots.Add(shoot);
    }

    void UpdateUI(TerrainManager.TerrainType type)
    {
        switch (type)
        {
            case TerrainManager.TerrainType.Water:
                waterCounter.UpdateUI(nutrients[(int)type], desiredNutrients[(int)type], allowedOffset);
                break;
            case TerrainManager.TerrainType.Black:
                blackCounter.UpdateUI(nutrients[(int)type], desiredNutrients[(int)type], allowedOffset);
                break;
            case TerrainManager.TerrainType.Yellow:
                yellowCounter.UpdateUI(nutrients[(int)type], desiredNutrients[(int)type], allowedOffset);
                break;
            case TerrainManager.TerrainType.Green:
                greenCounter.UpdateUI(nutrients[(int)type], desiredNutrients[(int)type], allowedOffset);
                break;
            case TerrainManager.TerrainType.Purple:
                purpleCounter.UpdateUI(nutrients[(int)type], desiredNutrients[(int)type], allowedOffset);
                break;
            default:
                break;
        }
    }
}
