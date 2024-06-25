using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the manager for the credits scene.
/// Everything inside of the credits scene is controlled in this script.
/// </summary>

public class CreditsManager : MonoBehaviour
{
    // Start on a black screen
    // Fade in game logo
    // Start scroll upwards

    // If we want to add anything inbetween this part then we can

    // Once scroll is completed:
    // Fade in "Thank you" text
    // Fade in game logo after
    // Fade in the "from" text and team logo at the same time
    // Fade out (already has a black background)
    // Scene switch to main menu

    #region Variables
    private UIManager manager_UI;
    private _SceneManager manager_scene;

    [Header("Credits Settings")]
    [SerializeField] private GameObject go_credits;
    [SerializeField] private float speed = 50f;

    [SerializeField] private Image fade_image;

    private GameObject go_finalCreditsParent;

    private Image image_gameLogoStart;

    private bool bool_scrollCompleted = false;

    [Space(5)]
    [Header("Audio")]
    [SerializeField] private AudioSource audio_source;
    [SerializeField] private AudioClip sound_music;
    #endregion

    private void Start()
    {
        // Get reference to managers.
        manager_UI = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        manager_scene = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();

        // Get reference to game logo that plays at the start of the credits.
        image_gameLogoStart = go_credits.transform.GetChild(1).gameObject.GetComponent<Image>();
        // Set alpha to 0
        image_gameLogoStart.color = new Color(1, 1, 1, 0);

        // Get reference to all the stuff that will play if the player has actually beaten the game.
        go_finalCreditsParent = go_credits.transform.GetChild(3).gameObject;
        go_finalCreditsParent.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = string.Empty;
        go_finalCreditsParent.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = string.Empty;
        go_finalCreditsParent.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        go_finalCreditsParent.transform.GetChild(3).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        // Start the credits coroutine.
        StartCoroutine(cr_Credits());
    }

    private IEnumerator cr_Credits()
    {
        yield return new WaitForSeconds(2);
        // Fade in the game logo.
        StartCoroutine(manager_UI.FadeIn(image_gameLogoStart, 0.45f, 0.005f));
        yield return new WaitUntil(() => image_gameLogoStart.color.a >= 1); // Wait until alpha is 1
        yield return new WaitForSeconds(1f);
        // Start the music.
        audio_source.Play();
        // Fade in the music.
        StartCoroutine(cr_FadeInMusic());
        yield return new WaitForSeconds(1f);
        // Start scroll
        StartCoroutine(cr_Scroll(-1620, go_credits, speed)); // I have no clue why it is -1620 when in engine it is -2160 like what

        // IF WE WANT ANYTHING IN BETWEEN IT GOES HERE
        // - we did not
        // END

        // Wait until the scroll has completed.
        yield return new WaitUntil(() => bool_scrollCompleted);
        // If the player finished the game before transitioning to the credits.
        if (manager_scene._hasFinishedGame)
        {
            // Reset the variable so it doesn't play when going to the credits from the main menu.
            manager_scene._hasFinishedGame = false;
            yield return new WaitForSeconds(1);
            // Output "Thank you for playing our game"
            StartCoroutine(cr_OutputTextLBL(go_finalCreditsParent.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>(), "Thank you for playing our game", 0.05f));
            yield return new WaitUntil(() => go_finalCreditsParent.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text == "Thank you for playing our game");
            yield return new WaitForSeconds(1);
            // Fade in game logo
            StartCoroutine(manager_UI.FadeIn(go_finalCreditsParent.transform.GetChild(0).gameObject.GetComponent<Image>(), 0.45f, 0.005f));
            yield return new WaitUntil(() => go_finalCreditsParent.transform.GetChild(0).gameObject.GetComponent<Image>().color.a >= 1); // Wait until alpha is 1
            yield return new WaitForSeconds(1);
            // Output "from all of us here at Kitty Crew"
            StartCoroutine(cr_OutputTextLBL(go_finalCreditsParent.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>(), "from all of us at Kitty Crew", 0.05f));
            yield return new WaitUntil(() => go_finalCreditsParent.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text == "from all of us at Kitty Crew");
            yield return new WaitForSeconds(1);
            // Team logo fade in
            StartCoroutine(manager_UI.FadeIn(go_finalCreditsParent.transform.GetChild(3).gameObject.GetComponent<Image>(), 0.5f, 0.005f));
            yield return new WaitUntil(() => go_finalCreditsParent.transform.GetChild(3).gameObject.GetComponent<Image>().color.a >= 1); // Wait until alpha is 1
            yield return new WaitForSeconds(3);
            StartCoroutine(manager_UI.FadeIn(fade_image, 0.4f, 0.005f));
            yield return new WaitUntil(() => fade_image.color.a >= 1); // Wait until alpha is 1
        }
        // Fade out the music.
        StartCoroutine(cr_FadeOutMusic());
        // Wait until music is fully faded out.
        yield return new WaitUntil(() => audio_source.volume <= 0);
        yield return new WaitForSeconds(2f);
        // Transition to main menu.
        manager_scene.func_SceneSwitch("MainMenu");
    }

    // Use this to output text letter by letter.
    private IEnumerator cr_OutputTextLBL(TextMeshProUGUI textField, string text, float speed)
    {
        foreach (char c in text)
        {
            textField.text += c;
            yield return new WaitForSeconds(speed);
        }
    }

    // Use this to move a gameobject in the UI due to rect-transform. (Only for credits gameobject)
    private IEnumerator cr_Scroll(float y, GameObject go, float speed)
    {
        while (go.GetComponent<RectTransform>().position.y != y)
        {
            float step = speed * Time.deltaTime;
            go.GetComponent<RectTransform>().position = Vector2.MoveTowards(go.transform.position, new Vector2(go.transform.position.x, y), step);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Scroll completed");
        bool_scrollCompleted = true;
    }

    // Use this to fade in the music.
    public IEnumerator cr_FadeInMusic()
    {
        while (audio_source.volume < 0.9f)
        {
            float step = 0.4f * Time.deltaTime;
            float current_volume = audio_source.volume;
            audio_source.volume = current_volume + step;

            yield return new WaitForSeconds(0.005f);
        }
    }
    // Use this to fade out the music.
    public IEnumerator cr_FadeOutMusic()
    {
        while (audio_source.volume > 0f)
        {
            float step = 0.4f * Time.deltaTime;
            float current_volume = audio_source.volume;
            audio_source.volume = current_volume - step;

            yield return new WaitForSeconds(0.005f);
        }
    }
}
