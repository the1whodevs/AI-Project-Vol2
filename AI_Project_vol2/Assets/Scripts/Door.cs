using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private DoorType _doorType;

    [SerializeField] private GameObject _eToInteract;

    private enum DoorType { Trigger, Interact }

    private Animator _doorAnimator;

    private bool _isOpen = false;

    // Use this for initialization
	void Start ()
    {
        _eToInteract.SetActive(false);
        if (_doorType == DoorType.Interact)
        {
            _doorAnimator = GameObject.Find("Interact Door").GetComponent<Animator>();
        }
        else
        {
            _doorAnimator = GameObject.Find("Trigger Door").GetComponent<Animator>();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_doorType == DoorType.Trigger)
        {
            _doorAnimator.SetBool("isOpening", true);
            _doorAnimator.SetFloat("speed", 1.0f); 
        }
        else if (_doorType == DoorType.Interact)
        {
            _eToInteract.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_doorType == DoorType.Trigger)
        {
            _doorAnimator.SetBool("isOpening", false);
            _doorAnimator.SetFloat("speed", -1.0f); 
        }
        else if (_doorType == DoorType.Interact)
        {
            _eToInteract.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_doorType == DoorType.Interact)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_isOpen)
                {
                    _doorAnimator.SetBool("isOpening", false);
                    _doorAnimator.SetFloat("speed", -1);
                    _isOpen = false;
                }
                else
                {
                    _doorAnimator.SetBool("isOpening", true);
                    _doorAnimator.SetFloat("speed", 1);
                    _isOpen = true;
                }
                
            }
        }
    }
}
