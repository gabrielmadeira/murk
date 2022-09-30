using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cricketAiNoJoke : MonoBehaviour
{
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        //transform.localRotation = Quaternion.Euler(90f, Time.time * 100f, 0); // Spins the cricket :D
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Monster") //Makes the cricket goes silent near danger
        {
            //Debug.Log("IM IN DANGER");
            source.volume = 0f;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Monster") //Makes the cricket sing when safe
        {
            //Debug.Log("IM NO MORE IN DANGER");
            source.volume = 0.1f;
        }
    }



}
