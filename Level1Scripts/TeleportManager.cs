using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportManager : MonoBehaviour
{
    #region Variables
    [Header("Player")]
    private Transform player;
    [Header("Managers")]
    private GameObject gameManager;
    private UIManager uiManager;
    [Header("Misc")]
    [SerializeField] private Image fadeImage;

    private float fadeAmount = 0.9f;
    private float fadeSpeed = 0.005f;
    #endregion

    private void Awake()
    {
        // Get reference to managers.
        gameManager = GameObject.FindWithTag("GameManager");
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

        // Get reference to player.
        StartCoroutine(GetPlayerReference());
    }

    // Use this to get reference to player.
    private IEnumerator GetPlayerReference()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Player"));
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Use this coroutine to teleport the player from room to room.
    public IEnumerator TeleportPlayer(Transform pos)
    {
        // Set movement input to false.
        gameManager.GetComponent<GameManager>().acceptInput = false;

        // Stop player movement animation.
        GameObject.FindWithTag("Player").GetComponent<PlayerScript>().StopMovementAnimation();

        // Fade in fade image.
        StartCoroutine(uiManager.FadeIn(fadeImage, fadeAmount, fadeSpeed));
        yield return new WaitUntil(() => fadeImage.color.a >= 1);
        yield return new WaitForSeconds(0.2f);

        // Teleport player.
        player.transform.position = pos.transform.position;

        yield return new WaitForSeconds(0.2f);

        // Fade in fade image.
        StartCoroutine(uiManager.FadeOut(fadeImage, fadeAmount, fadeSpeed));
        yield return new WaitUntil(() => fadeImage.color.a <= 0);
        yield return new WaitForSeconds(0.2f);

        // Set movement input to true.
        gameManager.GetComponent<GameManager>().acceptInput = true;

        yield return null;
    }

}
