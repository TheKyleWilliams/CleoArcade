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
        if (selected)
        {
            kenoNumberImage.sprite = selectedSkin;
        }
        else
        {
            kenoNumberImage.sprite = normalSkin;
        }
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