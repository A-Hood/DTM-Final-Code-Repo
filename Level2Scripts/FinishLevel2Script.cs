using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FinishLevel2Script : MonoBehaviour
{ 
    private GameManager gameManager;
    private _SceneManager sceneManager;
    private UIManager uiManager;

    private PlayerScript player;

    [SerializeField] private Image fadeImage;

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(FinishLevelTwo());
        }
    }

    // On collision run this coroutine.
    private IEnumerator FinishLevelTwo()
    {
        // Disable movement input.
        gameManager.acceptInput = false;
        // Set level completed.
        gameManager.levelTwoCompleted = true;

        // Stop player movement animation.
        player.StopMovementAnimation();
        yield return new WaitForSeconds(0.5f);
        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, 0.9f, 0.005f));
        yield return new WaitUntil(() => fadeImage.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        // Set has player died in this level to false.
        gameManager.hasPlayerDiedInLevelTwo = false;
        // Run level transition to next level.
        sceneManager.RunLevelTransition(3);
    }
}
