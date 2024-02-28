using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearNumbers : MonoBehaviour
{
    [SerializeField]
    private Button clearButton;

    // Start is called before the first frame update
    void Start()
    {
        // add listener
        clearButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // 'clear' numbers
        foreach (GameObject number in NumberManager.Instance.kenoNumbers)
        {
            number.GetComponent<NumberSelect>().ResetSkin();
        }

        // update selectedKenoNumbers in NumberManager
        NumberManager.Instance.selectedKenoNumbers.Clear();
        NumberManager.Instance.selectedNumbers = 0;

        // update UI and hide LastNumberIndicator
        UIManager.Instance.UpdateMarked(0);
        SpinManager.Instance.lastNumberIndicator.Hide();
    }
}
