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

    void Awake()
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

                // set button text to reflect keno number
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

                NumberManager.Instance.kenoNumbers.Add(kenoNumber);
                location = location + new Vector3(32, 0, 0);
                kenoNum++;
            }

            location = location - new Vector3(320, 0, 0) + new Vector3(0, -32, 0);
        }
    }
}