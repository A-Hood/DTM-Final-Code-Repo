using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;


    // Storing whether the intro has been played: is checked upon reloading the main menu.
    [HideInInspector] public bool introPlayed;
    [HideInInspector] public bool startScreenPlayed;

    // Save data will not be required due to the nature of the environment it will be played in. Only if released.
    #region Save Data
    [HideInInspector] public bool levelOneCompleted = false;
    [HideInInspector] public bool levelTwoCompleted = false;
    [HideInInspector] public bool levelThreeCompleted = false;
    [HideInInspector] public bool levelFourCompleted = false;
    [HideInInspector] public bool levelSixCompleted = false;
    [HideInInspector] public bool levelSevenCompleted = false;
    [HideInInspector] public bool levelEightCompleted = false;
    [HideInInspector] public bool levelNineCompleted = false;
    #endregion

    // Player prefab
    [Header("Player Prefab")]
    public GameObject playerPrefab;

    [Space(10)]

    // Should sound effects play in the levels.
    [Header("Play Sound Effects?")]
    public bool playSoundEffects;

    // Should accept movement input.
    [HideInInspector] public bool acceptInput = true;

    [Space(10)]

    // Keybindings
    [Header("Keybinds")]
    public KeyCode key_Interact;

    // For level 9 minigame
    [HideInInspector] public KeyCode key_FirstLane = KeyCode.Alpha1;
    [HideInInspector] public KeyCode key_SecondLane = KeyCode.Alpha2;
    [HideInInspector] public KeyCode key_ThirdLane  = KeyCode.Alpha3;
    [HideInInspector] public KeyCode key_FourthLane = KeyCode.Alpha4;

    // Level one variables. (These shouldn't be here)
    [Header("Level One")]
    [HideInInspector] public int heldWood = 0;
    [HideInInspector] public int bridgeStoredWood = 0;

    // Level four variables. (These shouldn't be here)
    [Header("Level Four")]
    [HideInInspector] public bool hasKey = false;

    // Level seven variables. (These need to be here so we don't lose data upon reloading scene)
    [HideInInspector] public int[] door_BadDoors;

    // Required to activate different dialogue opon reloading scene.
    [HideInInspector] public bool isPlayerDead = false;

    [HideInInspector] public bool hasPlayerDiedInLevelTwo = false;
    [HideInInspector] public bool hasPlayerDiedInLevelThree = false;
    [HideInInspector] public bool hasPlayerDiedInLevelFive = false;
    [HideInInspector] public bool hasPlayerDiedInLevelSix = false;
    [HideInInspector] public bool hasPlayerDiedInLevelSeven = false;
    [HideInInspector] public bool hasPlayerDiedInLevelEight = false;

    [Space(5)]
    // Speech sprites for each character. Respective numbers to reference next to name.
    [Header("All Character Sprites")]
    public Sprite[] sprites_xolotl; // 1
    public Sprite[] sprites_mictlantecuhtli; // 2
    public Sprite[] sprites_vermillion; // 3
    public Sprite[] sprites_bees; // 4

    // Only here to reset to main menu if someone leaves a level open or doens't want to play.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { SceneManager.LoadScene("MainMenu"); }
    }

    private void Awake()
    {
        // Cap FPS
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        // Check if instance exists
        if (Instance)
            Destroy(this.gameObject);

        // Don't destroy object on loading a new scene.
        DontDestroyOnLoad(gameObject);
    }
}
