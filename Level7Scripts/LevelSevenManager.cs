using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSevenManager : MonoBehaviour
{
    #region Managers
    private GameManager gameManager;
    private UIManager uiManager;
    private _SceneManager sceneManager;
    #endregion

    #region Room Info
    [SerializeField] private GameObject[] room_doorParent;
    private int room_amountOfDoors = 3;
    private int room_currentRoom = 1;
    #endregion

    #region Player
    private GameObject player;
    private PlayerScript player_script;
    #endregion

    #region Dialogue Lines
    private string[] levelStartLines = {

        "Nice to speak with you again, Quetzalcoatl.",
        "In front of you should be three doors.",
        "Behind one of them lies a hungry Jaguar.",
        "The other two doors are safe.",
        "You must pick the correct door to progress.",
        "Make sure to keep a note of what doors are safe!",
        "Left is always right, right?"

    };

    private string[] correctDoor = {

        "That's the correct door!",
        "Keep going Quetzalcoatl...",
        "Time is of the essence!"

    };

    private string[] levelFinishLines = {

        "Good job on getting past the Jaguar!",
        "To the next layer!"

    };

    private string[] onRespawnLines = {

        "Glad you are back...",
        "I hope you remembered what doors are safe..."

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

    [SerializeField] private Image image_jaguarEyes;

    private GameObject dialogueCanvas;
    private DialogueScript dialogueSystem;
    #endregion

    #region Audio
    [Space(5)]
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audio_source;

    [Space(3)]
    [Header("Sounds")]
    public AudioClip sound_openDoor;
    public AudioClip sound_correctDoor;
    public AudioClip sound_incorrectDoor;
    public AudioClip sound_jaguar;
    #endregion

    [Space(5)]
    [Header("Testing")]
    public TextMeshProUGUI text_testing;

    private GameObject camera_holder;

    private Coroutine walk_cr;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();

        camera_holder = GameObject.FindWithTag("CameraHolder");

        dialogueCanvas = GameObject.FindWithTag("DialogueCanvas");
        dialogueSystem = dialogueCanvas.transform.GetChild(0).GetComponent<DialogueScript>();
        dialogueCanvas.SetActive(false);

        // if player hasn't been in level in this game instance
        if (!gameManager.hasPlayerDiedInLevelSeven) { RandomiseDoors(); }

        InstantiateDoors();

        StartCoroutine(cr_StartLevel());
    }

    // this is for testing
    private void Update()
    {
        if (text_testing.isActiveAndEnabled)
        {
            if (player != null)
            {
                text_testing.text = "Player Position: X = " + player.transform.position.x + "  //  Y  = " + player.transform.position.y;
            }
            else
            {
                text_testing.text = "No player found in scene";
            }
        }
    }

    private IEnumerator cr_StartLevel()
    {
        // Disable movement input.
        gameManager.acceptInput = false;

        // Spawn player.
        player = Instantiate(gameManager.playerPrefab);
        player.AddComponent<LevelSevenPlayer>();
        player.GetComponent<LevelSevenPlayer>().levelSevenManager = this.GetComponent<LevelSevenManager>();
        player.transform.position = new Vector2(-0.5f, 8.3f);
        player.transform.name = gameManager.playerPrefab.name;
        player_script = player.GetComponent<PlayerScript>();

        Vector2 destination = new Vector2(-0.5f, 14.5f);
        yield return new WaitForSeconds(0.5f);
        gameManager.isPlayerDead = false;
        // Fade into gameplay.
        StartCoroutine(uiManager.GetComponent<UIManager>().FadeOut(fadeImage, 0.5f, 0.005f));
        yield return new WaitForSeconds(1f);
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("up", destination, 1.5f));
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == destination);
        yield return new WaitForSeconds(1.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        if (!gameManager.hasPlayerDiedInLevelSeven) { dialogueSystem.StartDialogue(1, levelStartLines); }
        else { dialogueSystem.StartDialogue(1, onRespawnLines); }
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        // Stop camera following.
        Destroy(camera_holder.GetComponent<CameraFollowYAxis>());
        // Pan camera upwards.
        float speed = 2.1f;
        while (new Vector2(camera_holder.transform.position.x, camera_holder.transform.position.y) != new Vector2(-0.5f, 19.5f))
        {
            float step = speed * Time.deltaTime;
            camera_holder.transform.position = Vector2.MoveTowards(camera_holder.transform.position, new Vector2(-0.5f, 19.5f), step);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        // Enable movement input.
        gameManager.acceptInput = true;
    }

    // Use this when interacting with a door.
    public IEnumerator InteractWithDoor(GameObject door_collidingWith)
    {
        // Initialise variables.
        float fadeToNextRoom = door_collidingWith.transform.position.y + 1f;
        Vector2 infrontOfDoor = new Vector2(door_collidingWith.transform.position.x, door_collidingWith.transform.position.y - 0.5f);
        Vector2 behindDoor = new Vector2(door_collidingWith.transform.position.x, door_collidingWith.transform.position.y + 3f);

        // Save whether the door interacted with is a bad door.
        bool isBadDoor = door_collidingWith.GetComponent<DoorInfo>().isBadDoor;

        // Disable moveemnt input.
        gameManager.acceptInput = false;

        // Stop current movement animation to avoid shaky animations
        player_script.StopMovementAnimation();
        // Walk player infront of door.
        StartCoroutine(player_script.Walk("up", infrontOfDoor, 0.5f));
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == infrontOfDoor);
        yield return new WaitForEndOfFrame();
        // Set player to face upwards.
        player_script.SetPlayerAnimationDirection("up");
        // Open door.
        PlaySound(sound_openDoor);
        Destroy(door_collidingWith);
        yield return new WaitForSeconds(1f);
        // Walk player to behine door.
        walk_cr = StartCoroutine(player_script.Walk("up", behindDoor, 1f));
        yield return new WaitUntil(() => player.transform.position.y >= fadeToNextRoom);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.6f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1f);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForEndOfFrame();
        // If this door isn't a bad door.
        if (!isBadDoor) 
        {
            if (room_currentRoom <= room_doorParent.Length) { StartCoroutine(ChoseCorrectDoor()); }
            else { StartCoroutine(cr_FinishLevel()); }
        }
        // If this door is a bad door.
        else {  StartCoroutine(cr_PlayerDeath()); }
    }

    // If chose the correct door.
    private IEnumerator ChoseCorrectDoor()
    {
        // Stop walking coroutine.
        StopCoroutine(walk_cr);
        yield return new WaitForSeconds(0.5f);
        // Play sound.
        PlaySound(sound_correctDoor);
        yield return new WaitForSeconds(0.5f);
        // If this room is the first room.
        if (room_currentRoom == 1) 
        { 
            player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 16);
        }
        else
        {
            player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 18);
        }
        Vector2 walk_destination = new Vector2(player.transform.position.x, player.transform.position.y + 4.5f);
        camera_holder.transform.position = new Vector2(camera_holder.transform.position.x, camera_holder.transform.position.y + 30);
        // Start player walk.
        StartCoroutine(player_script.Walk("up", walk_destination, 1.5f));
        // Fade into gameplay.
        StartCoroutine(uiManager.FadeOut(fadeImage, 0.6f, 0.005f));
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == walk_destination);
        // Increment room.
        room_currentRoom++;
        // Enable movement input.
        gameManager.acceptInput = true;
    }

    #region Player Death
    public IEnumerator cr_PlayerDeath()
    {
        gameManager.isPlayerDead = true;
        gameManager.hasPlayerDiedInLevelSeven = true;
        // Stop walk coroutine.
        StopCoroutine(walk_cr);
        // Destroy player for now but
        // vvv replace this with an animation when we get one...
        Destroy(player);
        yield return new WaitForSeconds(0.5f);
        // Play sound
        PlaySound(sound_incorrectDoor);
        yield return new WaitForSeconds(1f);
        // Play jaguar growl sound
        PlaySound(sound_jaguar);
        // Fade in jaguar eyes.
        StartCoroutine(uiManager.FadeIn(image_jaguarEyes, 0.5f, 0.005f));
        yield return new WaitUntil(() => image_jaguarEyes.color.a >= 1f);
        yield return new WaitForSeconds(1f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, onDeathLines);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        yield return new WaitForSeconds(0.5f);
        // Fade out jaguar eyes.
        StartCoroutine(uiManager.GetComponent<UIManager>().FadeOut(image_jaguarEyes, 0.5f, 0.005f));
        yield return new WaitUntil(() => image_jaguarEyes.color.a <= 0);
        yield return new WaitForSeconds(0.5f);
        // Restart scene.
        sceneManager.RestartScene();
    }
    #endregion

    private IEnumerator cr_FinishLevel()
    {
        gameManager.hasPlayerDiedInLevelSeven = false;
        yield return new WaitForSeconds(1f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, levelFinishLines);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        // Run level transition to next level.
        sceneManager.RunLevelTransition(8);
    }

    #region Door Functions
    private void RandomiseDoors()
    {
        gameManager.door_BadDoors = new int[room_doorParent.Length];

        for (int i = 0; i < gameManager.door_BadDoors.Length; i++) { gameManager.door_BadDoors[i] = Random.Range(0, room_amountOfDoors); }

        Debug.Log(gameManager.door_BadDoors[0]);
    }

    private void InstantiateDoors()
    {
        for (int i = 0; i < room_doorParent.Length; i++)
        {
            room_doorParent[i].transform.GetChild(gameManager.door_BadDoors[i]).GetComponent<DoorInfo>().isBadDoor = true;
        }
    }
    #endregion

    public void PlaySound(AudioClip clip)
    {
        audio_source.PlayOneShot(clip);
    }
}
