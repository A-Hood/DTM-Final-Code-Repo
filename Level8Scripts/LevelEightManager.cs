using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEightManager : MonoBehaviour
{
    #region Constants
    #region Managers
    private GameManager gameManager;
    private UIManager uiManager;
    private _SceneManager sceneManager;
    #endregion

    #region Player Info
    private GameObject player;
    private PlayerScript player_script;
    #endregion

    #region Camera Holder
    private GameObject camera_holder;
    #endregion

    #region Dialogue Lines
    private string[] levelStartLines = {

        "Nice you didn’t become Jaguar food.",
        "I have a question for you Quetzalcoatl.",
        "Do you like boats?!?",
        "Because I love boats!!!",
        "And guess what…",
        "You are gonna sail a boat to Mictlan.",
        "Oh I almost forgot, the waters are dangerous and you might encounter obstacles.",
        "Have a great sail !!!"

    };

    private string[] levelFinishLines = {

        "Good job on getting past the treacherous waters!",
        "To the next layer!"

    };

    private string[] onRespawnLines = {

        "Glad we could drag you from those treachorous waters.",
        "Anyways get moving...",
        "We need to make it to Mictlan!"

    };

    private string[] onDeathLines = {

        "Ouch...",
        "That looked like it hurt!",
        "Time to try again!"

    };
    #endregion

    #region Boat Info
    private GameObject boat_go;
    private BoatScript boat_script;
    #endregion

    #region Lines Info
    [Header("Lane Positions")]
    [SerializeField] private Transform[] lane_onePositions;
    [SerializeField] public Transform[] lane_twoPositions;
    [SerializeField] public Transform[] lane_threePositions;
    #endregion

    #region Blocker Info
    [Header("Blocker Info")]
    public GameObject blocker_prefab;
    public float blocker_timeBetweenSpawns;
    #endregion

    #region UI
    [Space(10)]
    [Header("Fade Settings")]
    public Image fadeImage;
    private float fadeSpeed = 0.005f;

    private GameObject dialogueCanvas;
    private DialogueScript dialogueSystem;
    #endregion
    #endregion

    [HideInInspector] public bool boatCanBeControlled = false;

    public bool canSpawnBlockers = false;

    private float timer;

    private ProgressBarScript progressbar_script;
    private bool shouldProgressBarBeMoving = false;

    public bool hasEnteredBoat = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audio_generalSource;
    [SerializeField] private AudioSource audio_water;
    private float audio_volume;

    [Header("Sounds")]
    public AudioClip[] sound_rowing;
    public AudioClip sound_laneSwitch;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();

        boat_go = GameObject.FindWithTag("Boat");
        boat_script = boat_go.GetComponent<BoatScript>();

        camera_holder = GameObject.FindWithTag("CameraHolder");

        progressbar_script = GameObject.FindWithTag("ProgressBar").GetComponent<ProgressBarScript>();
        progressbar_script.gameObject.SetActive(false);

        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueSystem = dialogueCanvas.transform.GetChild(0).gameObject.GetComponent<DialogueScript>();
        dialogueCanvas.SetActive(false);

        StartCoroutine(cr_StartLevel());
    }

    private void Update()
    {
        ShowLines();

        if (canSpawnBlockers)
        {
            if (timer > 0) { timer -= Time.deltaTime; }
            
            else
            {
                timer = blocker_timeBetweenSpawns;
                SpawnBlockers();
            }
        }
        if (boat_go != null)
        {
            if (shouldProgressBarBeMoving) { progressbar_script.MoveProgressBar(5.5f, 240f, boat_go.transform.position.x); }
        }
    }

    private IEnumerator cr_StartLevel()
    {
        // Disable movement input.
        gameManager.acceptInput = false;

        // Spawn player.
        player = Instantiate(gameManager.playerPrefab);
        player.AddComponent<LevelEightPlayer>();
        player.GetComponent<LevelEightPlayer>().level_manager = this;
        player.transform.position = new Vector2(-15f, -0.5f);
        player.transform.name = gameManager.playerPrefab.name;
        player_script = player.GetComponent<PlayerScript>();

        Vector2 destination = new Vector2(-9.5f, -0.5f);
        yield return new WaitForSeconds(0.5f);
        gameManager.isPlayerDead = false;
        // Fade into gameplay.
        StartCoroutine(uiManager.FadeOut(fadeImage, 0.5f, 0.005f));
        yield return new WaitForSeconds(1f);
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("right", destination, 1.5f));
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == destination);
        yield return new WaitForSeconds(1.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        if (!gameManager.hasPlayerDiedInLevelEight) { dialogueSystem.StartDialogue(1, levelStartLines); }
        else { dialogueSystem.StartDialogue(1, onRespawnLines); }
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        yield return new WaitForSeconds(0.5f);
        // Enable movement input.
        gameManager.acceptInput = true;
    }

    #region Boat Coroutines
    // Call this to enter boat.
    public void EnterBoat()
    {
        hasEnteredBoat = true;
        StartCoroutine(cr_EnterBoat());
    }
    private IEnumerator cr_EnterBoat()
    {
        // Disable movement input.
        gameManager.acceptInput = false;
        // Stop movement animation.
        player_script.StopMovementAnimation();
        Vector2 walkToInfrontOfBoat = new Vector2(-6.6f, -0.9f);
        Vector2 inBoat = new Vector2(-4.98f, -0.9f);
        // Destroy boat enter collider.
        Destroy(boat_go.GetComponent<CircleCollider2D>());
        // Start player walk.
        StartCoroutine(player_script.Walk("right", walkToInfrontOfBoat, 1f));
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == walkToInfrontOfBoat);
        yield return new WaitForEndOfFrame();
        // Set sprite direction.
        player_script.SetPlayerAnimationDirection("right");
        yield return new WaitForSeconds(0.5f);
        // Start player walk to into boat.
        StartCoroutine(player_script.Walk("right", inBoat, 1.5f));
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == inBoat);
        // Set player parent to boat go
        player.transform.parent = boat_go.transform;
        // Wait to start the boat moving
        yield return new WaitForSeconds(1f);
        // Start before main ride.
        StartCoroutine(cr_StartOfLevelRide());
    }

    public IEnumerator cr_StartOfLevelRide()
    {
        Vector2 destination = new Vector2(boat_go.transform.position.x + 10, boat_go.transform.position.y);
        float fadeAtY = boat_go.transform.position.x + 5;
        // Set moving boat to be active
        boat_script.SetMovingBoatActive();
        // Start moving boat forward.
        StartCoroutine(boat_script.cr_MoveBoatForward(destination.x, 2.3f));
        // Fade when here.
        yield return new WaitUntil(() => boat_go.transform.position.x >= fadeAtY);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => new Vector2(boat_go.transform.position.x, boat_go.transform.position.y) == destination);
        yield return new WaitForEndOfFrame();
        // Start main boat ride.
        StartMainBoatRide();
    }

    private IEnumerator cr_MainBoatRide()
    {
        // Move boat position.
        boat_go.transform.position = lane_twoPositions[0].transform.position;
        // Enable progress bar.
        progressbar_script.gameObject.SetActive(true); // set progress bar to be visible
        // Move camera holder.
        camera_holder.transform.position = new Vector2(5.5f, -62);
        // Add camera component and change stopping points.
        camera_holder.AddComponent<CameraFollowXAxis>();
        camera_holder.GetComponent<CameraFollowXAxis>().xStoppingPoint1 = 5.5f;
        camera_holder.GetComponent<CameraFollowXAxis>().xStoppingPoint2 = 240.5f;
        // Change player sprite to look right.
        player_script.SetPlayerAnimationDirection("right");
        yield return new WaitForSeconds(1f);
        // Fade into gameplay.
        StartCoroutine(uiManager.FadeOut(fadeImage, 0.5f, 0.005f));
        // Start boat movement.
        StartCoroutine(boat_script.cr_MoveBoatForward(lane_twoPositions[1].transform.position.x, 3.5f));
        yield return new WaitUntil(() => fadeImage.color.a <= 0);
        yield return new WaitForSeconds(0.5f);
        // Allow boat to move on the y-axis.
        StartCoroutine(boat_script.cr_MoveBoatVertical(lane_twoPositions[1].transform.position.y, 5));
        // Change bools.
        boatCanBeControlled = true;
        canSpawnBlockers = true;
        // Wait until boat is past first lane point.
        yield return new WaitUntil(() => boat_go.transform.position.x >= lane_twoPositions[0].transform.position.x);
        shouldProgressBarBeMoving = true;
        // Wait until boat gets to the end.
        yield return new WaitUntil(() => boat_go.transform.position.x >= 220.5f);
        canSpawnBlockers = false;
        yield return new WaitUntil(() => boat_go.transform.position.x >= 240f);
        shouldProgressBarBeMoving = false;
        yield return new WaitUntil(() => boat_go.transform.position.x >= 245.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1);
        // End the boat ride.
        EndBoatRide();
    }

    private void StartMainBoatRide()
    {
        StopAllCoroutines();

        StartCoroutine(cr_MainBoatRide());
    }

    private void EndBoatRide()
    {
        // Stop coroutines.
        StopAllCoroutines();
        // Stop boat sounds.
        boat_script.StopBoatSounds();
        boatCanBeControlled = false;
        // Start end of boat ride.
        StartCoroutine(cr_EndBoatRide());
    }

    private IEnumerator cr_EndBoatRide()
    {
        Vector2 boat_destination = new Vector2(117.82f, -1.5f);
        // Destroy camera follow component.
        Destroy(camera_holder.GetComponent<CameraFollowXAxis>());
        camera_holder.transform.position = new Vector2(118, -0.5f);
        // Disable progress bar.
        progressbar_script.gameObject.SetActive(false);
        boat_go.transform.position = new Vector2(102.5f, -1.5f);
        yield return new WaitForSeconds(0.5f);
        // Fade into gameplay.
        StartCoroutine(uiManager.FadeOut(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a <= 0);
        // Start moving boar forward.
        StartCoroutine(boat_script.cr_MoveBoatForward(boat_destination.x, 2f));
        yield return new WaitUntil(() => boat_go.transform.position.x >= boat_destination.x);
        // Set the not moving boat to be active and de-attach player from boat go
        boat_script.SetNotMovingBoatActive();
        player.transform.parent = null;
        yield return new WaitForSeconds(0.5f);
        float tempX = player.transform.position.x + 2.5f;
        // Start player walk out of boat.
        StartCoroutine(player_script.Walk("right", new Vector2(player.transform.position.x + 4, player.transform.position.y), 1.2f));
        yield return new WaitUntil(() => player.transform.position.x >= tempX);
        // Set player to look at camera.
        player_script.SetPlayerAnimationDirection("down");
        yield return new WaitForSeconds(0.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, levelFinishLines);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        yield return new WaitForSeconds(0.5f);
        // Enable movement input.
        gameManager.acceptInput = true;
    }
    #endregion

    public float GetDestinationFromLane(int lane)
    {
        switch (lane)
        {
            case 1:
                return lane_threePositions[1].position.y;
            case 2:
                return lane_twoPositions[1].position.y;
            case 3:
                return lane_onePositions[1].position.y;
        }

        return 0;
    }

    private void SpawnBlockers()
    {
        // 50/50 chance for 1 or 2 boulders to spawn
        int randAmount = Random.Range(1, 3);

        float distanceFromPlayer = 20;

        Debug.Log(randAmount);

        switch (randAmount)
        {
            case 1:
                int randLane = Random.Range(1, 4);
                if (randLane == 1)
                {
                    GameObject go = Instantiate(blocker_prefab);
                    go.transform.position = new Vector2(boat_go.transform.position.x + distanceFromPlayer, lane_onePositions[1].transform.position.y);
                    break;
                }
                else if (randLane == 2)
                {
                    GameObject go = Instantiate(blocker_prefab);
                    go.transform.position = new Vector2(boat_go.transform.position.x + distanceFromPlayer, lane_twoPositions[1].transform.position.y);
                    break;
                }
                else if (randLane == 3)
                {
                    GameObject go = Instantiate(blocker_prefab);
                    go.transform.position = new Vector2(boat_go.transform.position.x + distanceFromPlayer, lane_threePositions[1].transform.position.y);
                    break;
                }

                break;

            case 2:
                // generate random lane 1 to 3
                int randLane1 = Random.Range(1, 4);
                int randLane2 = Random.Range(1, 4);
                while (randLane1 == randLane2) { randLane2 = Random.Range(1, 4); }

                if (randLane1 == 1 || randLane2 == 1) 
                { 
                    GameObject go = Instantiate(blocker_prefab);
                    go.transform.position = new Vector2(boat_go.transform.position.x + distanceFromPlayer, lane_onePositions[1].transform.position.y);
                }
                if (randLane1 == 2 || randLane2 == 2)
                {
                    GameObject go = Instantiate(blocker_prefab);
                    go.transform.position = new Vector2(boat_go.transform.position.x + distanceFromPlayer, lane_twoPositions[1].transform.position.y);
                }
                if (randLane1 == 3 || randLane2 == 3)
                {
                    GameObject go = Instantiate(blocker_prefab);
                    go.transform.position = new Vector2(boat_go.transform.position.x + distanceFromPlayer, lane_threePositions[1].transform.position.y);
                }
                break;
        }
    }

    #region Player Death
    public void PlayerDeath()
    {
        // Stop all coroutines.
        StopAllCoroutines();
        canSpawnBlockers = false;
        boatCanBeControlled = false;
        // Start player death.
        StartCoroutine(coroutine_PlayerDeath());
    }

    // Call on collision with blocker.
    public IEnumerator coroutine_PlayerDeath()
    {
        gameManager.isPlayerDead = true;
        gameManager.hasPlayerDiedInLevelEight = true;
        // Destroy player for now but
        // vvv replace this with an animation when we get one...
        Destroy(boat_go);
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1f);
        yield return new WaitForSeconds(0.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, onDeathLines);
        yield return new WaitUntil(() => dialogueCanvas.activeInHierarchy == false);
        yield return new WaitForSeconds(1f);
        // Restart scene.
        sceneManager.RestartScene();
    }
    #endregion


    public void FinishLevel()
    {
        StartCoroutine(cr_FinishLevel());
    }
    private IEnumerator cr_FinishLevel()
    {
        // Disable movement input.
        gameManager.acceptInput = false;
        gameManager.hasPlayerDiedInLevelEight = false;
        // Stop player movement animation.
        player_script.StopMovementAnimation();
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        // Run level transition to next level.
        sceneManager.RunLevelTransition(9);
    }

    public void PlaySound(AudioClip clip)
    {
        audio_generalSource.PlayOneShot(clip);
    }

    #region Debug
    private void ShowLines()
    {
        Debug.DrawLine(lane_onePositions[0].transform.position, lane_onePositions[1].transform.position, Color.magenta);
        Debug.DrawLine(lane_twoPositions[0].transform.position, lane_twoPositions[1].transform.position, Color.magenta);
        Debug.DrawLine(lane_threePositions[0].transform.position, lane_threePositions[1].transform.position, Color.magenta);
    }
    #endregion
}
