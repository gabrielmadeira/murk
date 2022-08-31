using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject ObjectMusic; // Universal narrator
    private AudioSource voiceAudioSrc; // Narrator audio source

    public AudioClip welcomeAudio;
    public AudioClip menuAudio;
    public AudioClip startSelectedAudio;
    public AudioClip settingsSelectedAudio;
    public AudioClip quitSelectedAudio;
    public AudioClip SpacebarTopAudio; //We are missing this one

    GameObject currentSelected;
    GameObject previouslySelected;

    public static bool welcomed = false;
    public static bool uninterruptable = false;

    public float secondsUntilInstuctions = 0;

    void Start() {
        ObjectMusic = GameObject.FindWithTag("Narrator");
        voiceAudioSrc = ObjectMusic.GetComponent<AudioSource>();

        if (!welcomed)
        {
            PlayAudioClip(welcomeAudio);
            uninterruptable = true;
            welcomed = true;
        }
    }
 
     void Update()
     {
        secondsUntilInstuctions -= Time.deltaTime; // Counts down seconds until next instruction
        if (secondsUntilInstuctions<=0 && !voiceAudioSrc.isPlaying) // Checks if it can play instructions right now
        {
            PlayAudioClip(menuAudio);
            uninterruptable = true;
            secondsUntilInstuctions = 4*60; // Keeps silent for 4 minutes and then gives instructions agains
        }

        if(uninterruptable && !voiceAudioSrc.isPlaying) // Turns off uninterruptable bool if the audio turned off
            uninterruptable = false;

        if (!uninterruptable) {
            currentSelected = EventSystem.current.currentSelectedGameObject;
            if (!previouslySelected || (previouslySelected.name != currentSelected.name))
            {
                if(currentSelected.name == "PlayButton")
                {
                    previouslySelected = EventSystem.current.currentSelectedGameObject;
                    PlayAudioClip(startSelectedAudio);
                }
                else if(currentSelected.name == "SettingsButton")
                {
                    previouslySelected = EventSystem.current.currentSelectedGameObject;
                    PlayAudioClip(settingsSelectedAudio);
                }
                else if(currentSelected.name == "QuitButton")
                {
                    previouslySelected = EventSystem.current.currentSelectedGameObject;
                    PlayAudioClip(quitSelectedAudio);
                }
                print(currentSelected.name);
            }
        }
        
     }

    public void PlayGame() {
        SceneManager.LoadScene(1);
    }
    
    public void OptionsGame() {
        SceneManager.LoadScene(2);
    }

    public void QuitGame() {
        Application.Quit();
    }

    void PlayAudioClip(AudioClip soundClip)
    {
        voiceAudioSrc.clip = soundClip;
        voiceAudioSrc.Play();
    }
}
