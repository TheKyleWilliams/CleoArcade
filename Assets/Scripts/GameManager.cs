using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Numerics;

public class GameManager : MonoBehaviour
{
    // singleton instance
    private static GameManager instance;

    // public accessor for instance
    public static GameManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    public int balance;     

    // Payout table based on the number of CATCHes for each PICK
    private int[,] payoutTable = new int[,] {
        // CATCH:     0,  1,  2,   3,   4,   5,   6,   7,   8,   9,  10
        /* PICK 3 */ {0,  0,  3,  19,   0,   0,   0,   0,   0,   0,   0},
        /* PICK 4 */ {0,  0,  1,   5,  45,   0,   0,   0,   0,   0,   0},
        /* PICK 5 */ {0,  0,  0,   3,  24, 240,   0,   0,   0,   0,   0},
        /* PICK 6 */ {0,  0,  0,   2,   5,  42, 410,   0,   0,   0,   0},
        /* PICK 7 */ {0,  0,  0,   1,   3,   7, 118, 500,   0,   0,   0},
        /* PICK 8 */ {0,  0,  0,   0,   3,  10,  65, 250, 1000,  0,   0},
        /* PICK 9 */ {0,  0,  0,   0,   2,   5,  12, 108,  200, 1000, 0},
        /* PICK 10*/ {0,  0,  0,   0,   1,   3,   5,  35,  206, 1000, 2000}
    };

    void Awake()
    {
        // make sure there's only one instance of a game manager
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // initialize player balance
        balance = PlayerPrefs.GetInt("balance", 10);
        UIManager.Instance.UpdateCredit(balance);

        // Check for new day and reward player
        CheckAndRewardForNewDay();
    }

    public int CalculatePayout(HashSet<int> drawnNumbers)
    {
        SpinManager.Instance.matches = 0;

        // calculate number of 'hits'
        foreach (int number in NumberManager.Instance.selectedKenoNumbers)
        {
            if (drawnNumbers.Contains(number))
            {
                SpinManager.Instance.matches++;
            }
        }

        // line pickIndex up with payoutTable 
        int pickIndex = NumberManager.Instance.selectedNumbers - 3;
        int catchIndex = SpinManager.Instance.matches;

        // calculate payout from payoutTable
        int roundPayout;
        if (NumberManager.Instance.selectedNumbers >= 3 && NumberManager.Instance.selectedNumbers <=10 && catchIndex >= 0 && catchIndex < payoutTable.GetLength(1))
        {
            roundPayout = payoutTable[pickIndex, catchIndex];
        }
        else
        {
            roundPayout = 0;
        }

        // double payout if in bonus round
        if (SpinManager.Instance.isInBonusRound)
        {
            roundPayout *= 2;
        }

        // set user display payout
        SpinManager.Instance.payout = roundPayout;

        // update balance to reflect payout, save, and update ui
        balance += roundPayout;
        PlayerPrefs.SetInt("balance", balance);
        PlayerPrefs.Save();
        UIManager.Instance.UpdateCredit(balance);

        return roundPayout;
    }

    private void CheckAndRewardForNewDay()
    {
        // Get today's date
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");

        // Get the last recorded day
        string lastDayPlayed = PlayerPrefs.GetString("LastDayPlayed", "");

        // Check if today is a new day compared to the last recorded day
        if (today != lastDayPlayed)
        {
            // Update the last played day to today
            PlayerPrefs.SetString("LastDayPlayed", today);

            // Add balance
            balance += 10;

            // Save the updated balance and update UI
            PlayerPrefs.SetInt("balance", balance);
            PlayerPrefs.Save();
            UIManager.Instance.UpdateCredit(balance);
        }
    }

}