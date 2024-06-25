using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script handles everything in the team splash screen scene.
/// </summary>
public class IntroHandler : MonoBehaviour
{
    #region Variables
    [Header("Animation")]
    public Animator anim_TeamIcon;
    public Animator anim_GameLogo;

    [Header("Team Name Text")]
    public TextMeshProUGUI _teamLogoText;
    public string _teamText = "Kitty\nCrew";
    public float textSpeed = 0.15f;

    [Header("Team Splash Screen Canvas")]
    public CanvasGroup teamLogoCanvasGroup;
    public Canvas teamLogoCanvas;

    [Space(5)]
    [Header("Fade Settings")]
    [SerializeField] private Image fade_image;
    private float fade_amount = 0.7f;
    private float fade_speed = 0.005f;
    [Space(5)]

    private bool teamSplashScreenComplete;

    [Header("Game Manager")]
    // Create new game manager object.
    private GameManager gameManager;
    private UIManager uiManager;

    [Header("Sounds")]
    public AudioClip teamIconSwooshSound;
    public AudioClip catMeowSound;
    public AudioClip[] keyboardSounds;
    public AudioSource soundsSource;

    private KeyCode skipKey = KeyCode.Space;
    #endregion

    private void Awake()
    {
        // Get reference to managers.
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
    }

    private void Start()
    {
        StartCoroutine(TeamSplashScreenStart());
    }

    private void Update()
    {
        if (Input.GetKeyDown(skipKey))
        {
            StopAllCoroutines();
            StartCoroutine(EndIntro());
        }
    }

    IEnumerator TeamSplashScreenStart()
    {
        // Empty current string.
        _teamLogoText.text = string.Empty;

        // Initialise intro played bool to be true so it does not play the intro again upon reloading main menu.
        gameManager.GetComponent<GameManager>().introPlayed = true;

        yield return new WaitForSeconds(1f);

        // Play icon swipe animation
        soundsSource.clip = teamIconSwooshSound;
        soundsSource.Play();
        anim_TeamIcon.Play("IconSwipe");

        yield return new WaitForSeconds(1.5f);

        // Output team name letter by letter wish textSpeed intervals.
        foreach (char c in _teamText.ToCharArray())
        {
            // Generate random keyboard click sfx.
            int randInt = Random.Range(0, 6);

            soundsSource.clip = keyboardSounds[randInt];
            soundsSource.Play();

            // Output new letter.
            _teamLogoText.text += c;

            // Wait.
            yield return new WaitForSeconds(textSpeed);
        }

        // Wait for 1 seconds.
        yield return new WaitForSeconds(1.5f);

        // Play meow sound
        soundsSource.clip = catMeowSound;
        soundsSource.Play();

        // Wait for 3 seconds.
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EndIntro());
    }

    private IEnumerator EndIntro()
    {
        // Fade canvas out.
        StartCoroutine(uiManager.FadeIn(fade_image, 0.8f, 0.005f));
        yield return new WaitUntil(() => fade_image.color.a >= 1);
        yield return new WaitForSeconds(0.2f);
        // Go to main menu scene
        GameObject.Find("SceneManager").GetComponent<_SceneManager>().func_SceneSwitch("MainMenu");
    }
}