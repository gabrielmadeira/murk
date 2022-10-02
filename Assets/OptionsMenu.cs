using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
    public GameObject Narrator; // Universal narrator
    private AudioSource voiceAudioSrc; // Narrator audio source

    private List<AudioClip> ClipstoPlay;

    public AudioClip enterSettingsAudio;
    public AudioClip quitSettingsAudio;
    public AudioClip mapSizeSelectedAudio;
    public AudioClip mapSizeChangedAudio;
    public AudioClip mapSizeSetToMin;
    public AudioClip mapSizeSetToMax;
    public AudioClip mapSizeSetTo4;
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
        Narrator = GameObject.FindWithTag("Narrator");
        voiceAudioSrc = Narrator.GetComponent<AudioSource>();

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
            size = 4; // If given size was not an int, uses size 4
            PlayAudioClip(mapSizeSetTo4);
        }
        else {
            if (size > 10)
            {
                size = 10; // If given size was too big, lowers it
                DoNotDestroy.clipsToPlay.Add(mapSizeSetToMax);
            }
            else if (size < 1)
            {
                size = 1; // If given size was too small, increases it
                DoNotDestroy.clipsToPlay.Add(mapSizeSetToMin);
            }
            else {
                // Accepts the given size
                DoNotDestroy.clipsToPlay.Add(mapSizeChangedAudio);
            }

            if (size != 10)
                DoNotDestroy.clipsToPlay.Add(DoNotDestroy.shareableNumbers[size]); //Says what it was set to
            else
            {
                DoNotDestroy.clipsToPlay.Add(DoNotDestroy.shareableNumbers[1]);
                DoNotDestroy.clipsToPlay.Add(DoNotDestroy.shareableNumbers[0]);
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