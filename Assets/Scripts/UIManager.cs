using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // actual Singleton instance
    private static UIManager instance;

    // public accessor for instance
    public static UIManager Instance 
    {
        get { return instance; }
        private set { instance = value; }
    }

    // public TextMeshProUGUI win, credit, hit, bonusGamesRemaining, totalBonusWin;

    [SerializeField]
    private TextMeshProUGUI marked;

    [SerializeField]
    private TextMeshProUGUI hit;

    [SerializeField]
    private TextMeshProUGUI win;
    [SerializeField]
    private TextMeshProUGUI bonusWin;
    [SerializeField]
    private TextMeshProUGUI bonusGamesRemaining;
    [SerializeField]
    private TextMeshProUGUI credit;

    public void Awake()
    {
        // ensure only one UIManager exists
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


    public void UpdateMarked(int numOfMarkedNumbers)
    {
        marked.text = "" + numOfMarkedNumbers;
    }

    public void UpdateHit(int numOfHitNumbers)
    {
        hit.text = "" + numOfHitNumbers;
    }

    public void UpdateWin(int payout)
    {
        win.text = "" + payout;
    }

    public void UpdateCredit(int balance)
    {
        credit.text = "$" + balance + ".00";
    }

    public void UpdateBonusGamesRemaining(int numOfBonusGamesLeft)
    {
        bonusGamesRemaining.text = "Games remaining: " + numOfBonusGamesLeft;
    }

    public void UpdateTotalBonusWin(int bonusPayout)
    {
        bonusWin.text = "Total win: " + bonusPayout;
    }

}
