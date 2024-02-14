using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Numerics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int balance;     
    public int matches;
    public int payout;
    private bool isInBonusRound = false;
    public bool inGame = false;
    private int freeSpinsLeft = 0;  
    int realBonusRoundWinnings = 0;
    public GameObject bonusPanel;
    public GameObject backgroundBonusPanel;

    [SerializeField]
    private LastNumberIndicator lastNumberIndicator;

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

        // panel manager
        bonusPanel.SetActive(false);
        backgroundBonusPanel.SetActive(false);

        // Check for new day and reward player
        CheckAndRewardForNewDay();

        // lastNumberIndicator.transform.SetAsLastSibling();
    }

    public void GameStart()
    {
        // check if in current game, if so disable start button
        if (inGame)
        {
            return;
        }

        // regular spin settings
        if (!isInBonusRound)
        {
            // hide bonus panels
            bonusPanel.SetActive(false);
            backgroundBonusPanel.SetActive(false);

            // ensure at least two numbers are selected and balance is sufficient 
            if (NumberManager.Instance.selectedNumbers < 3 || balance < 1)
            {
                Debug.Log("Not enough numbers selected or insufficient balance.");
                return;
            }

            // deduct balance, save, and update ui
            balance -= 1;
            PlayerPrefs.SetInt("balance", balance);
            PlayerPrefs.Save();
            UIManager.Instance.UpdateCredit(balance);


        }

        // hide last number indicator before game start
        // lastNumberIndicator.Hide();

        // start drawing
        StartCoroutine(DrawNumbersWithDelay());
    }

    public void ResetGame()
    {
        matches = 0;
        payout = 0;

        // update hit and win ui
        UIManager.Instance.UpdateHit(matches);
        UIManager.Instance.UpdateWin(payout);

        // clear 'drawn' numbers (visually)
        foreach (GameObject number in NumberManager.Instance.kenoNumbers)
        {
            number.GetComponent<NumberSelect>().UpdateSkin();
        }

        // hide lastNumberIndicator
        lastNumberIndicator.Hide();
    }

    private IEnumerator DrawNumbersWithDelay()
    {
        ResetGame();

        // initialize drawnNumbers
        HashSet<int> drawnNumbers = new HashSet<int>();

        // draw all 20 numbers
        while (drawnNumbers.Count < 20)
        {
            inGame = true;

            // number range to draw from
            int draw = Random.Range(1, 81);

            if (!drawnNumbers.Contains(draw))
            {
                // last drawn number logic
                if (drawnNumbers.Count == 19 && !isInBonusRound)
                {
                    // set last drawn number
                    GameObject lastDrawnKenoNumber = NumberManager.Instance.kenoNumbers[draw - 1];

                    // put lastDrawnNumberIndicator at last number position
                    RectTransform lastKenoRectTransform = lastDrawnKenoNumber.GetComponent<RectTransform>();
                    if (lastKenoRectTransform != null)
                    {
                        UnityEngine.Vector2 lastNumberAnchoredPosition = lastKenoRectTransform.anchoredPosition;
                        lastNumberIndicator.MoveTo(lastNumberAnchoredPosition);
                    }
                }

                // add drawn number to HashSet and MarkAsHit
                drawnNumbers.Add(draw);
                NumberManager.Instance.kenoNumbers[draw - 1].GetComponent<NumberSelect>().MarkAsHit();

                // update 'hit' and 'win' for live display
                if (NumberManager.Instance.selectedKenoNumbers.Contains(draw))
                {
                    matches++;
                    UIManager.Instance.UpdateHit(matches);

                    payout = CalculatePayout(drawnNumbers);
                    UIManager.Instance.UpdateWin(payout);
                }

                // Delay between each number draw
                yield return new WaitForSeconds(0.145f); 
            }
        }

        inGame = false;

        // last number debug
        Debug.Log(drawnNumbers.Last());

        // bonus round
        if (isInBonusRound)
        {
            // calculate bonus round winnings 
            realBonusRoundWinnings += payout;

            // update bonus winnings ui
            UIManager.Instance.UpdateTotalBonusWin(realBonusRoundWinnings);

            // decrement free games and update ui
            freeSpinsLeft--;
            UIManager.Instance.UpdateBonusGamesRemaining(freeSpinsLeft);

            if (freeSpinsLeft <= 0)
            {
                isInBonusRound = false;
                Debug.Log("Total Bonus Round Winnings: " + realBonusRoundWinnings);
            }
        }
        else
        {
            // start bonus if last number drawn on winning round
            if (payout > 0 && NumberManager.Instance.selectedKenoNumbers.Contains(drawnNumbers.Last()))
            {
                isInBonusRound = true;
                freeSpinsLeft = 12;
                StartBonusSpins();
            }
        }
    }

    private int CalculatePayout(HashSet<int> drawnNumbers)
    {
        matches = 0;

        // calculate number of 'hits'
        foreach (int number in NumberManager.Instance.selectedKenoNumbers)
        {
            if (drawnNumbers.Contains(number))
            {
                matches++;
            }
        }

        // line pickIndex up with payoutTable 
        int pickIndex = NumberManager.Instance.selectedNumbers - 3;
        int catchIndex = matches;

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
        if (isInBonusRound)
        {
            roundPayout *= 2;
        }

        // set user display payout
        payout = roundPayout;

        // update balance to reflect payout, save, and update ui
        balance += roundPayout;
        PlayerPrefs.SetInt("balance", balance);
        PlayerPrefs.Save();
        UIManager.Instance.UpdateCredit(balance);

        return roundPayout;
    }

    public void StartBonusSpins()
    {
        // activate bonus panels
        bonusPanel.SetActive(true);
        backgroundBonusPanel.SetActive(true);

        // reset bonus winnings and games remaining ui
        realBonusRoundWinnings = 0;
        UIManager.Instance.UpdateTotalBonusWin(realBonusRoundWinnings);
        UIManager.Instance.UpdateBonusGamesRemaining(freeSpinsLeft);

        StartCoroutine(BonusSpinsRoutine());
    }

    private IEnumerator BonusSpinsRoutine()
    {
        // wait before bonus round start
        yield return new WaitForSeconds(2.0f);

        // bonus spins
        while (isInBonusRound && freeSpinsLeft > 0)
        {
            ResetGame();
            GameStart();

            // wait 3 seconds between free games
            yield return new WaitForSeconds(3.0f);
        }

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

// [Developer Tools]
    public void ForceStartBonusRound()
    {
        isInBonusRound = true;
        freeSpinsLeft = 12;
        StartBonusSpins();
    }

}