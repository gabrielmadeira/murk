using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Collectible : MonoBehaviour
{
    public static event Action OnCollected;

    // Ground
    public GameObject planePrefab;

    // Ground Measurements
    private float scale_x;
    private float scale_z;

    private Vector3 randomPosition;

    // Song audio source
    public AudioSource audioData;
    public List<AudioClip> songs;
    private int amountOfSongs;

    // Switching audio source off
    public AudioSource switchAudioSource;
    public List<AudioClip> switches;

    public GameObject noiseBroadcast;

    public float updateStep = 0.05f;
    public int sampleDataLength = 1024;
 
    private float currentUpdateTime = 0f;
 
    private float clipLoudness;
    private float[] clipSampleData;

    private float noiseReach;

    // Start is called before the first frame update
    void Start()
    {
        audioData.clip = songs[MainMenu.goalsCollected]; // Sets the sound to the
        audioData.Play();
        amountOfSongs = songs.Count;

        scale_x = 10*OptionsMenu.mapSizeX/2-5;
        if (scale_x < 0)
        {
            scale_x = 0;
        }

        scale_z = 10*OptionsMenu.mapSizeZ/2-5;
        if (scale_z < 0)
        {
            scale_z = 0;
        }

        clipSampleData = new float[sampleDataLength];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localRotation = Quaternion.Euler(90f, Time.time * 100f, 0);
    }

    void Update()
    {
        //ChangeNoiseBroadcast();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.gameObject.GetComponent<PlayerController>().goalsCollectedByPlayer++; // Increments this player's amount of goals collected
            MainMenu.goalsCollected++; // Increments the global amount of coins collected
            OnCollected?.Invoke();

            if (MainMenu.goalsCollected < amountOfSongs)
                ChangePostion();
            else
            {
                Cursor.lockState = CursorLockMode.Confined; // Ends game if all goals were collected
                SceneManager.LoadScene(3);
            }
        }
    }

    void ChangePostion()
    {
        switchAudioSource.clip = switches[UnityEngine.Random.Range(0,switches.Count)]; // Plays a radio swtich off sound
        switchAudioSource.Play();

        do
        {
            randomPosition = new Vector3(UnityEngine.Random.Range(-scale_x,scale_x+1),2,UnityEngine.Random.Range(-scale_z,scale_z+1));
        } while (Vector3.Distance(transform.position, randomPosition) < (scale_x+scale_z)/2);
        transform.position = randomPosition;

        StartAudio();
    }

    void ChangeNoiseBroadcast()
    {
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep) {
            currentUpdateTime = 0f;

            audioData.clip.GetData(clipSampleData, audioData.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData) {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for

            //Calcula a distancia até onde será ouvido o som do goal.
            noiseReach = Mathf.Sqrt(50000*clipLoudness+1f);
            if (float.IsNaN(noiseReach))
            {
                noiseReach = 1f;
            }

            noiseBroadcast.transform.localScale = new Vector3(0.1f, noiseReach*155f, noiseReach*2.5f);
            
        }
    }

    void StartAudio()
    { 
        audioData.clip = songs[MainMenu.goalsCollected]; // Sets the song to that of the current level
        audioData.PlayDelayed(switchAudioSource.clip.length+UnityEngine.Random.Range(0,2)); // Makes it start with a little delay 
    }
}
