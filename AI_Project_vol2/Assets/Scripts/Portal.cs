using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    Vector3 _startingModulePosition;

    public void SetStartingModulePosition(Vector3 pos)
    {
        _startingModulePosition = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = _startingModulePosition + (Vector3.up * other.transform.localScale.y / 2);
    }
}
