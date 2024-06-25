using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelFiveManager : MonoBehaviour
{
    #region Constants
    [Header("LANE INFO !!")]
    [Header("COLLUMN:")]
    [Header("Top")]
    public float collumn_Start;
    public float collumn_End;
    [Space(2)]
    [Header("Bottom")]
    public float collumn_TopY;
    public float collumn_BottomY;

    [Space(5)]

    [Header("ROW:")]
    [Header("Left")]
    public float row_TopY;
    public float row_BottomY;
    [Space(2)]
    [Header("Right")]
    public float row_Start;
    public float row_End;

    private Vector2[] collumn_starts;
    private Vector2[] collumn_ends;
    private Vector2[] row_starts;
    private Vector2[] row_ends;
    #endregion

    #region Wind Settings
    [Space(10)]
    [Header("Wind Settings")]
    [SerializeField] private GameObject wind_Prefab;
    private GameObject[] wind_current;
    [Header("Wind Speeds")]
    public float row_WindSpeed;
    public float collumn_WindSpeed;
    [Header("Wind Timings")]
    public float timeBetweenWindSpawns;
    public float warning_Time;
    [HideInInspector] public bool canSpawnWind = false;
    [Header("Wind - Sound Source")]
    [SerializeField] private GameObject audiosource_wind;
    [Space(10)]
    #endregion

    #region Managers
    private GameManager gameManager;
    private UIManager uiManager;
    private _SceneManager sceneManager;
    #endregion

    #region Player
    private GameObject player;
    #endregion

    #region Warnings
    public GameObject[] warnings;
    #endregion

    #region Dialogue Lines
    private string[] levelStartLines = {

        "Quetzalcoatl!!!",
        "Step no further, The Piercing Winds lie ahead.",
        "To avoid being struck you will need to hide behind something.",
        "Wait until the winds go past to make your move.",
        "See you on the other side, Quetzalcoatl."

    };

    private string[] onRespawnLines = {

        "I told you the wind was strong!",
        "Again, stand behind the pillars in order to avoid the wind!",
        "Warning indicators will show up so you know the incoming direction.",
        "Good luck yet again!"

    };

    private string[] onDeathLines = {

        "Ouch...",
        "That looked like it hurt!",
        "Time to try again!"

    };
    #endregion

    #region UI
    public Image fadeImage;
    private float fadeSpeed = 0.005f;

    private GameObject dialogueCanvas;
    private DialogueScript dialogueSystem;
    #endregion

    private float timer;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();

        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueSystem = dialogueCanvas.transform.GetChild(0).gameObject.GetComponent<DialogueScript>();

        dialogueCanvas.SetActive(false);

        fadeImage.color = new Color(0, 0, 0, 1f);

        timer = timeBetweenWindSpawns;
    }

    private void Start()
    {
        // Initialise collumns and rows.
        InitialiseCollumns();
        InitialiseRows();

        StartCoroutine(co_StartLevel());
    }

    private void Update()
    {
        ShowLines(collumn_starts, collumn_ends);
        ShowLines(row_starts, row_ends);
        
        if (canSpawnWind && !gameManager.isPlayerDead)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(cr_SpawnWind());
                timer = timeBetweenWindSpawns;
                canSpawnWind = false;
            }
        }
    }

    #region Level Start CR
    private IEnumerator co_StartLevel()
    {
        // Instantiate player and add level five player component.
        player = Instantiate(gameManager.playerPrefab);
        player.AddComponent<LevelFivePlayer>();
        // Move player and set name.
        player.transform.position = new Vector2(-25, -1);
        player.transform.name = gameManager.playerPrefab.name;
        // Disable movement input.
        gameManager.acceptInput = false;
        yield return new WaitForSeconds(0.5f);
        gameManager.isPlayerDead = false;
        // Fade into gameplay.
        StartCoroutine(uiManager.GetComponent<UIManager>().FadeOut(fadeImage, 0.5f, 0.005f));
        yield return new WaitForSeconds(1f);
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("right", new Vector2(-19.5f, -1f), 0.9f));
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == new Vector2(-19.5f, -1f));
        // Stop player movement animation.
        player.GetComponent<PlayerScript>().StopMovementAnimation();
        yield return new WaitForSeconds(1.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        if (!gameManager.hasPlayerDiedInLevelFive) {dialogueSystem.StartDialogue(1, levelStartLines); }
        else { dialogueSystem.StartDialogue(1, onRespawnLines); }
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        // Enable movement input.
        gameManager.acceptInput = true;
    }
    #endregion

    #region Player Death
    public void PlayerDeath()
    {
        StartCoroutine(coroutine_PlayerDeath(player));
    }

    // Call on boulder when collision with player.
    public IEnumerator coroutine_PlayerDeath(GameObject player)
    {
        gameManager.isPlayerDead = true;
        gameManager.hasPlayerDiedInLevelFive = true;
        // Destroy player for now but
        // vvv replace this with an animation when we get one...
        Destroy(player);
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.5f, 0.005f));
        yield return new WaitForSeconds(2f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, onDeathLines);
        yield return new WaitUntil(() => dialogueCanvas.activeInHierarchy == false);
        yield return new WaitForSeconds(1f);
        // Restart scene.
        sceneManager.RestartScene();
    }
    #endregion

    #region Initialise
    private void InitialiseCollumns()
    {
        collumn_starts = new Vector2[(int)(collumn_End - collumn_Start) + 1];
        collumn_ends = new Vector2[(int)(collumn_End - collumn_Start) + 1];

        float lastX = collumn_Start;

        for (int i = 0; i < collumn_starts.Length; i++)
        {
            collumn_starts[i] = new Vector2(lastX + i, collumn_TopY);
            collumn_ends[i] = new Vector2(lastX + i, collumn_BottomY);
        }
    }

    private void InitialiseRows()
    {
        row_starts = new Vector2[(int)(row_TopY - row_BottomY) + 1];
        row_ends = new Vector2[(int)(row_TopY - row_BottomY) + 1];

        float lastY = row_TopY;

        for (int i = 0; i < row_starts.Length; i++)
        {
            row_starts[i] = new Vector2(row_Start, lastY - i);
            row_ends[i] = new Vector2(row_End, lastY - i);
        }
    }
    #endregion

    #region Wind Functions
    private void InstantiateWind(string direction)
    {
        var wind_soundsource_go = Instantiate(audiosource_wind);

        switch (direction)
        {
            case ("right"):

                wind_current = new GameObject[row_starts.Length];

                for (int i = 0; i < row_starts.Length; i++)
                {
                    wind_current[i] = Instantiate(wind_Prefab);
                    wind_current[i].transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, -90);
                    wind_current[i].transform.position = row_starts[i];
                    wind_current[i].transform.parent = GameObject.FindWithTag("CurrentWind").transform;
                    wind_current[i].transform.name = wind_Prefab.name;
                    wind_current[i].GetComponent<WindInfo>().destination = row_ends[i];
                    wind_current[i].GetComponent<WindInfo>().moveSpeed = row_WindSpeed;
                    wind_current[i].GetComponent<WindInfo>().direction = direction;
                    wind_current[i].GetComponent<WindInfo>().startMoving = true;
                }

                wind_soundsource_go.transform.localScale = new Vector2(1, 8);
                wind_soundsource_go.transform.position = new Vector2(row_Start, -1);
                wind_soundsource_go.transform.parent = GameObject.FindWithTag("CurrentWind").transform;
                wind_soundsource_go.GetComponent<WindInfo>().destination = new Vector2(row_End, -1);
                wind_soundsource_go.GetComponent<WindInfo>().moveSpeed = row_WindSpeed;
                wind_soundsource_go.GetComponent<WindInfo>().direction = direction;
                wind_soundsource_go.GetComponent<WindInfo>().startMoving = true;

                break;

            case ("left"):

                wind_current = new GameObject[row_starts.Length];

                for (int i = 0; i < row_starts.Length; i++)
                {
                    wind_current[i] = Instantiate(wind_Prefab);
                    wind_current[i].transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 90);
                    wind_current[i].transform.position = row_ends[i];
                    wind_current[i].transform.parent = GameObject.FindWithTag("CurrentWind").transform;
                    wind_current[i].transform.name = wind_Prefab.name;
                    wind_current[i].GetComponent<WindInfo>().destination = row_starts[i];
                    wind_current[i].GetComponent<WindInfo>().moveSpeed = row_WindSpeed;
                    wind_current[i].GetComponent<WindInfo>().direction = direction;
                    wind_current[i].GetComponent<WindInfo>().startMoving = true;
                }

                wind_soundsource_go.transform.localScale = new Vector2(1, 8);
                wind_soundsource_go.transform.position = new Vector2(row_End, -1);
                wind_soundsource_go.transform.parent = GameObject.FindWithTag("CurrentWind").transform;
                wind_soundsource_go.GetComponent<WindInfo>().destination = new Vector2(row_Start, -1);
                wind_soundsource_go.GetComponent<WindInfo>().moveSpeed = row_WindSpeed;
                wind_soundsource_go.GetComponent<WindInfo>().direction = direction;
                wind_soundsource_go.GetComponent<WindInfo>().startMoving = true;

                break;

            case ("down"):

                wind_current = new GameObject[collumn_starts.Length];

                for (int i = 0; i < collumn_starts.Length; i++)
                {
                    wind_current[i] = Instantiate(wind_Prefab);
                    wind_current[i].transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 180);
                    wind_current[i].transform.position = collumn_starts[i];
                    wind_current[i].transform.parent = GameObject.FindWithTag("CurrentWind").transform;
                    wind_current[i].transform.name = wind_Prefab.name;
                    wind_current[i].GetComponent<WindInfo>().destination = collumn_ends[i];
                    wind_current[i].GetComponent<WindInfo>().moveSpeed = collumn_WindSpeed;
                    wind_current[i].GetComponent<WindInfo>().direction = direction;
                    wind_current[i].GetComponent<WindInfo>().startMoving = true;
                }

                wind_soundsource_go.transform.localScale = new Vector2(95, 1);
                wind_soundsource_go.transform.position = new Vector2(25.5f, collumn_TopY);
                wind_soundsource_go.transform.parent = GameObject.FindWithTag("CurrentWind").transform;
                wind_soundsource_go.GetComponent<WindInfo>().destination = new Vector2(25.5f, collumn_BottomY);
                wind_soundsource_go.GetComponent<WindInfo>().moveSpeed = collumn_WindSpeed;
                wind_soundsource_go.GetComponent<WindInfo>().direction = direction;
                wind_soundsource_go.GetComponent<WindInfo>().startMoving = true;

                break;

            case ("up"):

                wind_current = new GameObject[collumn_starts.Length];

                for (int i = 0; i < collumn_starts.Length; i++)
                {
                    wind_current[i] = Instantiate(wind_Prefab);
                    wind_current[i].transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
                    wind_current[i].transform.position = collumn_ends[i];
                    wind_current[i].transform.parent = GameObject.FindWithTag("CurrentWind").transform;
                    wind_current[i].transform.name = wind_Prefab.name;
                    wind_current[i].GetComponent<WindInfo>().destination = collumn_starts[i];
                    wind_current[i].GetComponent<WindInfo>().moveSpeed = collumn_WindSpeed;
                    wind_current[i].GetComponent<WindInfo>().direction = direction;
                    wind_current[i].GetComponent<WindInfo>().startMoving = true;
                }

                wind_soundsource_go.transform.localScale = new Vector2(95, 1);
                wind_soundsource_go.transform.position = new Vector2(25.5f, collumn_BottomY);
                wind_soundsource_go.transform.parent = GameObject.FindWithTag("CurrentWind").transform;
                wind_soundsource_go.GetComponent<WindInfo>().destination = new Vector2(25.5f, collumn_TopY);
                wind_soundsource_go.GetComponent<WindInfo>().moveSpeed = collumn_WindSpeed;
                wind_soundsource_go.GetComponent<WindInfo>().direction = direction;
                wind_soundsource_go.GetComponent<WindInfo>().startMoving = true;

                break;

        }
    }

    private string SpawnRandomWind()
    {
        Debug.Log("Spawning wind!");
        int randNum = Random.Range(1, 5);

        switch(randNum)
        {
            case 1:
                return ("up");
            case 2:
                return ("down");
            case 3:
                return ("left");
            case 4:
                return ("right");
        }

        return null;
    }

    private IEnumerator cr_SpawnWind()
    {
        string direction = SpawnRandomWind();
        int warningIndex = 0;

        if (direction == "left") { warningIndex = 1; }
        else if (direction == "right") { warningIndex = 0; }
        else if (direction == "up") { warningIndex = 3; }
        else if (direction == "down") { warningIndex = 2; }

        warnings[warningIndex].SetActive(true);

        yield return new WaitForSeconds(warning_Time);

        InstantiateWind(direction);

        yield return new WaitUntil(() => !GameObject.FindWithTag("Wind"));

        warnings[warningIndex].SetActive(false);

        canSpawnWind = true;
    }
    #endregion

    public IEnumerator cr_FinishLevel()
    {
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1f);
        yield return new WaitForSeconds(0.5f);
        // Run transition to next level.
        sceneManager.RunLevelTransition(6);
    }

    #region Debug
    private void ShowLines(Vector2[] start, Vector2[] end)
    {
        for (int i = 0;i < start.Length; i++)
        {
            Debug.DrawLine(start[i], end[i], Color.magenta);
        }
    }
    #endregion
}
