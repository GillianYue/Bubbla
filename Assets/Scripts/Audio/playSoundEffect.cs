using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSoundEffect : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioSource audioSource;

    void Start()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playASoundEffect(int index)
    {
        audioSource.clip = clips[index];
        audioSource.Play();
    }
}
