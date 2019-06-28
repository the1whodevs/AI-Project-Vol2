using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressurePlate : MonoBehaviour
{
    private bool _enabled = false;

    [SerializeField] private GameObject _playerTextGO;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void EnableGodlyMusic()
    {
        Debug.LogWarning("Godly music enabled!");
        _enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (_enabled)
        {
            if (other.gameObject.GetComponent<Rigidbody>().mass > 1)
            {
                _playerTextGO.GetComponent<Text>().text = "Congratulations! You may now enjoy this masterpiece!";
                _playerTextGO.SetActive(true);
                _audioSource.Play();
            } 
        }
        else
        {
            _playerTextGO.GetComponent<Text>().text = "You need to reach the portal at the end of the MAGIC! dungeon to listen to this godly music!";
            _playerTextGO.SetActive(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }

        _playerTextGO.SetActive(false);
    }
}
