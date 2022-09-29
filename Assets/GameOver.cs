using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI goalsCollectedTMP;
    public TextMeshProUGUI timeLastedTMP;

    // Start is called before the first frame update
    void Start()
    {
        // Shows the number of goals collected
        if (MainMenu.goalsCollected == 1)
            goalsCollectedTMP.text = "You collected 1 goal";
        else
            goalsCollectedTMP.text = "You collected " + MainMenu.goalsCollected + " goals";

        // Shows the ammount of time the player survived for
        timeLastedTMP.text = "You survived for: " + (Time.time-MainMenu.startOfTheGame) + " seconds";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            SceneManager.LoadScene(0);
        }
    }
}
