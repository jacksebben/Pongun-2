using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyGroup : MonoBehaviour
{
    public DifficultyButton[] difficultyButtons;

    private const int DEFAULT_DIFFICULTY = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        SelectDifficulty(difficultyButtons[DEFAULT_DIFFICULTY]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectDifficulty(DifficultyButton dButton)
    {
        // enable the selected button and disable the others
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            if (difficultyButtons[i] == dButton)
            {
                difficultyButtons[i].Select(true);

                Debug.Log("Difficulty set to: " + i);

                MainMenu.EnemyDifficulty = i;
            }
            else
            {
                difficultyButtons[i].Select(false);
            }
        }
    }
}
