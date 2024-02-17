using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpinManager : MonoBehaviour
{
    // singleton instance
    private static SpinManager instance;

    // public accessor for instance
    public static SpinManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    [SerializeField]
    private LastNumberIndicator lastNumberIndicator;

    [SerializeField]
    private GameObject bonusPanel;

    [SerializeField]
    private GameObject backgroundBonusPanel;
    private int freeSpinsLeft = 0;

    public bool inGame = false;
    public bool isInBonusRound = false; 
    public int matches, payout;
    public int realBonusRoundWinnings = 0;

    // Start is called before the first frame update
    void Start()
    {
        // ensure only one SpinManager exists
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

    public void GameStart()
    {
        // check if in current game, if so disable start button
        if (inGame)
        {
            return;
        }

        // ensure at least two numbers are selected and balance is sufficient 
        if (NumberManager.Instance.selectedNumbers < 3 || GameManager.Instance.balance < 1)
        {
            Debug.Log("Not enough numbers selected or insufficient balance.");
            return;
        }

        // deduct balance if not in bonus
        if (!isInBonusRound)
        {
            // hide bonus panels
            bonusPanel.SetActive(false);
            backgroundBonusPanel.SetActive(false);

            // deduct balance, save, and update ui
            GameManager.Instance.balance -= 1;
            PlayerPrefs.SetInt("balance", GameManager.Instance.balance);
            PlayerPrefs.Save();
            UIManager.Instance.UpdateCredit(GameManager.Instance.balance);
        }

        // start drawing
        StartCoroutine(DrawNumbersWithDelay());
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

                    payout = GameManager.Instance.CalculatePayout(drawnNumbers);
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

    // [Developer Tools]
    public void ForceStartBonusRound()
    {
        isInBonusRound = true;
        freeSpinsLeft = 12;
        StartBonusSpins();
    }
}
