using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSixManager : MonoBehaviour
{
    private GameManager gameManager;
    private _SceneManager sceneManager;
    private UIManager uiManager;

    #region Arrow Info
    [Header("Arrow Settings")]
    [Space(3)]
    [Header("Prefab")]
    [SerializeField] private GameObject arrow_prefab;
    [Header("Arrow Speed:")]
    public float arrow_speedX;
    public float arrow_speedY;
    [Space(1)]
    private float arrow_collumnPositionX;
    [Header("Arrow Positions")]
    [SerializeField] private float arrow_collumnTopY;
    [SerializeField] private float arrow_collumnBottomY;
    [SerializeField] private Vector2 arrow_rowLeft;
    [SerializeField] private Vector2 arrow_rowRight;
    [Space(1)]
    [Header("Arrow Timings")]
    [SerializeField] private float arrow_timeBetweenArrows;
    [SerializeField] private float arrow_warningTime;
    #endregion

    [Header("Player - Animations")]
    public Sprite[] sprites_playerWithSnakeFlamethrower_left;
    public Sprite[] sprites_playerWithSnakeFlamethrower_right;
    public Sprite[] sprites_playerWithSnakeFlamethrower_up;
    public Sprite[] sprites_playerWithSnakeFlamethrower_down;
    [Header("Fire - Animations")]
    public Sprite[] sprites_fire_left;
    public Sprite[] sprites_fire_right;
    public Sprite[] sprites_fire_up;
    public Sprite[] sprites_fire_down;
    [Header("Fire - Prefab")]
    [SerializeField] private GameObject fire_prefab;
    [HideInInspector] public GameObject fire_go;

    bool canShootArrow = false;

    [Space(10)]

    [Header("Walk Settings")]
    [SerializeField] private Vector2 walk_destination;
    [SerializeField] private float walk_speed;

    private GameObject player;

    #region Audio
    [Space(6)]
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audio_source;
    public float minPitch;
    public float maxPitch;

    [Space(3)]
    [Header("Sounds")]
    public AudioClip sound_arrow;
    public AudioClip sound_block;
    #endregion

    #region Dialogue Lines
    private string[] levelStartLines = {

        "Glad to see you made it in one piece.",
        "I will need you to walk slowly for this part of your journey.",
        "You have entered the Swarming Arrow cave and the Arrows react negatively to movement.",
        "*Use the arrow keys to block the incoming arrows*",
        "Speak with you again soon, Quetzalcoatl."

    };

    private string[] levelFinishLines = {

        "Good job on making it past the arrows!",
        "Anyway time to get moving...",
        "Time is of the essence!"

    };

    private string[] onRespawnLines = {

        "Good!",
        "You are back...",
        "Anyway time to get back to it.",
        "Get to the other side but be careful...",
        "Make sure to tread lightly and watch for incoming projectiles!"

    };

    private string[] onDeathLines = {

        "Ouch...",
        "That looked like it hurt!",
        "Time to try again!"

    };
    #endregion

    #region UI
    [Space(10)]
    [Header("Fade Settings")]
    public Image fadeImage;
    private float fadeSpeed = 0.005f;

    private GameObject dialogueCanvas;
    private DialogueScript dialogueSystem;
    #endregion

    [SerializeField] private GameObject[] warnings;

    [HideInInspector] public bool startWalk = false;

    private float timer;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueSystem = dialogueCanvas.transform.GetChild(0).gameObject.GetComponent<DialogueScript>();

        dialogueCanvas.SetActive(false);

        fadeImage.color = new Color(0, 0, 0, 1);

        gameManager.acceptInput = false;
    }

    private void Start()
    {
        StartCoroutine(cr_StartLevel());
        StartCoroutine(cr_StartMainWalk());
    }

    private void Update()
    {
        if (player != null) { arrow_collumnPositionX = player.transform.position.x; }
        ShowLines();

        if (startWalk)
        {
            if (canShootArrow)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    timer = arrow_timeBetweenArrows;
                    StartCoroutine(cr_SpawnArrow());
                }
            }
        }
    }

    private IEnumerator cr_StartLevel()
    {
        // Spawn player
        player = Instantiate(gameManager.playerPrefab);
        player.AddComponent<LevelSixPlayer>();
        player.GetComponent<LevelSixPlayer>().levelSixManager = this;
        player.transform.name = gameManager.playerPrefab.name;
        // Spawn fire animation game object
        fire_go = Instantiate(fire_prefab);
        fire_go.transform.parent = player.transform; // Set fire game object's parent to player game object.
        fire_go.transform.position = new Vector3(0, 0, 0);

        // Move player position.
        player.transform.position = new Vector2(-23, 0.5f);

        Vector2 destination = new Vector2(-11.5f, 0.5f);
        // Disable movement input.
        gameManager.acceptInput = false;
        yield return new WaitForSeconds(0.5f);
        gameManager.isPlayerDead = false;
        // Fade into gameplay.
        StartCoroutine(uiManager.GetComponent<UIManager>().FadeOut(fadeImage, 0.7f, 0.005f));
        yield return new WaitForSeconds(1f);
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("right", destination, 1.5f));
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == destination);
        yield return new WaitForSeconds(1.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        if (!gameManager.hasPlayerDiedInLevelSix) { dialogueSystem.StartDialogue(1, levelStartLines); }
        else { dialogueSystem.StartDialogue(1, onRespawnLines); }
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        // Start walk.
        startWalk = true;
    }

    private IEnumerator cr_StartMainWalk()
    {
        yield return new WaitUntil(() => startWalk);
        yield return new WaitForSeconds(1f);
        // Set start shooting arrows.
        canShootArrow = true;
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("right", walk_destination, walk_speed));
        // Wait until the end of the walk.
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == walk_destination);
        yield return new WaitForEndOfFrame();
        FinishWalk();
        
    }

    private void FinishWalk()
    {
        startWalk = false;

        StopAllCoroutines();
        ClearWarningsAndArrows();

        StartCoroutine(cr_FinishLevel());
    }

    private IEnumerator cr_FinishLevel()
    {
        Vector2 positionToWalkTo = new Vector2(75, 0.5f);
        float positionToStartFade = 69f;
        gameManager.hasPlayerDiedInLevelSix = false;
        // If fire sprite is enabled, disable it.
        if (fire_go.GetComponent<SpriteRenderer>().enabled) { fire_go.GetComponent<SpriteRenderer>().enabled = false; }
        yield return new WaitForSeconds(1.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, levelFinishLines);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("right", positionToWalkTo, 1.1f));
        yield return new WaitUntil(() => player.transform.position.x >= positionToStartFade);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.7f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1f);
        yield return new WaitForSeconds(1f);
        // Run level transition to next level.
        sceneManager.RunLevelTransition(7);
    }

    #region Arrow Functions
    private string ChooseArrowDirection()
    {
        int randNum = Random.Range(1, 5);

        switch(randNum)
        {
            case 1:
                return "up";
            case 2:
                return "down";
            case 3:
                return "left";
            case 4:
                return "right";
        }
        return null;
    }

    private void InstantiateArrow(string direction)
    {
        GameObject go;

        switch (direction)
        {
            case ("up"):

                go = Instantiate(arrow_prefab);
                go.transform.position = new Vector2(arrow_collumnPositionX, arrow_collumnBottomY);
                go.GetComponent<WindInfo>().destination = new Vector2(arrow_collumnPositionX + 0.5f, arrow_collumnTopY);
                go.GetComponent<WindInfo>().moveSpeed = arrow_speedY;

                go.GetComponent<WindInfo>().direction = "up";

                go.transform.localEulerAngles = new Vector3(0, 0, 0);
                
                go.GetComponent<WindInfo>().startMoving = true;

                break;

            case ("down"):

                go = Instantiate(arrow_prefab);
                go.transform.position = new Vector2(arrow_collumnPositionX, arrow_collumnTopY);
                go.GetComponent<WindInfo>().destination = new Vector2(arrow_collumnPositionX + 0.5f, arrow_collumnBottomY);
                go.GetComponent<WindInfo>().moveSpeed = arrow_speedY;

                go.transform.localEulerAngles = new Vector3(0, 0, 180);

                go.GetComponent<WindInfo>().direction = "down";

                go.GetComponent<WindInfo>().startMoving = true;
                
                break;

            case ("left"):

                go = Instantiate(arrow_prefab);
                go.transform.position = arrow_rowRight;
                go.GetComponent<WindInfo>().destination = arrow_rowLeft;
                go.GetComponent<WindInfo>().moveSpeed = arrow_speedX;

                go.GetComponent<WindInfo>().direction = "left";

                go.transform.localEulerAngles = new Vector3(0, 0, 90);

                go.GetComponent<WindInfo>().startMoving = true;

                break;

            case ("right"):

                go = Instantiate(arrow_prefab);
                go.transform.position = arrow_rowLeft;
                go.GetComponent<WindInfo>().destination = arrow_rowRight;
                go.GetComponent<WindInfo>().moveSpeed = arrow_speedX;

                go.transform.localEulerAngles = new Vector3(0, 0, -90);

                go.GetComponent<WindInfo>().direction = "right";

                go.GetComponent<WindInfo>().startMoving = true;

                break;
        }
    }

    private IEnumerator cr_SpawnArrow()
    {
        canShootArrow = false;
        // Set direction.
        string direction = ChooseArrowDirection();
        int warningIndex = 0;
        // Set warning active.
        if (direction == "left") { warningIndex = 0; }
        else if (direction == "right") { warningIndex = 1; }
        else if (direction == "up") { warningIndex = 2; }
        else if (direction == "down") { warningIndex = 3; }
        warnings[warningIndex].SetActive(true);
        yield return new WaitForSeconds(arrow_warningTime);
        // Instantiate arrow with direction.
        InstantiateArrow(direction);
        yield return new WaitUntil(() => !GameObject.FindWithTag("Arrow"));
        warnings[warningIndex].SetActive(false);
        canShootArrow = true;
    }
    #endregion

    #region Player Death
    public void PlayerDeath()
    {
        StopAllCoroutines();
        StartCoroutine(coroutine_PlayerDeath(player));
    }

    public IEnumerator coroutine_PlayerDeath(GameObject player)
    {
        gameManager.isPlayerDead = true;
        gameManager.hasPlayerDiedInLevelSix = true;
        // Destroy player for now but
        // vvv replace this with an animation when we get one...
        Destroy(player);
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.7f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, onDeathLines);
        yield return new WaitUntil(() => dialogueCanvas.activeInHierarchy == false);
        yield return new WaitForSeconds(0.5f);
        // Restart scene.
        sceneManager.RestartScene();
    }
    #endregion

    private void ClearWarningsAndArrows()
    {
        for (int i = 0; i < warnings.Length; i++) { warnings[i].SetActive(false); }
        Destroy(GameObject.FindWithTag("Arrow"));
    }

    private void ShowLines()
    {
        // Draw rows
        Debug.DrawLine(arrow_rowLeft, arrow_rowRight, Color.magenta);
        Debug.DrawLine(new Vector2(arrow_collumnPositionX, arrow_collumnTopY), new Vector2(arrow_collumnPositionX, arrow_collumnBottomY), Color.magenta);
    }

    public void RandomisePitch()
    {
        audio_source.pitch = Random.Range(minPitch, maxPitch);
    }
    public void PlaySound(AudioClip clip, bool randomPitch)
    {
        // if random pitch then randomise, else default
        if (randomPitch)
        {
            RandomisePitch();
        }
        else
        {
            audio_source.pitch = 1;
        }

        audio_source.PlayOneShot(clip);
    }
}
