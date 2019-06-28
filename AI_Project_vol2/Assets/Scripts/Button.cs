using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    [SerializeField] private GameObject _playerTextGO;

    private Animator _buttonAnimator;

    // Use this for initialization
    void Start()
    {
        _buttonAnimator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _buttonAnimator.SetTrigger("Pressed");
            GetComponent<AudioSource>().Play();
            GameObject.Find("Portal Room").GetComponent<PortalRoom>().EnablePortal();
            GameObject.Find("PCG").GetComponent<DunGen_v3>().StartGeneration();
        }

        _playerTextGO.GetComponent<Text>().text = "Press E to interact!";
        _playerTextGO.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        _playerTextGO.SetActive(false);
    }
}
