using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastDrawnNumberIndicator : MonoBehaviour
{
    // Hide the indicator
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Show the indicator at a specified position
    public void ShowAtPosition(Vector2 anchoredPosition)
    {
        gameObject.SetActive(true);
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = anchoredPosition;
        }
    }
}