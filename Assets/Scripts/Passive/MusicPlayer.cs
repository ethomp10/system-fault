﻿using UnityEngine;

//
// MusicPlayer.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Dynamically starts and stops planet music
//

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]

public class MusicPlayer : MonoBehaviour {

    [SerializeField] float maxVolume = 0.5f;
    [SerializeField] float fadeSpeed = 0.2f;

    AudioSource source;
    int fadeDirection = 0;

	void Start () {
        source = GetComponent<AudioSource>();
	}
	
	void Update () {
        if (fadeDirection == -1) FadeOut();
        else if (fadeDirection == 1) FadeIn();
	}

    void OnTriggerEnter(Collider other) {
        if (other.GetComponentInChildren<Camera>()) {
            Debug.Log("Music Player: Music Zone Entered");

            if (source.isPlaying) {
                fadeDirection = 1;
            } else {
                source.volume = maxVolume;
                source.Play();
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.GetComponentInChildren<Camera>()) {
            Debug.Log("Music Player: Music Zone Exited");
            fadeDirection = -1;
        }
    }

    void FadeOut() {
        source.volume -= Time.deltaTime * fadeSpeed;
        if (source.volume <= 0f) {
            source.Stop();
            fadeDirection = 0;
        }
    }

    void FadeIn() {
        source.volume += Time.deltaTime * fadeSpeed;
        if (source.volume >= maxVolume) {
            fadeDirection = 0;
        }
    }
}
