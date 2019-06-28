using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

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
        }
    }
}
