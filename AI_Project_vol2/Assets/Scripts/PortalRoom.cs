using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalRoom : MonoBehaviour
{

    [SerializeField] private GameObject _portal;

	// Use this for initialization
	void Start ()
    {
	    _portal.SetActive(false);   	
	}
    
    public void EnablePortal()
    {
        _portal.SetActive(true);
    }
}
