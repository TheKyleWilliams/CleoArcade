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

    // Skins as public fields
    public Sprite normalSkin;
    public Sprite selectedSkin;
    public Sprite hitSkin;
    public Sprite selectedAndHitSkin;

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
        if (GameManager.Instance.inGame)
        {
            return;
        }

        if (!selected)
        {
            if (GameManager.Instance.selectedNumbers < 10) 
            {
                GameManager.Instance.AddSelectedNumber(kenoNumberValue);
                selected = !selected;
                UpdateSkin();
            }
        }
        else
        {
            GameManager.Instance.RemoveSelectedNumber(kenoNumberValue);
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

        // Additional logic for hit and selectedAndHit skins can be added here if needed
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