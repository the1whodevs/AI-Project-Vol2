using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>().mass > 1)
        {
            _audioSource.Play();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}
