using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{

    [SerializeField] private float _rotSpeed;

	
	// Update is called once per frame
	void Update ()
    {
        Rotate();
    }

    void Rotate()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.right, -1 * mouseY * _rotSpeed);
        
    }
}
