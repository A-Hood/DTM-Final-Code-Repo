using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelOneManager : MonoBehaviour
{
    #region Variables
    private GameObject player;
    private GameManager gameManager;
    private UIManager uiManager;

    private CanvasGroup userTextCG;

    // Dialogue game objects.
    [Header("Dialogue Settings")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private GameObject dialogueSystem;

    [Space(5)]
    [Header("Colliders")]
    [SerializeField] private GameObject bridgeBlockCollider;
    [SerializeField] private GameObject[] waterBlockCollider;

    private GameObject a_collidedWithWood;

    [Space(5)]
    [Header("Sprites")]
    [SerializeField] private Sprite[] bridgeBehindSprites;
    [SerializeField] private Sprite[] bridgeInfrontSprites;
    [SerializeField] private SpriteRenderer bridgeBehindSpriteRenderer;
    [SerializeField] private SpriteRenderer bridgeInfrontSpriteRenderer;

    [Space(5)]
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    private float fadeSpeed = 0.005f;

    private string[] startingLines = {

        "Hello, Quetzalcoatl.",
        "I am your Narrator and Guide on your journey, Xolotl.",
        "I heard you are looking for the Jade Bones, am I correct?",
        "Yes, yes, the Jade Bones are guarded by the God of Death: Mictlantecuhtli.",
        "To meet with him you must make your Descent to Mictlan.",
        "You will need to pass Nine trials to get there.",
        "I will be here to guide you through the different trials you will face.",
        "But that is enough for our introduction.",
        "To begin your journey, speak with the Vermillion Dog straight down this path."

    };
    private string[] lines_vermillionDog =
    {
        // first interaction
        "Hi there, I am the Vermillion Dog.",
        "My duty is to stand guard of any intruders who might threaten this cave.",
        "I hear you are looking to make your Descent to Mictlan, is that right?",
        "Well that is awesome, one problem though...",
        "The bridge that used to be there has broken over time.",
        "If you look around you might be able to find some planks of wood to use to repair the bridge.",
        "Go on and look!",
        // after bridge is completed
        "There we go, the bridge is finished!",
        "Anyway get going.",
        "I hope you are able to find what you are looking for!",
        "Byeeeeee!!!"
    };

    // Timer
    private float timeTook = 0f;

    [Space(3)]
    [Header("Sounds")]
    public AudioSource audio_generalSource;

    public AudioClip soundclip_pickup;
    public AudioClip soundclip_deposit;
    public AudioClip soundclip_complete;

    [Space(5)]
    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI text_keybind;
    [SerializeField] private TextMeshProUGUI text_heldWood;
    [SerializeField] private TextMeshProUGUI text_depositedWood;
    #endregion

    private void Awake()
    {
        // Get reference to managers.
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

        // Instantiate player and place it at starting position.
        player = Instantiate(gameManager.GetComponent<GameManager>().playerPrefab);
        player.transform.position = new Vector2(-21, 0.5f);

        // Add camera follow script once player has been positioned correctly
        GameObject.FindWithTag("CameraHolder").AddComponent<CameraFollowAllAxis>();

        userTextCG = GameObject.FindWithTag("UICanvas").transform.GetChild(1).gameObject.GetComponent<CanvasGroup>();

        bridgeBehindSpriteRenderer.sprite = bridgeBehindSprites[0];
        bridgeInfrontSpriteRenderer.sprite = bridgeInfrontSprites[0];

        text_keybind.enabled = false;
        SetHeldWoodText();
        SetDepositedWoodText();

        userTextCG.alpha = 0f;
        // Disable user movement.
        gameManager.GetComponent<GameManager>().acceptInput = false;


    }

    private void Start()
    {
        // Check is "LevelOnePlayer" component already exists on player.
        if (player.GetComponent<LevelOnePlayer>() == null)
        {
            player.AddComponent<LevelOnePlayer>();
        }

        StartCoroutine(cr_StartLevel());
    }

    private void Update()
    {
        // If accepting input, continue timer.
        if (gameManager.GetComponent<GameManager>().acceptInput)
        {
            timeTook += Time.deltaTime;
        }

        // If UI text is hidden and we are accepting inputs, unhide UI text.
        if (userTextCG.alpha == 0f && gameManager.GetComponent<GameManager>().acceptInput)
        {
            userTextCG.alpha = 1f;
        }

        // Debug.Log(timeTook);
    }

    private IEnumerator cr_StartLevel()
    {
        yield return new WaitForSeconds(0.5f);
        // Start player walk.
        StartCoroutine(player.GetComponent<PlayerScript>().Walk("right", new Vector2(-16, 0.5f), 0.9f));
        // Start fade into gameplay.
        StartCoroutine(uiManager.FadeOut(fadeImage, 0.5f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a <= 0.5f);
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => new Vector2(player.transform.position.x, player.transform.position.y) == new Vector2(-16, 0.5f));
        // Stop movement animation and reset animation direction to face camera
        player.GetComponent<PlayerScript>().StopMovementAnimation();
        player.GetComponent<PlayerScript>().SetPlayerAnimationDirection("down");
        yield return new WaitForSeconds(2f);
        // Start dialogue.
        dialogueCanvas.SetActive(true);
        dialogueSystem.GetComponent<DialogueScript>().StartDialogue(1, startingLines);
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        yield return new WaitForSeconds(0.5f);
        // Activate movement input.
        gameManager.GetComponent<GameManager>().acceptInput = true;
    }

    public IEnumerator cr_TalkToVermillionDog()
    {
        // disable movement input
        gameManager.acceptInput = false;
        yield return new WaitForEndOfFrame();
        player.GetComponent<PlayerScript>().StopMovementSounds();
        player.GetComponent<PlayerScript>().StopMovementAnimation();
        yield return new WaitForSeconds(0.2f);
        // start dialogue
        dialogueCanvas.SetActive(true);
        dialogueSystem.GetComponent<DialogueScript>().StartDialogue(3, new string[] {

            lines_vermillionDog[0],
            lines_vermillionDog[1],
            lines_vermillionDog[2],
            lines_vermillionDog[3],
            lines_vermillionDog[4],
            lines_vermillionDog[5],
            lines_vermillionDog[6]

        });
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        yield return new WaitForSeconds(0.2f);
        // re-enable movement input
        gameManager.acceptInput = true;
    }

    public IEnumerator cr_BridgeCompletedDialogue()
    {
        // disable movement input
        gameManager.acceptInput = false;
        // stop movement animation 
        player.GetComponent<PlayerScript>().StopMovementAnimation();
        yield return new WaitForSeconds(0.5f);
        // set player to face camera
        player.GetComponent<PlayerScript>().SetPlayerAnimationDirection("down");
        yield return new WaitForSeconds(0.5f);
        // start dialogue
        dialogueCanvas.SetActive(true);
        dialogueSystem.GetComponent<DialogueScript>().StartDialogue(3, new string[] {

            lines_vermillionDog[7],
            lines_vermillionDog[8],
            lines_vermillionDog[9],
            lines_vermillionDog[10]

        });
        yield return new WaitUntil(() => !dialogueCanvas.activeInHierarchy);
        yield return new WaitForSeconds(0.2f);
        // re-enable movement input
        gameManager.acceptInput = true;
    }

    // For playing sound.
    public void PlaySound(AudioClip clip)
    {
        audio_generalSource.PlayOneShot(clip);
    }

    // To destroy the bridge collider.
    public void DestroyBridgeBlockCollider()
    {
        Destroy(bridgeBlockCollider);
        waterBlockCollider[0].SetActive(true);
        waterBlockCollider[1].SetActive(true);
    }
    // Set the bridge sprite.
    public void SetBridgeSprite()
    {
        bridgeBehindSpriteRenderer.sprite = bridgeBehindSprites[gameManager.GetComponent<GameManager>().bridgeStoredWood];
        bridgeInfrontSpriteRenderer.sprite = bridgeInfrontSprites[gameManager.GetComponent<GameManager>().bridgeStoredWood];
    }
    // Set UI text.
    public void SetHeldWoodText()
    {
        text_heldWood.text = "Wood Held: " + gameManager.heldWood.ToString();
    }
    // Set UI text.
    public void SetDepositedWoodText()
    {
        text_depositedWood.text = "Bridge Completed: " + gameManager.bridgeStoredWood.ToString() + "/10";
    }
    // Set UI text custom.
    public void SetKeybindText(string before, string after, KeyCode keybind)
    {
        uiManager.ShowKeybindText(text_keybind, before, after, keybind);
    }
    // Hide UI text.
    public void HideKeybindText()
    {
        uiManager.HideKeybindText(text_keybind);
    }
}
