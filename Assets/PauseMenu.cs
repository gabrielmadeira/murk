using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    public Toggle DebugToggler;

    void Start()
    {
        DebugToggler.isOn = OptionsMenu.isDebugMode;
    }

    // Update is called once per frame
    void Update()
    {
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

        gameIsPaused = true;
    }

    public void DebugMode(bool tog) {
        OptionsMenu.isDebugMode = DebugToggler.isOn;
    }

    public void LoadMenu()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1; // Resumes flow of time
        Cursor.lockState = CursorLockMode.Confined;
        gameIsPaused = false;

        SceneManager.LoadScene(3);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
