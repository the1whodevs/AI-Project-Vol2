using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaCheck : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Room") || other.gameObject.CompareTag("Junction"))
        {
            GameObject.Find("PCG").GetComponent<DunGen_v3>().RestartGeneration();
        }
    }

}
