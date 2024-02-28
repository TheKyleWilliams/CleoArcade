using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NumberSelect : MonoBehaviour
{
    private Button kenoNumber;
    private Image kenoNumberImage; // To change the sprite
    public int kenoNumberValue;

    [SerializeField]
    private Sprite normalSkin, selectedSkin, hitSkin, selectedAndHitSkin;

    private bool selected = false;

    // Start is called before the first frame update
    void Start()
    {
        // get button component
        kenoNumber = GetComponent<Button>();

        // get image component
        kenoNumberImage = GetComponent<Image>();

        // listener for button click
        kenoNumber.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // make sure not in the middle of game
        if (SpinManager.Instance.inGame)
        {
            return;
        }

        if (!selected)
        {
            if (NumberManager.Instance.selectedNumbers < 10) 
            {
                NumberManager.Instance.AddSelectedNumber(kenoNumberValue);
                selected = !selected;
                UpdateSkin();
            }
        }
        else
        {
            NumberManager.Instance.RemoveSelectedNumber(kenoNumberValue);
            selected = !selected;
            UpdateSkin();
        }
    }

    public void UpdateSkin()
    {
        // grab text component to change color
        TextMeshProUGUI text = kenoNumber.GetComponentInChildren<TextMeshProUGUI>();

        if (selected)
        {
            kenoNumberImage.sprite = selectedSkin;
            text.color = Color.yellow;
        }
        else
        {
            kenoNumberImage.sprite = normalSkin;
            text.color = Color.white;
        }
    }

    public void ResetSkin()
    {
        // reset text color and skin to default
        TextMeshProUGUI text = kenoNumber.GetComponentInChildren<TextMeshProUGUI>();
        text.color = Color.white;
        kenoNumberImage.sprite = normalSkin;

        selected = false;
    }


    // Method to be called from GameManager to mark as hit
    public void MarkAsHit()
    {
        kenoNumberImage.sprite = hitSkin;
        if (selected)
        {
            kenoNumberImage.sprite = selectedAndHitSkin;
        }
    }
}