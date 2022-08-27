using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible : MonoBehaviour
{
    public static event Action OnCollected;

    // Ground
    public GameObject planePrefab;

    // Ground Measurements
    private float scale_x;
    private float scale_z;

    private Vector3 randomPosition;

    // Audio source (Hino)
    AudioSource audioData;
    public GameObject noiseBroadcast;

    public float updateStep = 0.05f;
    public int sampleDataLength = 1024;
 
    private float currentUpdateTime = 0f;
 
    private float clipLoudness;
    private float[] clipSampleData;

    // Start is called before the first frame update
    void Start()
    {
        scale_x = 10*planePrefab.transform.localScale.x/2-5;
        if (scale_x < 0)
        {
            scale_x = 0;
        }

        scale_z = 10*planePrefab.transform.localScale.z/2-5;
        if (scale_z < 0)
        {
            scale_z = 0;
        }
        ChangePostion();

        clipSampleData = new float[sampleDataLength];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localRotation = Quaternion.Euler(90f, Time.time * 100f, 0);
    }

    void Update()
    {
        ChangeNoiseBroadcast();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnCollected?.Invoke();
            ChangePostion();
        }
    }

    void ChangePostion()
    {
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

            noiseBroadcast.transform.localScale = new Vector3(0.1f, 200*(clipLoudness)*155f, 200*(clipLoudness)*2.5f);
        }
    }

    void StartAudio()
    {
        audioData = GetComponent<AudioSource>();
        audioData.Play(0);
    }
}
