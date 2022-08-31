using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
    public GameObject ObjectMusic; // Universal narrator
    private AudioSource voiceAudioSrc; // Narrator audio source

    public AudioClip enterSettingsAudio;
    public AudioClip quitSettingsAudio;
    public AudioClip mapSizeSelectedAudio;
    public AudioClip mapSizeChangedAudio;
    public AudioClip mapSizeSetToMin;
    public AudioClip mapSizeSetToMax;
    public AudioClip mapSizeSetTo5;
    public AudioClip debugModeTurnOffAudio;
    public AudioClip debugModeTurnOnAudio;
    public AudioClip debugModeTurnedOffAudio;
    public AudioClip debugModeTurnedOnAudio;

    GameObject currentSelected;
    GameObject previouslySelected;

    public static bool isDebugMode;

    public static float mapSizeX = 20;
    public static float mapSizeZ = 20;
    public TMP_InputField SizeSetter;

    public Toggle DebugToggler;

    void Start() {
        ObjectMusic = GameObject.FindWithTag("Narrator");
        voiceAudioSrc = ObjectMusic.GetComponent<AudioSource>();

        SizeSetter.text = (mapSizeX/5).ToString();

        DebugToggler.isOn = isDebugMode;

        PlayAudioClip(enterSettingsAudio);
        MainMenu.uninterruptable = true;
    }

    void Update() {
        if(MainMenu.uninterruptable && !voiceAudioSrc.isPlaying) // Turns off uninterruptable bool if the audio turned off
            MainMenu.uninterruptable = false;

        if (!MainMenu.uninterruptable) {
            currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected && (!previouslySelected || (previouslySelected.name != currentSelected.name)))
            {
                if(currentSelected.name == "BackToMenuButton")
                {
                    previouslySelected = EventSystem.current.currentSelectedGameObject;
                    PlayAudioClip(quitSettingsAudio);
                    print(currentSelected.name);
                }
                else if(currentSelected.name == "DebugToggle")
                {
                    previouslySelected = EventSystem.current.currentSelectedGameObject;
                    if (isDebugMode)
                    {
                        PlayAudioClip(debugModeTurnOffAudio);
                    }
                    else
                    {
                        PlayAudioClip(debugModeTurnOnAudio);
                    }
                    print(currentSelected.name);
                }
                else if(currentSelected.name == "InputField (TMP)")
                {
                    previouslySelected = EventSystem.current.currentSelectedGameObject;
                    PlayAudioClip(mapSizeSelectedAudio);
                    print(currentSelected.name);
                }
            }
        }
    }

    public void MapSize(string sizeText) { // Updates the size of the map
        MainMenu.uninterruptable = true;
        if (!int.TryParse(sizeText, out int size))
        {
            size = 5; // If given size was not an int, uses size 5
            PlayAudioClip(mapSizeSetTo5);
        }
        else {
            if (size > 10)
            {
                size = 10; // If given size was too big, lowers it
                PlayAudioClip(mapSizeSetToMax);
            }
            else if (size < 1)
            {
                size = 1; // If given size was too small, increases it
                PlayAudioClip(mapSizeSetToMin);
            }
            else {
                // Accepts the given size
                PlayAudioClip(mapSizeChangedAudio);
            }
    
        }
        // Updates the toggler's text
        SizeSetter.text = size.ToString();

        // Gets map scale
        mapSizeX = size*5;
        mapSizeZ = size*5;
    }

    public void DebugMode(bool tog) {
        isDebugMode = tog;
        if (!MainMenu.uninterruptable)
        {
            if (isDebugMode)
            {
                PlayAudioClip(debugModeTurnedOnAudio);
            }
            else
            {
                PlayAudioClip(debugModeTurnedOffAudio);
            }
        }
        
        print(isDebugMode);
    }

    public void QuitGame() {
        SceneManager.LoadScene(0);
    }

    void PlayAudioClip(AudioClip soundClip)
    {
        voiceAudioSrc.clip = soundClip;
        voiceAudioSrc.Play();
    }
}