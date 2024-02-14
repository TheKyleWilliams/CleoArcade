using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastNumberIndicator : MonoBehaviour
{
    void Start()
    {
        // make sure indicator shows ontop of keno numbers
        gameObject.transform.SetAsLastSibling();
        Hide();
    }


    public void MoveTo(Vector2 position)
    {
        Show();

        // move to desired destination
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
}
