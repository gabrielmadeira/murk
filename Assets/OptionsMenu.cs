using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
    public AudioSource voiceAudioSrc;
    public AudioClip enterSettingsAudio;
    public AudioClip quitSettingsAudio;
    public AudioClip mapSizeSelectedAudio;
    public AudioClip mapSizeChangedAudio;
    public AudioClip debugModeTurnOffAudio;
    public AudioClip debugModeTurnOnAudio;
    public AudioClip debugModeTurnedOffAudio;
    public AudioClip debugModeTurnedOnAudio;

    GameObject currentSelected;
    GameObject previouslySelected;

    public static bool isDebugMode;

    public static float mapSizeX;
    public static float mapSizeZ;
    public TMP_InputField SizeSetter;

    public Toggle DebugToggler;

    void Start() {
        mapSizeX = Mathf.Max(Mathf.Min(mapSizeX,50),10);
        mapSizeZ = mapSizeX;

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
            if (!previouslySelected || (previouslySelected.name != currentSelected.name))
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

    public void MapSize(string sizeText) {
        if (!int.TryParse(sizeText, out int size))
        {
            size = 5;
            SizeSetter.text = "5";
        }
        else {
            size = Mathf.Max(Mathf.Min(size,10),1); // Adjusts the map size from the minumum to the maximum
            SizeSetter.text = size.ToString();
        }

        mapSizeX = size*5;
        mapSizeZ = size*5;

        PlayAudioClip(mapSizeChangedAudio);
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