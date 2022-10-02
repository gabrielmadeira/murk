using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    public List<AudioClip> numbers;
    public static List<AudioClip> shareableNumbers;
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
        shareableNumbers = numbers;

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
}
