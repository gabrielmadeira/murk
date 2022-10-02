using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    public List<AudioClip> units;
    public List<AudioClip> tens;
    public AudioClip hundred;
    public AudioClip thousand;
    public AudioClip point;
    [HideInInspector]
    public static List<AudioClip> shareableUnits;
    [HideInInspector]
    public static List<AudioClip> shareableTens;
    [HideInInspector]
    public static List<AudioClip> shareableSpecials;

    private static AudioSource voiceAudioSrc;

    [HideInInspector]
    public static List<AudioClip> clipsToPlay;

    private void Awake()
    {
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("Narrator");
        if (musicObj.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        voiceAudioSrc = musicObj[0].GetComponent<AudioSource>(); // Gets audio source
        shareableUnits = units;
        shareableTens = tens;
        shareableSpecials = new List<AudioClip>();
        shareableSpecials.Add(hundred);
        shareableSpecials.Add(thousand);
        shareableSpecials.Add(point);


        clipsToPlay = new List<AudioClip>();
    }

    void FixedUpdate()
    {
        if (clipsToPlay.Count > 0) {  // While there are clips waiting to be played
            if (!voiceAudioSrc.isPlaying) {
                // Plays first audio in line
                voiceAudioSrc.clip = clipsToPlay[0];
                voiceAudioSrc.Play();

                // Removes it from the list to be played
                clipsToPlay.RemoveAt(0);
            }
        }
    }

    public static void playNumber(float numberToPlay) {
        // Decomposes the number of seconds survived into a list of audio clips 
        List<AudioClip> numberAudioList = new List<AudioClip>();

        numberToPlay = Mathf.Abs(numberToPlay); // makes the given number positive

        float decimals = (numberToPlay%1);
        numberToPlay = (int)(numberToPlay-decimals);

        // Says decimals, if there are any CRAP CODE
        string stringyDecimals = ((int)(decimals*1000)).ToString();
        if (stringyDecimals.Length > 1)
        {
            bool zeroed = true;
            for (int i = Mathf.Min(3,stringyDecimals.Length)-1; i >= 0; i--) {
                int digit = (int)(stringyDecimals[i]-'0');
                if (zeroed && digit != 0)
                    zeroed = false;
                if (!zeroed)
                    numberAudioList.Add(shareableUnits[digit]);
            }
            numberAudioList.Add(shareableSpecials[2]); // Says "point"
        }
        

        do {
            numberToPlay = DoNotDestroy.PlayHundredNumber(numberToPlay, numberAudioList);
            numberToPlay /= 10;
            if ((int)(numberToPlay) != 0) {
                numberAudioList.Add(shareableSpecials[1]); // says thousand
        }
        } while ((int)(numberToPlay%10) != 0);

        numberAudioList.Reverse(); // Reverses the order so as to match english speaking pattern
        
        clipsToPlay.AddRange(numberAudioList); // Adds the clips representing the integer number to the list of clips to be played
    }

    private static float PlayHundredNumber(float numberToPlay, List<AudioClip> audioList) { // Says 3 digits of a number
        if  ((int)(numberToPlay%100) < 20) {
            audioList.Add(shareableUnits[(int)(numberToPlay%20)]);
            numberToPlay /= 10;
        }
        else {
            if ((int)(numberToPlay%10) != 0f) // says all unit number except for 0
                audioList.Add(shareableUnits[(int)(numberToPlay%10)]);
            numberToPlay /= 10;
            if ((int)(numberToPlay%10) != 0f) // says all tens of unit number except for 00
                audioList.Add(shareableTens[(int)(numberToPlay%10)-2]);
        }

        numberToPlay /= 10;
        if ((int)(numberToPlay%10) != 0) {
            audioList.Add(shareableSpecials[0]); // says hundred
            audioList.Add(shareableUnits[(int)(numberToPlay%10)]); // says the ammount of hundreds
        }
        return numberToPlay;
    }
}
