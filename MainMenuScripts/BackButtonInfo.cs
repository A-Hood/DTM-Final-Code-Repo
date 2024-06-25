using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonInfo : MonoBehaviour
{
    [Header("Put Screen That You Want To Be Hid Here!")]
    [Space(2)]
    public CanvasGroup toBeHid;
    [Space(5)]
    [Header("Put Screen That You Want To Be Unhid Here!")]
    [Space(2)]
    public CanvasGroup toBeUnhid;

    public void BackButton()
    {
        // Hide screen.
        toBeHid.alpha = 0;
        toBeHid.interactable = false;

        // Unhide screen.
        toBeUnhid.alpha = 1;
        toBeUnhid.interactable = true;
    }
}
