using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelNineManager : MonoBehaviour
{
    private GameManager gameManager;
    private UIManager uiManager;
    private _SceneManager sceneManager;

    #region Player Vars
    private GameObject player_go;
    private PlayerScript player_script;
    #endregion

    [Header("THE Conchy Wonchy")]
    [SerializeField] private GameObject conchyWonchy;
    [Space(3)]
    #region Conch Hero
    [Header("Conch Hero - (Fake) Player")]
    public GameObject ch_player;
    [Space(3)]
    [Header("Lanes")]
    [SerializeField] private Transform[] lane_gos;
    [Space(3)]
    [Header("Conch Hero - UI")]
    [SerializeField] private Canvas ch_uiCanvas;
    [Space(3)]
    [Header("Cameras")]
    public GameObject main_cameraHolder;
    public GameObject ch_cameraHolder;
    private Camera main_camera;
    private Camera ch_camera;
    [Space(3)]
    #region Bee Info
    [Header("Bee")]
    [SerializeField] private GameObject bee_prefab;
    [SerializeField] private float bee_delay = 0.7f;
    [SerializeField] private int bee_startingAmount = 2;
    [SerializeField] private int bee_endingAmount = 6;
    private int bee_currentAmount;
    [Space(3)]
    [Header("Bee - Lanes")]
    public int[] bee_lanes;
    [HideInInspector] public bool hasCompletedTurn = false;
    #endregion
    #endregion
    [Header("General - UI")]
    [SerializeField] private Image ui_fadeImage;
    [SerializeField] private TextMeshProUGUI ui_keybindText;
    private float fade_speed = 0.005f;

    #region Dialogue Settings
    private GameObject dialogueCanvas;
    private DialogueScript dialogueSystem;
    #region Lines
    private string[] lines_start = {

        "What a trip it has been.",
        "You have made it to Mictlan!",
        "Cross the bridge and speak with Mictlantecuhtli."

    };
    private string[] lines_mictlanFirstInteraction = {

        "So you must be the so called Quetzalcoatl I've heard about.",
        "Now let me ask you this one question.",
        "_<color=#800000>Why are you here?-</color>",
        "Ah, you want my precious _<color=#006424>Jade Bones-</color>.",
        "I will give them to you on one condition…",
        "See the Conch in front of you?",
        "Play me a tune and I will hand the _<color=#006424>Jade Bones-</color> over to you."

    };
    private string[] lines_xolotlAfterFirstInteraction = {

        "Quetzalcoatl!",
        "He is testing you.",
        "You will be unable to play a tune because it is just a Conch.",
        "Quick find something you can place inside of Conch that could make a noise!"

    };
    private string[] lines_theBees = { "Buzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz..." };
    private string[] lines_beforeMinigame = {

        "Great work those bees should do the trick.",
        "Now go rock Mictlan!!!"

    };
    private string[] lines_afterMinigame = {

        // Mictlantecuhtli
        "Wow, I am impressed...",
        "After all these years someone actually played a tune.",
        "A funky tune...",
        "I am a man of my word, take however many you need.",
        // Xolotl
        "Wow, you are the first to have ever descended to Mictlan and impressed Mictlantecuhtli.",
        "Head out the exit to the right to leave."

    };
    private string[] lines_randomAfterTurn = {

        "That was... wow.",
        "Beautiful musical prowess.",
        "Oh wow, I could listen to this all day...",
        "What a song."

    };
    private string[] lines_xolotlFinalLines = {

        "Thank you for letting me join you in this journey...",
        "May the world forever be in your debt.",
        "Now return to the surface...",
        "And again...",
        "Thank you"

    };
    #endregion
    #endregion

    #region Audio
    [Space(5)]
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audio_source;

    [Space(3)]
    [Header("Sounds")]
    public AudioClip[] sound_conch;
    #endregion

    [Space(3)]
    [Header("Collider")]
    [SerializeField] private GameObject collider_finishBlocker;

    [Space(3)]
    [Header("Triggers")]
    public GameObject cutsceneTrigger;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();

        // Get reference to dialogue stuffsies
        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueSystem = dialogueCanvas.transform.GetChild(0).GetComponent<DialogueScript>();
        dialogueCanvas.SetActive(false);

        // get camera components
        ch_camera = ch_cameraHolder.transform.GetChild(0).gameObject.GetComponent<Camera>();
        main_camera = main_cameraHolder.transform.GetChild(0).gameObject.GetComponent<Camera>();

        // ui_fadeImage.color = new Color(0, 0, 0, 1); // set fade image's alpha to 1

        gameManager.acceptInput = false;

        // Spawning and dealing with the player \\
        SpawnPlayer();
        player_go.transform.position = new Vector2(-37f, 0); // position player at start of level
        player_go.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1; // change sorting layer so that player is behind the top layer

        // start level
        StartCoroutine(StartLevel());
    }

    // -10.3 min
    // 8 max

    void Update()
    {
        DrawLaneLines();
        if (Input.GetKeyDown(KeyCode.P)) { StartCoroutine(cr_FinishLevel()); }
    }

    private IEnumerator StartLevel()
    {
        // Disable conch hero canvas
        ch_uiCanvas.enabled = false;
        ch_cameraHolder.SetActive(false);
        // Start player walk.
        StartCoroutine(player_script.Walk("right", new Vector2(-32, 0), 1.6f));
        // Fade into gameplay.
        StartCoroutine(uiManager.FadeOut(ui_fadeImage, 0.5f, 0.005f)); // start fade into gameplay
        yield return new WaitUntil(() => ui_fadeImage.color.a <= 0); // wait until fade has completed
        yield return new WaitUntil(() => player_go.transform.position.x == -32);
        yield return new WaitForSeconds(1.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, lines_start);
        yield return new WaitUntil(() => dialogueSystem.dialogueComplete);
        // Add camera follow component.
        main_cameraHolder.AddComponent<CameraFollowAllAxis>();
        // Change player sprite renderer sorting order.
        player_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3; // reset sorting order
        yield return new WaitForSeconds(0.5f);
        // Enable movement input.
        gameManager.acceptInput = true;
        yield return null;
    }

    // Runs "cutscene" where Quetz talks to Mictlan guy and picks up Conch
    public IEnumerator cr_FirstTalkWithMictlan()
    {
        // Disable movement input.
        gameManager.acceptInput = false;
        // Stop movement animation.
        player_script.StopMovementAnimation();
        // Start player walk.
        StartCoroutine(player_script.Walk("right", new Vector2(-20, 0.5f), 1.5f));
        yield return new WaitUntil(() => new Vector2(player_go.transform.position.x, player_go.transform.position.y) == new Vector2(-20, 0.5f));
        // Set player to face right.
        player_script.SetPlayerAnimationDirection("right");
        // Stop camera following on all axis.
        Destroy(main_cameraHolder.GetComponent<CameraFollowAllAxis>());
        yield return new WaitForSeconds(1);
        // Move camera to reveal Mictlan guy
        StartCoroutine(MoveCamera(new Vector2(-9, 0.5f), 2.1f));
        yield return new WaitUntil(() => main_cameraHolder.transform.position.x == -9);
        // Start player walk.
        StartCoroutine(player_script.Walk("right", new Vector2(-10, 0.5f), 1.8f));
        yield return new WaitForSeconds(1.5f);
        // Start first dialogue
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(2, new string[] {
            lines_mictlanFirstInteraction[0],
            lines_mictlanFirstInteraction[1],
            lines_mictlanFirstInteraction[2],
            lines_mictlanFirstInteraction[3],
        });
        yield return new WaitUntil(() => dialogueSystem.dialogueComplete);
        yield return new WaitForSeconds(0.5f);
        // Pan to jade bones
        StartCoroutine(MoveCamera(new Vector2(-2, 0.5f), 1.5f));
        yield return new WaitUntil(() => main_cameraHolder.transform.position.x >= -2);
        yield return new WaitForSeconds(2f);
        // Pan back to player and Mictlan guy
        StartCoroutine(MoveCamera(new Vector2(-9, 0.5f), 1.5f));
        yield return new WaitUntil(() => main_cameraHolder.transform.position.x <= -9f);
        yield return new WaitForSeconds(0.5f);
        // Start second dialogue
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(2, new string[] {
            lines_mictlanFirstInteraction[4],
            lines_mictlanFirstInteraction[5],
            lines_mictlanFirstInteraction[6]
        });
        yield return new WaitUntil(() => dialogueSystem.dialogueComplete);
        yield return new WaitForSeconds(0.5f);
        // Start player walk to conch.
        StartCoroutine(player_script.Walk("right", new Vector2(-5, 0.5f), 1.8f));
        yield return new WaitUntil(() => player_go.transform.position.x == -5);
        yield return new WaitForSeconds(1f);
        // Destroy conch gameobject.
        Destroy(conchyWonchy);
        yield return new WaitForSeconds(0.5f);
        // Make player face camera.
        player_script.SetPlayerAnimationDirection("down");
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, new string[] { lines_xolotlAfterFirstInteraction[0] });
        yield return new WaitUntil(() => dialogueSystem.dialogueComplete);
        // Move camera.
        StartCoroutine(MoveCamera(new Vector2(-5, 0.5f), 2.1f));
        yield return new WaitUntil(() => main_cameraHolder.transform.position.x == -5);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, new string[] {
            lines_xolotlAfterFirstInteraction[1],
            lines_xolotlAfterFirstInteraction[2],
            lines_xolotlAfterFirstInteraction[3]
        });
        yield return new WaitUntil(() => dialogueSystem.dialogueComplete);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(4, lines_theBees);
        yield return new WaitUntil(() => dialogueSystem.dialogueComplete);
        // Make it so player can collect bees.
        player_go.GetComponent<LevelNinePlayer>().canCollectBees = true;
        // Enable movement input.
        gameManager.acceptInput = true;
        // Enable camera following on all axis.
        main_cameraHolder.AddComponent<CameraFollowAllAxis>();
    }

    public IEnumerator cr_FinishLevel()
    {
        // Disable movement input.
        gameManager.acceptInput = false;
        // Stop player movement animation.
        player_script.StopMovementAnimation();
        yield return new WaitForSeconds(0.5f);
        // Start player walk.
        StartCoroutine(player_script.Walk("right", new Vector2(8, 0.5f), 1.9f));
        yield return new WaitUntil(() => player_go.transform.position.x == 8);
        // Set player to face camera.
        player_script.SetPlayerAnimationDirection("down");
        yield return new WaitForSeconds(1);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, lines_xolotlFinalLines);
        yield return new WaitUntil(() => dialogueSystem.dialogueComplete);
        yield return new WaitForSeconds(0.5f);
        // Set player to face right.
        player_script.SetPlayerAnimationDirection("right");
        yield return new WaitForSeconds(1.5f);
        // Start player walk.
        StartCoroutine(player_script.Walk("right", new Vector2(15, 0.5f), 1.5f));
        yield return new WaitUntil(() => player_go.transform.position.x >= 10.5);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(ui_fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => ui_fadeImage.color.a >= 1);
        yield return new WaitForSeconds(1);
        // Go to credits. (Has beat level)
        sceneManager.GoToCredits(true);
    }

    private IEnumerator MoveCamera(Vector2 destination, float speed)
    {
        while (new Vector2(main_cameraHolder.transform.position.x, main_cameraHolder.transform.position.y) != destination)
        {
            float step = speed * Time.deltaTime;
            main_cameraHolder.transform.position = Vector2.MoveTowards(main_cameraHolder.transform.position, destination, step);
            yield return new WaitForEndOfFrame();
        }
    }

    // Use this to spawn the player
    private void SpawnPlayer()
    {
        player_go = Instantiate(gameManager.playerPrefab);
        player_script = player_go.GetComponent<PlayerScript>();
        player_go.name = gameManager.playerPrefab.name;

        player_go.AddComponent<LevelNinePlayer>();
        player_go.GetComponent<LevelNinePlayer>().manager_levelNine = this;

        player_go.AddComponent<AudioListener>();
    }

    #region Conch Hero Functions
    private void SpawnBee(int lane)
    {
        GameObject go = Instantiate(bee_prefab);
        go.name = bee_prefab.name;

        go.transform.position = lane_gos[lane - 1].GetChild(0).transform.position;
        go.GetComponent<BeeScript>().destination = lane_gos[lane - 1].GetChild(1).transform.position;
        go.GetComponent<BeeScript>().lane = lane;
    }

    public void func_StartMinigame() { StartCoroutine(Minigame()); }

    private IEnumerator Minigame()
    {
        // Dialogue before minigame
        // Disable movement input.
        gameManager.acceptInput = false;
        // Stop player movement animation.
        player_script.StopMovementAnimation();
        yield return new WaitForSeconds(0.5f);
        // Set player to face right.
        player_script.SetPlayerAnimationDirection("down");
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, lines_beforeMinigame);
        yield return new WaitUntil(() => dialogueSystem.dialogueComplete);
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(ui_fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => ui_fadeImage.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        // Switch camera
        ch_cameraHolder.SetActive(true);
        main_cameraHolder.SetActive(false);
        // Activate CH UI
        ch_uiCanvas.enabled = true;
        yield return new WaitForSeconds(0.5f);
        // Fade into gameplay.
        StartCoroutine(uiManager.FadeOut(ui_fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => ui_fadeImage.color.a <= 0);
        yield return new WaitForSeconds(1);
        bee_currentAmount = bee_startingAmount;

        // Minigame, game loop.
        while (bee_currentAmount <= bee_endingAmount)
        {
            hasCompletedTurn = false;
            // Initialise note array and spawn bees.
            InitialiseNoteArray();
            StartCoroutine(SpawnBeesAndStartPlayerTurn());
            yield return new WaitUntil(() => hasCompletedTurn);

            // After turn //
            yield return new WaitForSeconds(1f);
            int randNum = Random.Range(0, 4);
            // Start dialogue.
            dialogueCanvas.SetActive(true);
            dialogueSystem.StartDialogue(2, new string[] { lines_randomAfterTurn[randNum] });
            yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
            ch_player.GetComponent<PlayAnimationScript>().shouldAnimationBeRunning = false; // stop boogy animation :<
            ch_player.GetComponent<PlayAnimationScript>().ResetSprite(); // reset fake player sprite
            bee_currentAmount++;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(2, new string[] {
            lines_afterMinigame[0],
        });
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);

        StartCoroutine(uiManager.FadeIn(ui_fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => ui_fadeImage.color.a >= 1);

        yield return new WaitForSeconds(0.5f);
        // Switch camera
        main_cameraHolder.SetActive(true);
        ch_cameraHolder.SetActive(false);
        // Deactivate CH UI
        ch_uiCanvas.enabled = false;
        // Teleport player
        player_go.transform.position = new Vector2(-10, 0.5f);
        player_script.SetPlayerAnimationDirection("right");
        // Move camera
        Destroy(main_cameraHolder.GetComponent<CameraFollowAllAxis>());
        main_cameraHolder.transform.position = new Vector2(-3.7f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(uiManager.FadeOut(ui_fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => ui_fadeImage.color.a <= 0);
        yield return new WaitForSeconds(1);
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(2, new string[] {
            lines_afterMinigame[1],
            lines_afterMinigame[2],
            lines_afterMinigame[3],
        });
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        yield return new WaitForSeconds(0.5f);
        player_script.SetPlayerAnimationDirection("down");
        yield return new WaitForSeconds(1);
        StartCoroutine(MoveCamera(new Vector2(-10, 0.5f), 1.5f));
        yield return new WaitUntil(() => main_cameraHolder.transform.position.x == -10);
        yield return new WaitForSeconds(0.5f);
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, new string[] {
            lines_afterMinigame[4],
            lines_afterMinigame[5],
        });
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);

        // Move blocker to now block player from leaving back over the bridge
        collider_finishBlocker.transform.position = new Vector2(-20.5f, 0);
        // Enable movement
        gameManager.acceptInput = true;
        // Set camera follow script
        main_cameraHolder.AddComponent<CameraFollowXAxis>();
        main_cameraHolder.GetComponent<CameraFollowXAxis>().xStoppingPoint1 = -10.3f;
        main_cameraHolder.GetComponent<CameraFollowXAxis>().xStoppingPoint2 = 8f;
        yield return new WaitForSeconds(0.5f);
    }

    // Player turn during minigame.
    private IEnumerator SpawnBeesAndStartPlayerTurn()
    {
        string[] text_Watch = { "Watch" };
        string[] text_YourTurn = { "Your Turn!" };
        // Start Xolotl dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, text_Watch);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        // Spawn bees.
        for (int i = 0; i < bee_lanes.Length; i++)
        {
            SpawnBee(bee_lanes[i]);
            yield return new WaitForSeconds(bee_delay);
        }
        // Wait until all bees have disappeared.
        yield return new WaitUntil(() => !GameObject.Find("Bee"));
        // Start Xolotl dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, text_YourTurn);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        ch_player.GetComponent<PlayAnimationScript>().shouldAnimationBeRunning = true;
        // Player note input.
        StartCoroutine(NoteInput());
        yield return null;
    }

    private IEnumerator NoteInput()
    {
        bool enterFailState = false;

        int index = 0;

        while (index < bee_lanes.Length && !enterFailState)
        {
            if (Input.GetKeyDown(gameManager.key_FirstLane))
            {
                Debug.Log("1 Key Pressed, 1");
                if (bee_lanes[index] == 1)
                {
                    index++;
                    PlaySound(1, sound_conch[0]);
                    Debug.Log("Correct Note");
                }
                else
                {
                    enterFailState = true;
                    PlaySound(0.5f, sound_conch[0]);
                    Debug.Log("Wrong Note");
                    Debug.Log("Entering failstate");
                    StartCoroutine(WrongNote());
                }
            }
            if (Input.GetKeyDown(gameManager.key_SecondLane))
            {
                Debug.Log("2 Key Pressed, 2");
                if (bee_lanes[index] == 2)
                {
                    index++;
                    PlaySound(1, sound_conch[1]);
                    Debug.Log("Correct Note");
                }
                else
                {
                    enterFailState = true;
                    PlaySound(0.5f, sound_conch[1]);
                    Debug.Log("Wrong Note");
                    Debug.Log("Entering failstate");
                    StartCoroutine(WrongNote());
                }
            }
            if (Input.GetKeyDown(gameManager.key_ThirdLane))
            {
                Debug.Log("3 Key Pressed, 3");
                if (bee_lanes[index] == 3)
                {
                    index++;
                    PlaySound(1, sound_conch[2]);
                    Debug.Log("Correct Note");
                }
                else
                {
                    enterFailState = true;
                    PlaySound(0.5f, sound_conch[2]);
                    Debug.Log("Wrong Note");
                    Debug.Log("Entering failstate");
                    StartCoroutine(WrongNote());
                }
            }
            if (Input.GetKeyDown(gameManager.key_FourthLane))
            {
                Debug.Log("4 Key Pressed, 4");
                if (bee_lanes[index] == 4)
                {
                    index++;
                    PlaySound(1, sound_conch[3]);
                    Debug.Log("Correct Note");
                }
                else
                {
                    enterFailState = true;
                    PlaySound(0.5f, sound_conch[3]);
                    Debug.Log("Wrong Note");
                    Debug.Log("Entering failstate");
                    StartCoroutine(WrongNote());
                }
            }
            yield return null;
        }
        if (!enterFailState) { hasCompletedTurn = true; }
    }
    private IEnumerator WrongNote()
    {
        string[] text_wrongNote = {

            "That's not the right note...",
            "Watch again and see if you can get it this time."

        };
        yield return new WaitForSeconds(1.4f);

        ch_player.GetComponent<PlayAnimationScript>().shouldAnimationBeRunning = false; // stop boogy animation :(

        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, text_wrongNote);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);

        StartCoroutine(SpawnBeesAndStartPlayerTurn());
    }

    private void InitialiseNoteArray()
    {
        bee_lanes = new int[bee_currentAmount];

        for (int i = 0; i < bee_currentAmount; i++)
        {
            bee_lanes[i] = Random.Range(1, 5);
        }
    }
    #endregion

    #region UI Functions
    public void ShowInteractKeybindText(string textBefore, string textAfter) { uiManager.ShowKeybindText(ui_keybindText, textBefore, textAfter, gameManager.key_Interact); }
    public void HideInteractKeybindText() { uiManager.HideKeybindText(ui_keybindText); }
    #endregion

    public void PlaySound(float pitch, AudioClip clip)
    {
        audio_source.pitch = pitch;
        audio_source.PlayOneShot(clip);
    }

    #region Debug
    private void DrawLaneLines()
    {
        for (int i = 0; i < lane_gos.Length; i++)
        {
            Debug.DrawLine(lane_gos[i].GetChild(0).transform.position, lane_gos[i].GetChild(1).transform.position, Color.magenta);
        }
    }
    #endregion
}
