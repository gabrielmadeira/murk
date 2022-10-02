using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI resultTitleTMP;
    public TextMeshProUGUI goalsCollectedTMP;
    public TextMeshProUGUI timeLastedTMP;

    public AudioClip youScoredAudio;
    public AudioClip youSurvivedAudio;
    public AudioClip minutesAudio;
    public AudioClip secondsAudio;
    public AudioClip victoryAudio;

    private float secondsSurvived;
    private int minutesSurvived;

    // Start is called before the first frame update
    void Start()
    {

        if (MainMenu.gameWon) {
            resultTitleTMP.text = "VICTORY";
            resultTitleTMP.color = new Color32(0, 147, 0, 255); // GREEN TEXT
            DoNotDestroy.clipsToPlay.Add(victoryAudio); // Tells the player he won
        }
        else{
            resultTitleTMP.text = "GAME OVER";
            resultTitleTMP.color = new Color32(147, 0, 0, 245); // RED TEXT
        }

        // Shows the number of goals collected
        if (MainMenu.goalsCollected == 1)
            goalsCollectedTMP.text = "You scored 1";
        else
            goalsCollectedTMP.text = "You scored " + MainMenu.goalsCollected;

        // Tells the number of goals collected
        DoNotDestroy.clipsToPlay.Add(youScoredAudio);
        DoNotDestroy.playNumber(MainMenu.goalsCollected);

        // Shows the ammount of time the player survived for
        secondsSurvived = (Time.time-MainMenu.startOfTheGame);
        minutesSurvived = (int)(secondsSurvived - secondsSurvived%60)/60;
        secondsSurvived = (float)System.Math.Round(secondsSurvived%60,3);

        // Tells the ammount of time the player survived for
        DoNotDestroy.clipsToPlay.Add(youSurvivedAudio);

        if (minutesSurvived > 0) {
            DoNotDestroy.playNumber(minutesSurvived);        
            DoNotDestroy.clipsToPlay.Add(minutesAudio); // Adds "minutes" for the number said to make sense
            timeLastedTMP.text = "and survived for: " + minutesSurvived + " minutes and " + secondsSurvived + " seconds";
        }
        else
            timeLastedTMP.text = "and survived for: " + secondsSurvived + " seconds";
        

        DoNotDestroy.playNumber(secondsSurvived);
        DoNotDestroy.clipsToPlay.Add(secondsAudio); // Adds "seconds" for the number said to make sense
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) // Leaves for menu
        {
            SceneManager.LoadScene(0);
        }
    }
}
