using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Destination { Start, Dungeon }

public class Portal : MonoBehaviour
{
    [SerializeField] private Destination _myDestination;

    private GameObject _portalBack;

    Vector3 _startingModulePosition;

    void Start()
    {
        _portalBack = GameObject.Find("Portal Back");
    }

    public void SetStartingModulePosition(Vector3 pos)
    {
        _startingModulePosition = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            Teleport(other.transform);
        }
    }

    private void Teleport(Transform transformToTP)
    {
        if (_myDestination == Destination.Dungeon)
        {
            transformToTP.position = _startingModulePosition + (Vector3.up * transformToTP.localScale.y / 2);
        }
        else if (_myDestination == Destination.Start)
        {
            GameObject.FindGameObjectWithTag("Respawn").GetComponent<PressurePlate>().EnableGodlyMusic();
            transformToTP.position = _portalBack.transform.position;
        }
    }

    public void SetDestination(Destination dest)
    {
        _myDestination = dest;
    }
}
