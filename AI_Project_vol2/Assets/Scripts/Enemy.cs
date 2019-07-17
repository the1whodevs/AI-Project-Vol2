using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum EnemyStatus { Patrolling, Chasing }

    [SerializeField] private Transform[] _patrolPoints;

    [SerializeField] private GameObject[] _eyeBlacks = new GameObject[2];

    private SphereCollider _sphColl;

    private AudioSource _audioSource;

    private EnemyStatus _eStatus;

    private NavMeshAgent _nma;

    private GameObject _player;

    private Transform _currentPointToReach;

    private float _pitchOffset = 0.0f;
    private float _eyeBlackOffset = 0.0f;

	// Use this for initialization
	void Start ()
    {
        _sphColl = GetComponent<SphereCollider>();
        _eStatus = EnemyStatus.Patrolling;
        _player = GameObject.FindGameObjectWithTag("Player");
        _nma = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
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

        UpdatePitch();
        UpdateEyeBlacks();
    }

    private void UpdateEyeBlacks()
    {
        float distFromPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_eStatus == EnemyStatus.Chasing && _eyeBlacks[0].transform.localPosition.y < 0.35f && _eyeBlacks[1].transform.localPosition.y < 0.35f)
        {
            _eyeBlackOffset = 0.35f * (_sphColl.radius - distFromPlayer) / _sphColl.radius;
            _eyeBlacks[0].transform.localPosition = new Vector3(_eyeBlacks[0].transform.localPosition.x, _eyeBlackOffset, _eyeBlacks[0].transform.localPosition.z);
            _eyeBlacks[1].transform.localPosition = new Vector3(_eyeBlacks[1].transform.localPosition.x, _eyeBlackOffset, _eyeBlacks[1].transform.localPosition.z);
        }
        else
        {
            Debug.Log(_eyeBlacks[0].transform.localPosition.y);
            Debug.Log(_eyeBlacks[1].transform.localPosition.y);
        }
    }

    private void UpdatePitch()
    {
        float distFromPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_eStatus == EnemyStatus.Chasing)
        {
            _pitchOffset = (_sphColl.radius - distFromPlayer) / _sphColl.radius;

            _audioSource.pitch = 0.5f + _pitchOffset;
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
            _audioSource.Play();
        }
    }
     void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _eStatus = EnemyStatus.Patrolling;
            _audioSource.Stop();
            _eyeBlacks[0].transform.localPosition = new Vector3(_eyeBlacks[0].transform.localPosition.x, 0.0f, _eyeBlacks[0].transform.localPosition.z);
            _eyeBlacks[1].transform.localPosition = new Vector3(_eyeBlacks[1].transform.localPosition.x, 0.0f, _eyeBlacks[1].transform.localPosition.z);
        }
    }
}
