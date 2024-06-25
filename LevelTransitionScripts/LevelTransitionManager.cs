using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LevelTransitionManager : MonoBehaviour
{
    private UIManager manager_UI;
    private _SceneManager manager_scene;

    [SerializeField] private GameObject go_renderImage;
    private RawImage ri_renderImage;
    private VideoPlayer vp_renderImage;

    [SerializeField] private VideoClip[] video_levelTransitionClips;

    [Header("Fade Settings")]
    [SerializeField] private Image fade_image;
    private float fade_amount = 0.8f;
    private float fade_speed = 0.005f;

    [Space(10)]
    [Header("TESTING")]
    public int testLevel;

    private void Start()
    {
        manager_UI = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        manager_scene = GameObject.FindWithTag("SceneManager").GetComponent<_SceneManager>();
        vp_renderImage = go_renderImage.GetComponent<VideoPlayer>();
        ri_renderImage = go_renderImage.GetComponent<RawImage>();

        StartCoroutine(cr_LevelTransition(manager_scene.GetLevelToTransitionTo()));
    }

    public IEnumerator cr_LevelTransition(int level)
    {
        Debug.Log(level);
        fade_image.color = new Color(0, 0, 0, 1);
        vp_renderImage.clip = video_levelTransitionClips[level-1];
        vp_renderImage.Play();
        yield return new WaitForSeconds(1);
        StartCoroutine(manager_UI.FadeOut(fade_image, fade_amount, fade_speed));
        yield return new WaitUntil(() => fade_image.color.a <= 0);
        // yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(4);
        StartCoroutine(manager_UI.FadeIn(fade_image, fade_amount, fade_speed));
        yield return new WaitUntil(() => fade_image.color.a >= 1);
        yield return new WaitForSeconds(0.5f);
        manager_scene.TransitionToLevel();
    }
}
