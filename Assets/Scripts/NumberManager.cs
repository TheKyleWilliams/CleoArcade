using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberManager : MonoBehaviour
{
    // singleton instance
    private static NumberManager instance;

    // public accessor for instance
    public static NumberManager Instance 
    {
        get { return instance; }
        private set { instance = value; }
    }

    // initialize kenoNumbers and selectedKenoNumbers
    public List<GameObject> kenoNumbers = new List<GameObject>();
    public List<int> selectedKenoNumbers = new List<int>();
    public int selectedNumbers = 0;

    public void Awake()
    {
        // ensure only one NumberManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // won't destroy gameObject when switching scenes
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void AddSelectedNumber(int number)
    {
        if (!selectedKenoNumbers.Contains(number))
        {
            selectedKenoNumbers.Add(number);

            // increment selectedNumbers
            selectedNumbers++;
            Debug.Log(selectedNumbers + " numbers selected");

            // update marked UI
            UIManager.Instance.UpdateMarked(NumberManager.Instance.selectedNumbers);
        }
    }

    public void RemoveSelectedNumber(int number)
    {
        if (selectedKenoNumbers.Contains(number))
        {
            selectedKenoNumbers.Remove(number);

            // decrement selectedNumbers
            selectedNumbers--;
            Debug.Log(selectedNumbers + " numbers selected");

            // update marked UI
            UIManager.Instance.UpdateMarked(NumberManager.Instance.selectedNumbers);
        }
    }
}
