using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelTwoManager : MonoBehaviour
{
    #region Variables
    // Script for this opening text still required (placeholder of general things needing to be talked about).
    private string[] startingLines = {

        "Hey again!",
        "Welcome to the cliff's edge.",
        "To progress through this level you will need to…",
        "make your way to the end of this cliff and go through the hole to exit.",
        "But wait, there are unstable boulders rolling down the cliff at an alarming rate!",
        "I urge you to be safe, Quetzalcoatl.",
        "Speak with you again soon.",

    };

    private string[] linesOnRespawn = {

        "Glad you are back.",
        "Now back to the task at hand...",
        "Avoid the boulders and get to the end!",
        "Good luck this time!"

    };

    private string[] onDeathLines = {

        "Ouch...",
        "That looked like it hurt!",
        "Time to try again!"

    };

    [Header("Player Settings")]
    private GameObject player;
    [SerializeField] private GameObject player_PositionCollider;

    private GameManager gameManager;
    private _SceneManager sceneManager;
    private UIManager uiManager;

    [Space(5)]
    [Header("Dialogue Settings")]
    [SerializeField] private GameObject dialogueCanvas;
    private DialogueScript dialogueSystem;

    [Header("Boulder Info")]
    [SerializeField] private GameObject boulderPrefab;
    [SerializeField] private Transform[] boulderSpawnLoc;
    [SerializeField] private Transform[] boulderDeathLoc;
    [SerializeField] private float timeBetweenBoulderSpawns;

    // Timer var
    private float timer;

    // Black image used for fading in and out.
    [SerializeField] private Image fadeImage;
    private float fadeAmount = 0.5f;
    private float fadeSpeed = 0.005f;

    [Header("Warning Icons")]
    [SerializeField] private TextMeshProUGUI warningIcon1, warningIcon2, warningIcon3;
    #endregion

    private void Awake()
    {
        // Get reference to managers
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();

        dialogueSystem = dialogueCanvas.transform.GetChild(0).gameObject.GetComponent<DialogueScript>();
        // player = Instantiate(playerPrefab);
        
        // Spawn player.
        player = Instantiate(gameManager.playerPrefab);
        GameObject go = Instantiate(player_PositionCollider);
        go.transform.parent = player.transform;
        go.transform.position = new Vector2(0, -0.55f);
        player.transform.position = new Vector2(-7, 33); 
    }

    private void Start()
    {
        gameManager.acceptInput = false;

        StartCoroutine(OnLevelStart());
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = timeBetweenBoulderSpawns;
            SpawnBoulder();
        }

        // Debug.Log(dialogueCanvas.activeInHierarchy);
    }

    private IEnumerator OnLevelStart()
    {
        yield return new WaitForSeconds(0.5f);

        gameManager.isPlayerDead = false;

        // Fade into gameplay.
        StartCoroutine(uiManager.FadeOut(fadeImage, fadeAmount, fadeSpeed));
        yield return new WaitForSeconds(1f);
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("down", new Vector2(-7, 31), 0.9f));
        // Wait until player walk has completed.
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == new Vector2(-7, 31));
        // Stop player movement animations.
        player.GetComponent<PlayerScript>().StopMovementAnimation();
        yield return new WaitForSeconds(1.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        if (!gameManager.hasPlayerDiedInLevelTwo) { dialogueSystem.StartDialogue(1, startingLines); }
        else { dialogueSystem.StartDialogue(1, linesOnRespawn); }
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        // Enable movement input.
        gameManager.acceptInput = true;
    }

    // BOULDER LOGIC

    // Random number between 1 and 3.
    private int WhereToSpawnBoulder()
    {
        return Random.Range(1, 4);
    }

    // Use this to spawn a boulder.
    private void SpawnBoulder()
    {
        // Get lane to spawn boulder in.
        int lane = WhereToSpawnBoulder();

        // Initiliase boulder var by creating boulder from prefab.
        GameObject boulder = Instantiate(boulderPrefab);
        boulder.name = boulderPrefab.name; // Set name.

        // Get lane int and set boulder script based on that.
        switch (lane)
        {
            case 1: // Lane 1
                boulder.transform.position = new Vector2(boulderSpawnLoc[0].position.x, boulderSpawnLoc[0].position.y);
                boulder.GetComponent<BoulderScript>().boulder_DeathLocation = boulderDeathLoc[0];
                boulder.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 11;
                break;
                // Sorting order 12 between.
            case 2: // Lane 2
                boulder.transform.position = new Vector2(boulderSpawnLoc[1].position.x, boulderSpawnLoc[1].position.y);
                boulder.GetComponent<BoulderScript>().boulder_DeathLocation = boulderDeathLoc[1];
                boulder.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 13;
                break;
                // Sorting order 14 between.
            case 3: // Lane 3
                boulder.transform.position = new Vector2(boulderSpawnLoc[2].position.x, boulderSpawnLoc[2].position.y);
                boulder.GetComponent<BoulderScript>().boulder_DeathLocation = boulderDeathLoc[2];
                boulder.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 15;
                break;

        }
    }

    public void PlayerDeath(GameObject player)
    {
        StartCoroutine(coroutine_PlayerDeath(player));
    }


    // Call on boulder when collision with player.
    public IEnumerator coroutine_PlayerDeath(GameObject player)
    {
        gameManager.isPlayerDead = true;
        gameManager.hasPlayerDiedInLevelTwo = true;
        // For now destroy player but
        // vvv replace this with an animation when we get one...
        Destroy(player);
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, fadeAmount, fadeSpeed));
        yield return new WaitUntil(() => fadeImage.color.a >= 1); // Wait until fade image alpha is 1.
        yield return new WaitForSeconds(0.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, onDeathLines);
        yield return new WaitUntil(() => dialogueCanvas.activeInHierarchy == false);     
        yield return new WaitForSeconds(1f);
        // Restart scene.
        RestartPlayer();
    }

    public void RestartPlayer()
    {
        Debug.Log("Switching scene to Level2");
        sceneManager.func_SceneSwitch("Level2");
    }
}