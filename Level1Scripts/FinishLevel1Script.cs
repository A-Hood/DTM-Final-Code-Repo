using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishLevel1Script : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject sceneManager;
    private UIManager uiManager;

    private PlayerScript player;

    [SerializeField] private Image fadeImage;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        sceneManager = GameObject.FindWithTag("SceneManager");
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

        player = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(FinishLevelOne());
        }
    }

    // On collision run this coroutine.
    private IEnumerator FinishLevelOne()
    {
        // Disable movement input.
        gameManager.acceptInput = false;
        // Stop player movement animation.
        player.StopMovementAnimation();
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.8f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        // Run level transition to next level.
        sceneManager.GetComponent<_SceneManager>().RunLevelTransition(2);
    }
}
