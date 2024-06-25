using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class LevelThreeManager : MonoBehaviour
{
    // Things that are required in this level:
    // - Death from wall spikes
    // - Death from falling stuff (skulls/stones on floor)
    // - Respawn mechanic with speech from Axotl
    // - Spawn speech from Xolotl with instructions

    private GameObject player;
    private PlayerScript player_script;
    #region Dialogue Lines
    private string[] levelStartLines = {

        "Welcome to layer three, the Spikey Valley",
        "This level is self explanatory... don’t touch the walls and you will be alright.",
        "Oh and one more thing, I heard some of the rocks above are unstable.",
        "Be sure to look extra carefully to spot where you should and should not go!",
        "Best of luck, Quetzalcoatl!"

    };
    private string[] onRespawnLines = {

        "Glad you're back!",
        "Now back to buisness...",
        "Avoid the spikes and unstable rocks, and get to the end!",
        "Good luck!"

    };
    private string[] onDeathLines = {

        "Ouch...",
        "That looked like it hurt!",
        "Time to try again I guess!"

    };
    #endregion
    #region Canvas Info
    private GameObject dialogueCanvas;
    private DialogueScript dialogueSystem;
    #endregion
    #region Managers
    private GameManager gameManager;
    private _SceneManager sceneManager;
    private UIManager uiManager;
    #endregion
    #region Fading
    // Black image used for fading in and out.
    [SerializeField] private Image fadeImage;
    private float fadeSpeed = 0.005f;
    #endregion
    #region Camera
    private GameObject cameraHolder;
    #endregion
    [Space(5)]
    [Header("Falling Rock Prefab")]
    [SerializeField] private GameObject prefab_fallingRock;


    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();
        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueSystem = dialogueCanvas.transform.GetChild(0).gameObject.GetComponent<DialogueScript>();
        cameraHolder = GameObject.FindWithTag("CameraHolder");
        player = Instantiate(gameManager.playerPrefab);
        player_script = player.GetComponent<PlayerScript>();

    }

    private void Start()
    {
        dialogueCanvas.SetActive(false);
        StartCoroutine(co_StartLevel());
    }

    private IEnumerator co_StartLevel()
    {
        // Add level three player component to player.
        player.AddComponent<LevelThreePlayer>();
        player.GetComponent<LevelThreePlayer>().SetLevelThreeManager(this);
        // Move player.
        player.transform.position = new Vector2(-15.5f, 1f);
        // Disable movement input.
        gameManager.acceptInput = false;
        yield return new WaitForSeconds(0.5f);
        gameManager.isPlayerDead = false;
        // Fade into gameplay.
        StartCoroutine(uiManager.GetComponent<UIManager>().FadeOut(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a <= 0.5f);
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("right", new Vector2(-10.5f, 1f), 0.9f));
        // Wait for walk to be completed.
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == new Vector2(-10.5f, 1f));
        // Stop player movement animation.
        player.GetComponent<PlayerScript>().StopMovementAnimation();
        yield return new WaitForSeconds(1.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        if (!gameManager.hasPlayerDiedInLevelThree) { dialogueSystem.StartDialogue(1, levelStartLines); }
        else { dialogueSystem.StartDialogue(1, onRespawnLines); }
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        // Enable input.
        gameManager.acceptInput = true;
    }

    public void OnPlayerCollisionWithSpikedWall() { StartCoroutine(coroutine_PlayerDeathSpikes(player)); }

    // Call on collision with spike wall.
    public IEnumerator coroutine_PlayerDeathSpikes(GameObject player)
    {
        gameManager.isPlayerDead = true;
        // Disable player input.
        gameManager.acceptInput = false;
        // For now destroy player but
        // vvv replace this with an animation when we get one...
        Destroy(player);
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.9f, 0.005f));
        yield return new WaitForSeconds(2f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, onDeathLines);
        yield return new WaitUntil(() => dialogueCanvas.activeInHierarchy == false);
        yield return new WaitForSeconds(1f);
        // Set this to true so it players respawn dialogue.
        gameManager.hasPlayerDiedInLevelThree = true;
        // Restart scene.
        sceneManager.RestartScene();
    }

    public void StartFallingSpikeCR(Vector2 pos)
    {
        StartCoroutine(cr_FallingRockDeath(pos));
    }

    private IEnumerator cr_FallingRockDeath(Vector2 pos)
    {
        gameManager.isPlayerDead = true;
        gameManager.hasPlayerDiedInLevelThree = true;
        // Disable player input.
        gameManager.acceptInput = false;
        // Stop player movement animation.
        player_script.StopMovementAnimation();
        yield return new WaitForSeconds(0.5f);
        // Instantiate falling rock prefab.
        var go = Instantiate(prefab_fallingRock);
        go.transform.position = pos; // Move to above trigger.
        go.transform.GetChild(0).GetComponent<PlayAnimationScript>().shouldAnimationBeRunning = true; // Start animation.
        yield return new WaitForSeconds(0.6f);
        Destroy(player); // Destroy player.
        yield return new WaitForSeconds(1f);
        go.transform.GetChild(0).GetComponent<PlayAnimationScript>().shouldAnimationBeRunning = false; // Stop animation.
        yield return new WaitForSeconds(1f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.9f, 0.005f));
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


    public IEnumerator cr_FinishLevel()
    {
        yield return new WaitForSeconds(0.5f);

        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.7f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        // Run level transition to next level.
        sceneManager.RunLevelTransition(4);
    }
}
