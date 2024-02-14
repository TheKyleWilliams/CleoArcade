using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastDrawnNumberIndicator : MonoBehaviour
{
    // singleton instance
    private static LastDrawnNumberIndicator instance;

    // public accessor for instance
    public static LastDrawnNumberIndicator Instance 
    {
        get { return instance; }
        private set { instance = value; }
    }

    [SerializeField]
    private GameObject lastDrawnNumberIndicatorPrefab;
    public LastDrawnNumberIndicator lastDrawnNumberIndicator;
    private GameObject lastDrawnIndicatorInstance; 

    [SerializeField]
    private Canvas canvas;

    void Awake()
    {
        // ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        CreateLastDrawnNumberIndicator();
    }

    // Hide the indicator
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Show the indicator at a specified position
    public void ShowAtPosition(Vector2 anchoredPosition)
    {
        // gameObject.SetActive(true);
        // RectTransform rectTransform = GetComponent<RectTransform>();
        // if (rectTransform != null)
        // {
        //     rectTransform.anchoredPosition = anchoredPosition;
        // }
    }

    private void CreateLastDrawnNumberIndicator()
    {
        // if (lastDrawnNumberIndicatorPrefab != null)
        // {
        //     GameObject indicator = Instantiate(lastDrawnNumberIndicatorPrefab, canvas.transform);
        //     indicator.transform.localPosition = new Vector3(0, 0, 0); // Adjust as needed
        //     indicator.GetComponent<RectTransform>().sizeDelta = new Vector2(26, 26);
        //     lastDrawnNumberIndicator = indicator.GetComponent<LastDrawnNumberIndicator>();
        // }
        // else
        // {
        //     Debug.LogError("LastDrawnNumberIndicator prefab not assigned.");
        // }

        // Hide();
    }
}