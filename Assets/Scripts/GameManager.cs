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
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI selectedText;
    public TextMeshProUGUI bonusRoundWinText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI BonusGamesText;
    public int balance;
    public int matches;
    public int payout;
    public int selectedNumbers = 0;
    public List<GameObject> kenoNumbers;
    public List<int> selectedKenoNumbers = new List<int>();
    private bool isInBonusRound = false;
    public bool inGame = false;
    private int freeSpinsLeft = 0;
    private int bonusRoundWinnings = 0;
    int realBonusRoundWinnings = 0;
    public LastDrawnNumberIndicator lastDrawnNumberIndicator;
    public GameObject bonusPanel;
    public GameObject backgroundBonusPanel;



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
        kenoNumbers = new List<GameObject>();
        balance = PlayerPrefs.GetInt("balance", 10);
        bonusPanel.SetActive(false);
        backgroundBonusPanel.SetActive(false);

        // Check for new day and reward player
        CheckAndRewardForNewDay();
    }

    public void GameStart()
    {
        // check if in current game, if so disable start button
        if (inGame)
        {
            return;
        }


        if (!isInBonusRound)
        {
            bonusPanel.SetActive(false);
            backgroundBonusPanel.SetActive(false);
            if (selectedNumbers < 3 || balance < 1)
            {
                Debug.Log("Not enough numbers selected or insufficient balance.");
                return;
            }


            balance -= 1;
            PlayerPrefs.SetInt("balance", balance);
            PlayerPrefs.Save();
        }

        // hide last number indicator
        lastDrawnNumberIndicator.Hide();

        StartCoroutine(DrawNumbersWithDelay());
    }

    private void ResetGame()
    {
        matches = 0;
        payout = 0;

        foreach (GameObject number in kenoNumbers)
        {
            number.GetComponent<NumberSelect>().UpdateSkin();
        }
    }

    private IEnumerator DrawNumbersWithDelay()
    {
        ResetGame();
        HashSet<int> drawnNumbers = new HashSet<int>();
        while (drawnNumbers.Count < 20)
        {
            inGame = true;
            int draw = Random.Range(1, 81);
            if (!drawnNumbers.Contains(draw))
            {
                if (drawnNumbers.Count == 19 && !isInBonusRound)
                {
                    GameObject lastDrawnKenoNumber = kenoNumbers[draw - 1];
                    RectTransform lastKenoRectTransform = lastDrawnKenoNumber.GetComponent<RectTransform>();
                    if (lastKenoRectTransform != null)
                    {
                        UnityEngine.Vector2 lastNumberAnchoredPosition = lastKenoRectTransform.anchoredPosition;
                        lastDrawnNumberIndicator.ShowAtPosition(lastNumberAnchoredPosition);
                    }
                }
                drawnNumbers.Add(draw);
                // kenoNumbers[draw - 1].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                kenoNumbers[draw - 1].GetComponent<NumberSelect>().MarkAsHit();

                if (selectedKenoNumbers.Contains(draw))
                {
                    matches++;
                }

                yield return new WaitForSeconds(0.145f); // Delay between each number draw
            }
        }

        inGame = false;

        // last number debug
        Debug.Log(drawnNumbers.Last());

        int roundPayout = CalculatePayout(drawnNumbers);
        if (isInBonusRound)
        {
            bonusRoundWinnings += roundPayout;
            realBonusRoundWinnings = bonusRoundWinnings;
            freeSpinsLeft--;
            if (freeSpinsLeft <= 0)
            {
                isInBonusRound = false;
                Debug.Log("Total Bonus Round Winnings: " + bonusRoundWinnings);
                bonusRoundWinnings = 0; // Reset bonus round winnings for next time
            }
        }
        else
        {
            if (roundPayout > 0 && selectedKenoNumbers.Contains(drawnNumbers.Last()))
            {
                isInBonusRound = true;
                freeSpinsLeft = 12;
                StartBonusSpins();
            }
        }

    }

    void Update()
    {
        balanceText.text = "$" + balance + ".00";
        selectedText.text = "" + selectedNumbers;
        hitText.text = "" + matches;
        winText.text = "" + payout;
        bonusRoundWinText.text = "total " + realBonusRoundWinnings;
        BonusGamesText.text = freeSpinsLeft + " Games Remaining";
    }

    public void SetLastDrawnNumberIndicator(LastDrawnNumberIndicator indicator)
    {
        lastDrawnNumberIndicator = indicator;
    }

    public void HideLastDrawnNumber()
    {
        if (lastDrawnNumberIndicator != null)
        {
            lastDrawnNumberIndicator.Hide();
        }
    }

    // Example function that shows the indicator at a new position
    public void ShowLastDrawnNumberAtPosition(UnityEngine.Vector3 newPosition)
    {
        if (lastDrawnNumberIndicator != null)
        {
            lastDrawnNumberIndicator.ShowAtPosition(newPosition);
        }
    }
    private void HighlightDrawnNumbers(HashSet<int> drawnNumbers)
    {
        foreach (GameObject number in kenoNumbers)
        {
            int numberValue = number.GetComponent<NumberSelect>().kenoNumberValue;
            TextMeshProUGUI numberText = number.GetComponentInChildren<TextMeshProUGUI>();
            if (drawnNumbers.Contains(numberValue))
            {
                numberText.color = Color.red;
            }
            else
            {
                numberText.color = Color.white;
            }
        }
    }

    private int CalculatePayout(HashSet<int> drawnNumbers)
    {
        matches = 0;
        foreach (int number in selectedKenoNumbers)
        {
            if (drawnNumbers.Contains(number))
            {
                matches++;
            }
        }

        int pickIndex = selectedNumbers - 3;
        int catchIndex = matches;
        int roundPayout = (selectedNumbers >= 3 && selectedNumbers <= 10 && catchIndex >= 0 && catchIndex < payoutTable.GetLength(1))
            ? payoutTable[pickIndex, catchIndex]
            : 0;

        if (isInBonusRound)
        {
            roundPayout *= 2;
        }

        payout = roundPayout;

        balance += roundPayout;
        PlayerPrefs.SetInt("balance", balance);
        PlayerPrefs.Save();

        return roundPayout;
    }

    public void AddSelectedNumber(int number)
    {
        if (!selectedKenoNumbers.Contains(number))
        {
            selectedKenoNumbers.Add(number);
            IncrementCounter();
        }
    }

    public void RemoveSelectedNumber(int number)
    {
        if (selectedKenoNumbers.Contains(number))
        {
            selectedKenoNumbers.Remove(number);
            DecrementCounter();
        }
    }

    public void IncrementCounter() 
    {
        selectedNumbers++;
        Debug.Log(selectedNumbers + " numbers selected");
    }

    public void DecrementCounter()
    {
        selectedNumbers--;
        Debug.Log(selectedNumbers + " numbers selected");
    }

    public void ForceStartBonusRound()
    {
        isInBonusRound = true;
        freeSpinsLeft = 12;
        StartBonusSpins(); // Start the bonus spins immediately
    }

    // Call this method to start the bonus spins
    public void StartBonusSpins()
    {
        bonusPanel.SetActive(true);
        backgroundBonusPanel.SetActive(true);
        StartCoroutine(BonusSpinsRoutine());
    }

    private IEnumerator BonusSpinsRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        while (isInBonusRound && freeSpinsLeft > 0)
        {
            ResetGame();
            GameStart();
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

            // Save the updated balance
            PlayerPrefs.SetInt("balance", balance);
            PlayerPrefs.Save();
        }
    }

}