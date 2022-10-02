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
    public AudioClip secondsAudio;
    public AudioClip victoryAudio;

    private float secondsSurvived;

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
        DoNotDestroy.clipsToPlay.Add(DoNotDestroy.shareableNumbers[MainMenu.goalsCollected]);

        // Shows the ammount of time the player survived for
        secondsSurvived = Time.time-MainMenu.startOfTheGame;
        timeLastedTMP.text = "You survived for: " + secondsSurvived + " seconds";

        // Tells the ammount of time the player survived for
        DoNotDestroy.clipsToPlay.Add(youSurvivedAudio);

        // Decomposes the number of seconds survived into a list of audio clips 
        List<AudioClip> numberAudioList = new List<AudioClip>();
        while  (secondsSurvived >= 10) {
            numberAudioList.Add(DoNotDestroy.shareableNumbers[(int)(secondsSurvived%10)]);
            secondsSurvived /= 10;
        }
        numberAudioList.Add(DoNotDestroy.shareableNumbers[(int)(secondsSurvived%10)]);
        numberAudioList.Reverse(); // Reverses the order so as to match english speaking pattern

        DoNotDestroy.clipsToPlay.AddRange(numberAudioList); // Adds the clips representing the number to the list of clips to be played

        DoNotDestroy.clipsToPlay.Add(secondsAudio); // Adds "seconds" for the number said to make sense
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E)) // Leaves for menu
        {
            SceneManager.LoadScene(0);
        }
    }
}
