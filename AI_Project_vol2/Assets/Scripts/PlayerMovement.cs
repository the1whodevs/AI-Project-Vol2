using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotSpeed;

    [SerializeField] private AudioClip _stepClip1;
    [SerializeField] private AudioClip _stepClip2;

    private AudioSource _playerAudioSource;

    private int _stepCount = 0;

    private float _timer = 0.200f; //first time we move, we're taking a step!
    private float _stepInterval = 0.200f;

    private Rigidbody _rb;

	// Use this for initialization
	void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        _playerAudioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        MovePlayer();
        RotatePlayer();
        EnableDisaleCursor();
    }

    void EnableDisaleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool currentState = Cursor.visible;

            if (currentState)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    void PlayStepSound()
    {
        if (_timer > _stepInterval)
        {
            _stepCount++;

            if (_stepCount % 2 == 0)
            {
                _playerAudioSource.PlayOneShot(_stepClip1);
            }
            else
            {
                _playerAudioSource.PlayOneShot(_stepClip2);
            }
            _timer = 0.0f;
        }
        else
        {
            _timer += Time.fixedDeltaTime;
        }
    }

    void MovePlayer()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _rb.AddForce(transform.forward * _moveSpeed, ForceMode.VelocityChange);
            //_rb.velocity = transform.forward * _moveSpeed;
            PlayStepSound();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _rb.AddForce(-1 * transform.forward * _moveSpeed, ForceMode.VelocityChange);
            //_rb.velocity = -1 * transform.forward * _moveSpeed;
            PlayStepSound();
        }

        if (Input.GetKey(KeyCode.A))
        {
            _rb.AddForce(-1 * transform.right* _moveSpeed, ForceMode.VelocityChange);
            //_rb.velocity = -1 * transform.right * _moveSpeed;
            if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            {
                PlayStepSound(); 
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _rb.AddForce(transform.right * _moveSpeed, ForceMode.VelocityChange);
            //_rb.velocity = transform.right * _moveSpeed;
            if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            {
                PlayStepSound();
            }
        }

        //if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        //{
        //    _rb.velocity = Vector3.zero;
        //}
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX * _rotSpeed);
    }
}
