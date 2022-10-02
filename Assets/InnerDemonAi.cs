using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDemonAi : MonoBehaviour
{
    private AudioSource demonAudioSrc;
    private MeshRenderer demonRender;
    public List<AudioClip> demonWhisperClips;
    private float delay;
    private float velocity;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        demonRender = GetComponent<MeshRenderer>(); // Gets the demons render

        demonAudioSrc = GetComponent<AudioSource>(); // Gets audio source

        Randomize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log("Delay: " + delay + " velocity: " + velocity + " distance: " + distance);
        delay -= Time.deltaTime;
        if (delay <= 0) {
            demonRender.enabled = true;

            demonAudioSrc.clip = demonWhisperClips[Random.Range(0,demonWhisperClips.Count)]; // Choses a whisper
            demonAudioSrc.Play(); // Plays the demonic whisper

            Randomize();
        }

        if (demonRender.enabled && !demonAudioSrc.isPlaying) { // If the demon goes silent
            demonRender.enabled = false; // Makes the demon disapear
        }

        Vector3 relativePos = new Vector3(distance*Mathf.Cos(Time.time*velocity),0.5f,distance*Mathf.Sin(Time.time*velocity)); // Gets the position of the demon relative to the player
        transform.position = transform.parent.transform.position + relativePos; // Makes the demon circle the player
    }

    private void Randomize() {
        delay = Random.Range(60,300); // Gets a random new delay
        velocity = Random.Range(-3,3); // Gets a random velocity
        distance = Random.Range(1f,2f); // Gets a random distance
    }
}
