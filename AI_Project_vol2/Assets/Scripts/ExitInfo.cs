using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitInfo : MonoBehaviour
{
    private bool _connected = false;

    /// <summary>
    /// Marks the exit as connected. After this is called,
    /// CheckIfConnected will always return true.
    /// </summary>
	public void MarkAsConnected()
    {
        _connected = true;
        Destroy(gameObject);
    }

    /// <summary>
    /// Returns the value of _connected. If true, there is no need to connect anything more to this exit.
    /// If false, this exits needs something to connect to!
    /// </summary>
    /// <returns></returns>
    public bool CheckIfConnected() { return _connected; }

}
