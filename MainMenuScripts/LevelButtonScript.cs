using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class LevelButtonScript : MonoBehaviour
{
    public Button level1Button, level2Button, level3Button, level4Button, level5Button, level6Button, level7Button, level8Button, level9Button;
    public Image fade_image;
    private float fade_speed = 0.005f;
    public MainMenuHandler mainMenuHandler;

    private void Start()
    {
        level1Button.onClick.AddListener(() => GoToLevel(1));
        level2Button.onClick.AddListener(() => GoToLevel(2));
        level3Button.onClick.AddListener(() => GoToLevel(3));
        level4Button.onClick.AddListener(() => GoToLevel(4));
        level5Button.onClick.AddListener(() => GoToLevel(5));
        level6Button.onClick.AddListener(() => GoToLevel(6));
        level7Button.onClick.AddListener(() => GoToLevel(7));
        level8Button.onClick.AddListener(() => GoToLevel(8));
        level9Button.onClick.AddListener(() => GoToLevel(9));
    }

    public void GoToLevel(int level)
    {
        mainMenuHandler.levelScreen.interactable = false;
        StartCoroutine(cr_GoIntoLevel(level));
    }

    public IEnumerator cr_GoIntoLevel(int level)
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(mainMenuHandler.cr_FadeOutMusic());
        while (fade_image.color.a < 1f)
        {
            float currentAlpha = fade_image.color.a;
            fade_image.color = new Color(0, 0, 0, currentAlpha + fade_speed);

            yield return new WaitForSeconds(fade_speed);
        }
        yield return new WaitForSeconds(0.5f);

        GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>().RunLevelTransition(level);
    }
}
