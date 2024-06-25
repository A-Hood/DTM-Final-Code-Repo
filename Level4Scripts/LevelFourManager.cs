using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// amount of ice blocks in each area: 3, 5, 12

public class LevelFourManager : MonoBehaviour
{
    #region Ice Block Settings
    [Header("Ice Block Settings")]
    [SerializeField] private int[] amountOfIceBlocks;

    [SerializeField] private GameObject prefab_IceCube;
    [SerializeField] private GameObject prefab_IceCubeWithKey;
    [SerializeField] private Transform parent_IceBlocks;

    [Space(4)]
    [Header("Area Borders")]
    [SerializeField] private Vector2[] areaOneBorder;
    [SerializeField] private Vector2[] areaTwoBorder;
    [SerializeField] private Vector2[] areaThreeBorder;
    #endregion

    #region Managers
    private GameManager gameManager;
    private _SceneManager sceneManager;
    private UIManager uiManager;
    #endregion

    [Space(5)]
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    private float fadeSpeed = 0.05f;

    #region Player Settings
    private GameObject player;
    private PlayerScript playerScript;
    #endregion

    #region Dialogue Settings
    [SerializeField] private GameObject dialogueCanvas;
    private DialogueScript dialogueSystem;
    #region Lines
    private string[] lines_startOfLevel =
    {
        "Oof, it’s a bit chilly here.",
        "So I am going to cut it short.",
        "Use your Snake by pressing F to melt ice cubes.",
        "You will need to melt the one containing a key.",
        "Once you have the key you should unlock the gate up ahead.",
        "Best of luck!"
    };
    #endregion
    #endregion

    [Space(6)]
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audio_generalSource;

    [Space(3)]
    [Header("Sounds")]
    public AudioClip sound_pickupKey;
    public AudioClip sound_doorOpen;

    [Space(5)]
    [Header("Movement Sprites With Snake & Fire :3")]
    [SerializeField] private Sprite[] sprite_movement_withsnake_down;
    [SerializeField] private Sprite[] sprite_movement_withsnake_up;
    [SerializeField] private Sprite[] sprite_movement_withsnake_left;
    [SerializeField] private Sprite[] sprite_movement_withsnake_right;

    [Space(5)]
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI text_keybind;

    private void Awake()
    {
        // Get reference to manager classes
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (GameObject.FindWithTag("SceneManager") != null) { sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>(); }
        if (GameObject.FindWithTag("UIManager") != null) { uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>(); }

        // Get reference to dialogue stuffsies
        dialogueSystem = dialogueCanvas.transform.GetChild(0).GetComponent<DialogueScript>();
        dialogueCanvas.SetActive(false);

        // remove once startlevel coroutine is complete
        player = Instantiate(gameManager.playerPrefab);
        player.transform.position = new Vector2(-24f, -0.5f);
        playerScript = player.GetComponent<PlayerScript>();
        player.name = gameManager.playerPrefab.name;
        player.AddComponent<LevelFourPlayer>(); // add level four player component to player
        player.GetComponent<LevelFourPlayer>().manager_level = this; // set reference to this script

        text_keybind.enabled = false;

        // set player movement animation sprites
        playerScript.sprite_moveDown = sprite_movement_withsnake_down;
        playerScript.sprite_moveUp = sprite_movement_withsnake_up;
        playerScript.sprite_moveLeft = sprite_movement_withsnake_left;
        playerScript.sprite_moveRight = sprite_movement_withsnake_right;

        gameManager.acceptInput = false;
    }

    private void Start()
    {
        InstantiateIceCubes();

        // start level
        StartCoroutine(cr_StartLevel());
    }

    private void Update()
    {
        ShowBorders();
    }

    private IEnumerator cr_StartLevel()
    {
        yield return new WaitForSeconds(0.5f);
        // Fade into gameplay
        StartCoroutine(uiManager.FadeOut(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a <= 0.5f);
        // start player walk and wait until complete
        StartCoroutine(playerScript.Walk("right", new Vector2(-18, -0.5f), 1.8f));
        yield return new WaitUntil(() => player.transform.position.x >= -18);
        yield return new WaitForSeconds(0.5f);
        // Set player sprite facing down.
        playerScript.SetPlayerAnimationDirection("down");
        yield return new WaitForSeconds(0.5f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.StartDialogue(1, lines_startOfLevel);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        yield return new WaitForSeconds(0.5f);
        // Enable input.
        gameManager.acceptInput = true;
    }

    #region Ice Cubes
    private void InstantiateIceCubes()
    {
        SpawnIceCube(amountOfIceBlocks[0], areaOneBorder[0], areaOneBorder[1]);
        SpawnIceCube(amountOfIceBlocks[1], areaTwoBorder[0], areaTwoBorder[1]);
        SpawnIceCube(amountOfIceBlocks[2], areaThreeBorder[0], areaThreeBorder[1]);
    }

    /// <summary>
    /// Spawns ice cubes depending on constant "amount", uses area borders which need to be specified when called.
    /// Makes sure the ice cubes dont spawn on top of each other (or is at least supposed to...)
    /// </summary>
    private void SpawnIceCube(int amount, Vector2 min, Vector2 max)
    {
        // Get random number for key.
        int randKey = Random.Range(0, amount);
        GameObject[] go = new GameObject[amount]; // Instantiate gameobject array.
        Vector2 randVec = Vector2.zero; // Instantiate random vector.

        // Offset distance between ice cubes.
        float offset = 5;

        for (int i = 0; i < amount; i++)
        {
            // Generate random position within the border.
            randVec = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

            // Check whether ice cube has spawned within offset of other ice cubes.
            for (int p = 0 ; p < amount ; p++)
            {
                if (go[p] != null)
                {
                    if (!(randVec.x < go[p].transform.position.x + offset &&
                        randVec.x > go[p].transform.position.x - offset &&
                        randVec.y < go[p].transform.position.y + offset &&
                        randVec.y > go[p].transform.position.y - offset))
                    {
                    }
                    else
                    {
                        randVec = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
                        p--;
                    }
                }
                else 
                { 
                    break;
                }
            }
            // If not a ice cube with key.
            if (i != randKey) { 
                go[i] = Instantiate(prefab_IceCube);
                go[i].transform.name = prefab_IceCube.name;
                go[i].transform.parent = parent_IceBlocks.GetChild(0).GetComponent<Transform>();
            }
            // If ice cube with a key.
            else { 
                go[i] = Instantiate(prefab_IceCubeWithKey);
                go[i].transform.name = prefab_IceCubeWithKey.name;
                go[i].transform.parent = parent_IceBlocks.GetChild(1).GetComponent<Transform>();
            }
            // Set ice cube to position.
            go[i].transform.position = randVec;
        }
    }
    #endregion

    public void PlaySound(AudioClip clip)
    {
        audio_generalSource.clip = clip;
        audio_generalSource.Play();
    }

    public IEnumerator cr_FinishLevel()
    {
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        // Run level transition to next level.
        sceneManager.RunLevelTransition(5);
    }

    public void DisplayDoorTextKeybind(bool hasKey)
    {
        if (hasKey)
        {
            text_keybind.fontSize = 75;
            uiManager.ShowKeybindText(text_keybind, "Press ", " to use key!", gameManager.key_Interact);
        }
        else
        {
            text_keybind.fontSize = 50;
            uiManager.ShowKeybindText(text_keybind, "You need to find the key to open this door!", "", KeyCode.None);
        }
        
    }

    public void ShowKeybindText(string before, string after, KeyCode key)
    {
        text_keybind.fontSize = 75;
        uiManager.ShowKeybindText(text_keybind, before, after, key);
    }
    public void HideKeybindText() { uiManager.HideKeybindText(text_keybind); }

    #region Debug 
    #region Debug Variables
    private Vector2 areaOneTopRight;
    private Vector2 areaOneBottomLeft; 
    private Vector2 areaTwoTopRight;
    private Vector2 areaTwoBottomLeft; 
    private Vector2 areaThreeTopRight;
    private Vector2 areaThreeBottomLeft;
    #endregion

    private void ShowBorders()
    {
        float duration = 0f;

        CalculateBorder();

        if(areaOneBorder.Length != 0)
        {
            // Top left
            Debug.DrawLine(areaOneBorder[0], areaOneTopRight, Color.magenta, duration);
            // Top Right
            Debug.DrawLine(areaOneTopRight, areaOneBorder[1], Color.magenta, duration);
            // Bottom Left
            Debug.DrawLine(areaOneBorder[1], areaOneBottomLeft, Color.magenta, duration);
            // Bottom Right
            Debug.DrawLine(areaOneBottomLeft, areaOneBorder[0], Color.magenta, duration);
        }
        if(areaTwoBorder.Length != 0)
        {
            // Top left
            Debug.DrawLine(areaTwoBorder[0], areaTwoTopRight, Color.magenta, duration);
            // Top Right
            Debug.DrawLine(areaTwoTopRight, areaTwoBorder[1], Color.magenta, duration);
            // Bottom Left
            Debug.DrawLine(areaTwoBorder[1], areaTwoBottomLeft, Color.magenta, duration);
            // Bottom Right
            Debug.DrawLine(areaTwoBottomLeft, areaTwoBorder[0], Color.magenta, duration);
        }
        if(areaThreeBorder.Length != 0)
        {
            // Top left
            Debug.DrawLine(areaThreeBorder[0], areaThreeTopRight, Color.magenta, duration);
            // Top Right
            Debug.DrawLine(areaThreeTopRight, areaThreeBorder[1], Color.magenta, duration);
            // Bottom Left
            Debug.DrawLine(areaThreeBorder[1], areaThreeBottomLeft, Color.magenta, duration);
            // Bottom Right
            Debug.DrawLine(areaThreeBottomLeft, areaThreeBorder[0], Color.magenta, duration);
        }
    }


    private void CalculateBorder()
    {
        areaOneTopRight = new Vector2(areaOneBorder[1].x, areaOneBorder[0].y);
        areaOneBottomLeft = new Vector2(areaOneBorder[0].x, areaOneBorder[1].y);

        areaTwoTopRight = new Vector2(areaTwoBorder[1].x, areaTwoBorder[0].y);
        areaTwoBottomLeft = new Vector2(areaTwoBorder[0].x, areaTwoBorder[1].y);

        areaThreeTopRight = new Vector2(areaThreeBorder[1].x, areaThreeBorder[0].y);
        areaThreeBottomLeft = new Vector2(areaThreeBorder[0].x, areaThreeBorder[1].y);
    }
    #endregion
}
