using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Start Menu Settings")]
    public Canvas startMenuCanvas;
    public CanvasGroup startMenuCanvasGroup;
    public Image _gameLogo;
    public Animator anim_GameLogo;
    public TextMeshProUGUI pressAnyKeyPrompt;

    [Header("Main Menu Settings")]
    public Animator anim_MainMenuButtons;
    public CanvasGroup levelScreen;
    public CanvasGroup mainMenuScreen;

    [Space(5)]
    [Header("Fade Image")]
    [SerializeField] private Image fade_image;
    private float fade_amount = 0.5f;
    private float fade_speed = 0.005f;
    private float fadeSpeed = 0.02f;
    [Space(5)]

    private GameManager gameManager;
    private _SceneManager sceneManager;
    private UIManager uiManager;

    private bool mainMenuAccessed;

    [SerializeField] private SpriteRenderer mainMenuBackgroundSprite;

    [Space(5)]
    [Header("Audio")]
    [SerializeField] private AudioSource main_menu_audioSource;
    [SerializeField] private AudioClip main_menu_music;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

        // reinitialise vars incase player left before end
        gameManager.isPlayerDead = false;
        gameManager.hasPlayerDiedInLevelTwo = false;
        gameManager.hasPlayerDiedInLevelThree = false;
        gameManager.hasPlayerDiedInLevelFive = false;
        gameManager.hasPlayerDiedInLevelSix = false;
        gameManager.hasPlayerDiedInLevelSeven = false;
        gameManager.hasPlayerDiedInLevelEight = false;
        gameManager.heldWood = 0;
        gameManager.bridgeStoredWood = 0;

        sceneManager._hasFinishedGame = false;

        // Start game logo before first frame with alpha of 0.
        _gameLogo.color = new Color(_gameLogo.color.r, _gameLogo.color.g, _gameLogo.color.b, 0);

        // Start with level screen invisible and !interactable
        levelScreen.alpha = 0;
        levelScreen.interactable = false;

        mainMenuScreen.alpha = 1;
        mainMenuScreen.interactable = false;

        startMenuCanvasGroup.alpha = 1f;
        fade_image.color = new Color(0, 0, 0, 1);
    }

    private void Start()
    {
        if (!gameManager.GetComponent<GameManager>().startScreenPlayed)
            StartCoroutine(StartMenu());
        else
        {
            StartCoroutine(StartMainMenuAfterAlreadyAccessed());
        }

        main_menu_audioSource.Play();
        StartCoroutine(cr_FadeInMusic());
    }

    private void Update()
    {
        // Checks for: left mouse button being pressed, whether we have already accessed the main menu, start screen has been played.
        if (Input.anyKey && !mainMenuAccessed && gameManager.GetComponent<GameManager>().startScreenPlayed)
        {
            mainMenuAccessed = true;
            StartCoroutine(StartMainMenuFromStartScreen());
        }
    }

    // START MENU UPON LOADING GAME AFTER INTRO
    IEnumerator StartMenu()
    {
        yield return new WaitForSeconds(1f);

        // Activate start menu
        startMenuCanvas.gameObject.SetActive(true);

        // Fade out fade image
        StartCoroutine(uiManager.FadeOut(fade_image, fade_amount, fade_speed));
        yield return new WaitForSeconds(0.5f);
        
        // Play logo animation before fading in logo.
        anim_GameLogo.Play("GameLogoRotate");

        // Fade in logo.
        StartCoroutine(uiManager.FadeIn(_gameLogo, fade_amount, fade_speed));

        yield return new WaitUntil(() => _gameLogo.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        
        // Set start screen played bool to true
        gameManager.GetComponent<GameManager>().startScreenPlayed = true;

        pressAnyKeyPrompt.gameObject.SetActive(true);
    }

    // USE THIS IS MAIN MENU HAS NOT YET BEEN ACCESSED THIS SESSION
    IEnumerator StartMainMenuFromStartScreen()
    {
        yield return new WaitForSeconds(1);

        pressAnyKeyPrompt.gameObject.SetActive(false);

        // Reset rotation of game logo,
        // Start the transition from main game logo size and position to the right of the menu,
        // Start animation for main menu buttons to slide in.
        if (_gameLogo.GetComponent<Animator>() != null)
        {
            anim_GameLogo.Play("GameLogoResetPosition");
            anim_GameLogo.Play("GameLogoOnClickAnimation");
            anim_MainMenuButtons.Play("MainMenuButtonsAppear");
        }

        yield return new WaitForSeconds(1.5f);

        mainMenuScreen.interactable = true;
    }

    // USE THIS IF MAIN MENU HAS ALREADY BEEN ACCESSED IN THIS SESSION
    IEnumerator StartMainMenuAfterAlreadyAccessed()
    {
        Destroy(_gameLogo.GetComponent<Animator>());

        StartCoroutine(uiManager.FadeOut(fade_image, 0.6f, 0.005f));
        // Fade in start menu canvas.
        while (startMenuCanvasGroup.alpha < 1f)
        {
            float currentAlpha = mainMenuBackgroundSprite.color.a;
            startMenuCanvasGroup.alpha += fadeSpeed;

            yield return new WaitForSeconds(fadeSpeed);
        }

        _gameLogo.rectTransform.anchoredPosition = new Vector3(350, 0, 0);
        _gameLogo.rectTransform.sizeDelta = new Vector2(1044.899f, 500f);

        yield return new WaitUntil(() => fade_image.color.a <= 0);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(uiManager.FadeIn(_gameLogo, 0.5f, 0.005f));

        yield return new WaitUntil(() => _gameLogo.color.a >= 1);
        anim_MainMenuButtons.speed = 1.5f;
        anim_MainMenuButtons.Play("MainMenuButtonsAppear");


        mainMenuScreen.interactable = true;
    }

    public IEnumerator cr_FadeInMusic()
    {
        while (main_menu_audioSource.volume < 0.5f)
        {
            float step = 0.5f * Time.deltaTime;
            float current_volume = main_menu_audioSource.volume;
            main_menu_audioSource.volume = current_volume + step;

            yield return new WaitForSeconds(0.005f);
        }
    }
    public IEnumerator cr_FadeOutMusic()
    {
        while (main_menu_audioSource.volume > 0f)
        {
            float step = 0.5f * Time.deltaTime;
            float current_volume = main_menu_audioSource.volume;
            main_menu_audioSource.volume = current_volume - step;

            yield return new WaitForSeconds(0.005f);
        }
    }

    public void LevelScreenAppear()
    {
        levelScreen.alpha = 1;
        levelScreen.interactable = true;
    }
    public void MainMenuScreenDisappear()
    {
        mainMenuScreen.alpha = 0;
        mainMenuScreen.interactable = false;
    }

    public void EnterCredits()
    {
        mainMenuScreen.interactable = false;
        StartCoroutine(cr_EnterCredits());
    }

    private IEnumerator cr_EnterCredits()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(cr_FadeOutMusic());
        while (fade_image.color.a < 1f)
        {
            float currentAlpha = fade_image.color.a;
            fade_image.color = new Color(0, 0, 0, currentAlpha + fadeSpeed);

            yield return new WaitForSeconds(fadeSpeed);
        }
        yield return new WaitForSeconds(0.5f);

        sceneManager.GoToCredits(false);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
