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
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(90f, Time.time * 100f, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().coinsCollected++;
            Debug.Log(other.gameObject.GetComponent<PlayerController>().coinsCollected);
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

    void StartAudio()
    {
        audioData = GetComponent<AudioSource>();
        audioData.Play(0);
    }
}
