using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible : MonoBehaviour
{
    public static event Action OnCollected;

    public GameObject planePrefab;
    private float size;

    private Vector3 randomPosition;

    // Start is called before the first frame update
    void Start()
    {
        size = 10*planePrefab.transform.localScale.x/2-5;
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
            OnCollected?.Invoke();
            ChangePostion();
        }
    }

    private void ChangePostion()
    {
        randomPosition = new Vector3(UnityEngine.Random.Range(-size,size+1),2,UnityEngine.Random.Range(-size,size+1));
        transform.position = randomPosition;
    }
}
