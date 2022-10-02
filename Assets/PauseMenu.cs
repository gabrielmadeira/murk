using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject Narrator; // Universal narrator
    private AudioSource voiceAudioSrc; // Narrator audio source

    public AudioClip pauseAudio;
    public AudioClip resumeAudio;
    public AudioClip youExitedAudio;
    public AudioClip debugModeTurnedOnAudio;
    public AudioClip debugModeTurnedOffAudio;

    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    public Toggle DebugToggler;

    void Start()
    {
        Narrator = GameObject.FindWithTag("Narrator");
        voiceAudioSrc = Narrator.GetComponent<AudioSource>();

        DebugToggler.isOn = OptionsMenu.isDebugMode;
    }

    // Update is called once per frame
    void Update()
    {
        if(MainMenu.uninterruptable && !voiceAudioSrc.isPlaying) // Turns off uninterruptable bool if the audio turned off
            MainMenu.uninterruptable = false;

        if (Input.GetKeyDown(KeyCode.E)) {
            if (gameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1; // Resumes flow of time
        Cursor.lockState = CursorLockMode.Locked;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach(AudioSource a in audios)
        {
            if (a.tag != "Narrator")
                a.UnPause();
        }

        PlayAudioClip(resumeAudio);
        gameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0; // Stops time
        Cursor.lockState = CursorLockMode.Confined;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach(AudioSource a in audios)
        {
            if (a.tag != "Narrator")
                a.Pause();
        }

        PlayAudioClip(pauseAudio);
        gameIsPaused = true;
    }

    public void DebugMode(bool tog) {
        OptionsMenu.isDebugMode = DebugToggler.isOn;
        if (!MainMenu.uninterruptable)
        {
            if (OptionsMenu.isDebugMode) {
                PlayAudioClip(debugModeTurnedOnAudio);
            }
            else {
                PlayAudioClip(debugModeTurnedOffAudio);
            }
        }
    }

    public void LoadMenu()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1; // Resumes flow of time
        Cursor.lockState = CursorLockMode.Confined;
        gameIsPaused = false;

        DoNotDestroy.clipsToPlay.Add(youExitedAudio);
        SceneManager.LoadScene(3);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    void PlayAudioClip(AudioClip soundClip)
    {
        voiceAudioSrc.clip = soundClip;
        voiceAudioSrc.Play();
    }
}
