using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{   
    public static int EnemyDifficulty = 0;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButton()
    {
        // Load the game scene.
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
