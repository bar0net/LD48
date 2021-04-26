using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [Header("Turn Management")]
    public int turn = 0;
    public int subturn = 0;
    public int turnSegments = 4;

    [Header("Hand Management")]
    public TerrainManager.TerrainType[] spawnableTypes;
    TerrainManager.TerrainType[] hand;
    public UI_HandTile[] handTiles;
    public Color handHighlight = Color.yellow;
    public Color handNeutral = Color.white;
    public GameObject overlayTile;

    [Header("UI")]
    public Text turnText;
    public Scrollbar subturnBar;
    public AudioSource audioSource;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    int currHandSelection = 0;

    TerrainManager _tm;
    SoilManager _soil;
    SpriteRenderer _overlay_sr;
    Plant _plant;

    bool advanceSubTurn = false;
    bool pause = false;
    bool mute = false;

    private void Awake()
    {
        hand = new TerrainManager.TerrainType[handTiles.Length];
    }

    private void Start()
    {
        _tm = FindObjectOfType<TerrainManager>();
        _soil = FindObjectOfType<SoilManager>();
        _overlay_sr = overlayTile.GetComponent<SpriteRenderer>();
        _plant = FindObjectOfType<Plant>();

        turnText.text = "Day " + (turn + 1).ToString();
        subturnBar.size = (float)subturn / turnSegments;

        DealHand();
        for (int i = 0; i < handTiles.Length; ++i) handTiles[i].SetBackgroundColor(i == currHandSelection ? handHighlight : handNeutral);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Pause(!pause);
        if (Input.GetKeyDown(KeyCode.P))      Pause(!pause);

        if (Input.mouseScrollDelta.y < 0) HandUp();
        if (Input.mouseScrollDelta.y > 0) HandDown();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _soil.GenerateBlankRow();
        }
        //if (Input.GetMouseButton(2)) AdvanceTurn();

        if (advanceSubTurn)
        {
            advanceSubTurn = false;
            AdvanceSubturn();
        }
    }

    public void AdvanceTurn()
    {
        turn += 1;
        subturn = 0;

        _plant.NextTurn();

        if (_soil.Depth() - Mathf.Abs(_plant.depth) < _soil.initialHeight) _soil.GenerateBlankRow();

        turnText.text = "Day " + (turn+1).ToString();
        subturnBar.size = (float)subturn / turnSegments;
    }

    public void AdvanceSubturn()
    {
        subturn += 1;
        if (subturn >= turnSegments) AdvanceTurn();
        DealHand();
        subturnBar.size = (float)subturn / turnSegments;
    }

    public void DealHand()
    {

        for (int i = 0; i < handTiles.Length; ++i)
        {
            int rng = Random.Range(0, spawnableTypes.Length);
            hand[i] = spawnableTypes[rng];
            handTiles[i].SetForeground(_tm.TerrainSprite(hand[i]), _tm.TerrainColor(hand[i]));
        }

        UpdateOverlayTile();
    }

    public void HandUp()
    {
        int N = handTiles.Length;
        currHandSelection += 1;
        if (currHandSelection >= N) currHandSelection -= N;
        for (int i = 0; i < N; ++i) handTiles[i].SetBackgroundColor(i == currHandSelection ? handHighlight : handNeutral);

        UpdateOverlayTile();
    }

    public void HandDown()
    {
        int N = handTiles.Length;
        currHandSelection -= 1;
        if (currHandSelection < 0) currHandSelection += N;
        for (int i = 0; i < N; ++i) handTiles[i].SetBackgroundColor(i == currHandSelection ? handHighlight : handNeutral);

        UpdateOverlayTile();

    }

    void UpdateOverlayTile()
    {
        _overlay_sr.sprite = _tm.TerrainSprite(hand[currHandSelection]);
        _overlay_sr.color = _tm.TerrainColor(hand[currHandSelection]);

        // Show magnet area of effect
        _overlay_sr.transform.GetChild(0).gameObject.SetActive(hand[currHandSelection] == TerrainManager.TerrainType.Magnet);
    }

    public void PlaceTile(Vector2Int coords)
    {
        _soil.Place(coords, hand[currHandSelection]);
        //AdvanceSubturn();
        advanceSubTurn = true;
    }

    public void PlaceTile(Vector2Int coords, TerrainManager.TerrainType type)
    {
        _soil.Place(coords, type);
    }

    public TerrainManager.TerrainType RandomResource()
    {
        // Place magnet in position 1 so it's not selectable
        return spawnableTypes[Random.Range(1, spawnableTypes.Length)];
    }

    public Color ResourceColor(TerrainManager.TerrainType type)
    {
        switch (type)
        {
            case TerrainManager.TerrainType.Water:
                return new Color(0.46f, 0.55f, 0.84f);
            case TerrainManager.TerrainType.Black:
                return new Color(0.1f, 0.1f, 0.1f);
            case TerrainManager.TerrainType.Yellow:
                return new Color(0.86f, 0.78f, 0.24f);
            case TerrainManager.TerrainType.Purple:
                return new Color(0.56f, 0.38f, 0.62f);
            case TerrainManager.TerrainType.Green:
                return new Color(0.69f, 0.78f, 0.66f);

            default:
                return Color.red;
        }
    }

    public void GameOver()
    {
        pause = true;
        foreach (RootShoot r in _plant.activeRootShoots) r.enabled = false;
        gameOverPanel.SetActive(true);
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void Pause(bool value)
    {
        pausePanel.SetActive(value);
        this.pause = value;
    }

    public void MuteAudio(bool value)
    {
        mute = value;

        if (value) audioSource.Pause();
        else audioSource.UnPause();

    }

    public bool Paused() { return pause; }
}


