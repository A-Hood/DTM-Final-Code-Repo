using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreasInfo : MonoBehaviour
{
    #region Area Constants
    [SerializeField] private Vector2[] areaOneBorder;
    [SerializeField] private Vector2[] areaTwoBorder;
    [SerializeField] private Vector2[] areaThreeBorder;

    private Vector2 areaOneTopRight;
    private Vector2 areaTwoTopRight;
    private Vector2 areaThreeTopRight;
    private Vector2 areaOneBottomLeft;
    private Vector2 areaTwoBottomLeft;
    private Vector2 areaThreeBottomLeft;
    #endregion

    private void Update()
    {
        ShowBorders();
    }

    private void ShowBorders()
    {
        float duration = 0f;

        CalculateBorder();

        if (areaOneBorder.Length != 0)
        {
            // Top left
            Debug.DrawLine(areaOneBorder[0], areaOneTopRight, Color.magenta, duration);
            // Top Right
            Debug.DrawLine(areaOneTopRight, areaOneBorder[1], Color.magenta, duration);
            // Bottom Left
            Debug.DrawLine(areaOneBorder[1], areaOneBottomLeft, Color.magenta, duration);
            // Bottom Right
            Debug.DrawLine(areaOneBottomLeft, areaOneBorder[0], Color.magenta, duration);
        }
        if (areaTwoBorder.Length != 0)
        {
            // Top left
            Debug.DrawLine(areaTwoBorder[0], areaTwoTopRight, Color.magenta, duration);
            // Top Right
            Debug.DrawLine(areaTwoTopRight, areaTwoBorder[1], Color.magenta, duration);
            // Bottom Left
            Debug.DrawLine(areaTwoBorder[1], areaTwoBottomLeft, Color.magenta, duration);
            // Bottom Right
            Debug.DrawLine(areaTwoBottomLeft, areaTwoBorder[0], Color.magenta, duration);
        }
        if (areaThreeBorder.Length != 0)
        {
            // Top left
            Debug.DrawLine(areaThreeBorder[0], areaThreeTopRight, Color.magenta, duration);
            // Top Right
            Debug.DrawLine(areaThreeTopRight, areaThreeBorder[1], Color.magenta, duration);
            // Bottom Left
            Debug.DrawLine(areaThreeBorder[1], areaThreeBottomLeft, Color.magenta, duration);
            // Bottom Right
            Debug.DrawLine(areaThreeBottomLeft, areaThreeBorder[0], Color.magenta, duration);
        }
    }

    private void CalculateBorder()
    {
        areaOneTopRight = new Vector2(areaOneBorder[1].x, areaOneBorder[0].y);
        areaOneBottomLeft = new Vector2(areaOneBorder[0].x, areaOneBorder[1].y);

        areaTwoTopRight = new Vector2(areaTwoBorder[1].x, areaTwoBorder[0].y);
        areaTwoBottomLeft = new Vector2(areaTwoBorder[0].x, areaTwoBorder[1].y);

        areaThreeTopRight = new Vector2(areaThreeBorder[1].x, areaThreeBorder[0].y);
        areaThreeBottomLeft = new Vector2(areaThreeBorder[0].x, areaThreeBorder[1].y);
    }
}
