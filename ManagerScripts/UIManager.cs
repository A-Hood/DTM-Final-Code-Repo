using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Use this to fade to black (or any colour).
    public IEnumerator FadeIn(Image image, float amount, float speed)
    {
        while (image.color.a < 1)
        {
            float step = amount * Time.deltaTime;

            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + step);

            yield return new WaitForSeconds(speed);
        }
    }

    // Use this to fade back to gameplay (or whatever else is behind it).
    public IEnumerator FadeOut(Image image, float amount, float speed)
    {
        while (image.color.a > 0)
        {
            float step = amount * Time.deltaTime;

            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - step);

            yield return new WaitForSeconds(speed);
        }
    }

    // Use this to set text in the UI.
    public void ShowKeybindText(TextMeshProUGUI text, string textBefore, string textAfter, KeyCode key)
    {
        if (key != KeyCode.None)
        {
            // Set text
            text.text = textBefore + key + textAfter;
        }
        else
        {
            // Set text
            text.text = textBefore + textAfter;
        }
        // Enable text
        text.enabled = true;
    }
    // Use this to hide text in the UI.
    public void HideKeybindText(TextMeshProUGUI text)
    {
        // Set text
        text.text = string.Empty;
        // Enable text
        text.enabled = false;
    }
}
