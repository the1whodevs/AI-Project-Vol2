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
            Debug.Log(gameObject.name + " collided with " + other.gameObject.name);
            //SceneManager.LoadScene(0); //restart the process!
            GameObject.Find("PCG").GetComponent<DunGen_v3>().RestartGeneration();
        }
    }

}
