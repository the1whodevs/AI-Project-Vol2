using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unneccessary_Jerry_Script : MonoBehaviour
{

    [SerializeField] private Texture2D[] memeFrames;
    [SerializeField] private int memeFramesPerSecond;

    private Renderer _rend;

    // Use this for initialization
	void Start ()
    {
        _rend = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        int index  = (int)(Time.time * memeFramesPerSecond);
        index = index % memeFrames.Length;
        _rend.material.mainTexture = memeFrames[index];
    }
}
