using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum EnemyStatus { Patrolling, Chasing }

    [SerializeField] private Transform[] _patrolPoints;

    private EnemyStatus _eStatus;

    private NavMeshAgent _nma;

    private GameObject _player;

    private Transform _currentPointToReach;

	// Use this for initialization
	void Start ()
    {
        _eStatus = EnemyStatus.Patrolling;
        _player = GameObject.FindGameObjectWithTag("Player");
        _nma = GetComponent<NavMeshAgent>();
        _nma.speed = 8;
    }

    void Update()
    {
        switch (_eStatus)
        {
            case EnemyStatus.Patrolling:
                Patrol();
                break;
            case EnemyStatus.Chasing:
                _nma.SetDestination(_player.transform.position + _player.transform.forward * _player.transform.localScale.x);
                break;
        }
    }

    private void Patrol()
    {
        if (_currentPointToReach == null || DestinationReached(_currentPointToReach.position))
        {
            _currentPointToReach = _patrolPoints[Random.Range(0, _patrolPoints.Length)];
            _nma.SetDestination(_currentPointToReach.position);
        }
        else
        {
            _nma.SetDestination(_currentPointToReach.position);
        }
    }

    private bool DestinationReached(Vector3 destination)
    {
        float destX = destination.x;
        float destZ = destination.z;
        //we don't care about height

        float myX = transform.position.x;
        float myZ = transform.position.z;

        return Mathf.Approximately(myX ,destX) && Mathf.Approximately(myZ,destZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _eStatus = EnemyStatus.Chasing;
        }
    }
     void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _eStatus = EnemyStatus.Patrolling;
        }
    }
}
