using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NumberCreation : MonoBehaviour
{
    public GameObject numberPrefab;
    public Canvas canvas;
    public GameObject lastDrawnNumberIndicatorPrefab; // Prefab for the last drawn number indicator
    private Vector3 location = new Vector3(-136, 110, 0);
    private Button kenoButton;

    void Start()
    {
        // Initialize number counter
        int kenoNum = 1;

        // Create keno numbers
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                // Instantiate keno number and name it
                GameObject kenoNumber = Instantiate(numberPrefab, location, Quaternion.identity);
                kenoNumber.name = "Keno number " + kenoNum;
                kenoNumber.transform.SetParent(canvas.transform, false);
                kenoButton = kenoNumber.GetComponent<Button>(); 

                TextMeshProUGUI textComponent = kenoNumber.GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = kenoNum.ToString();

                NumberSelect numberSelectScript = kenoNumber.GetComponent<NumberSelect>();
                if (numberSelectScript != null)
                {
                    numberSelectScript.kenoNumberValue = kenoNum;
                }
                else
                {
                    Debug.LogError("NumberSelect script not found on the instantiated kenoNumber GameObject.");
                }

                GameManager.Instance.kenoNumbers.Add(kenoNumber);
                location = location + new Vector3(32, 0, 0);
                kenoNum++;
            }

            location = location - new Vector3(320, 0, 0) + new Vector3(0, -32, 0);
        }

        CreateLastDrawnNumberIndicator();
    }

    private void CreateLastDrawnNumberIndicator()
    {
        if (lastDrawnNumberIndicatorPrefab != null)
        {
            GameObject indicator = Instantiate(lastDrawnNumberIndicatorPrefab, canvas.transform);
            indicator.transform.localPosition = new Vector3(0, 0, 0); // Adjust as needed
            indicator.GetComponent<RectTransform>().sizeDelta = new Vector2(26, 26);

            // Set the reference in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetLastDrawnNumberIndicator(indicator.GetComponent<LastDrawnNumberIndicator>());
            }
            else
            {
                Debug.LogError("GameManager instance not found.");
            }
        }
        else
        {
            Debug.LogError("LastDrawnNumberIndicator prefab not assigned.");
        }

        GameManager.Instance.lastDrawnNumberIndicator.Hide();
    }
}