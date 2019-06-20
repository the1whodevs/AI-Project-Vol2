using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitInfo : MonoBehaviour
{ 
    public enum ModuleType { Room, Corridor, Junction, InvalidType }

    private ModuleType[] _connectableModules;
    private ModuleType _moduleType;

    private string _parentTag;

    private bool _connected = false;

	// Use this for initialization
	void Start ()
    {
        _parentTag = transform.parent.tag;
        _moduleType = ConvertTagToModuleType(_parentTag);

        switch (_moduleType)
        {
            case ModuleType.Room:
                _connectableModules = new ModuleType[1]; //rooms can only connect to corridors
                _connectableModules[0] = ModuleType.Corridor; 
                break;

            case ModuleType.Corridor:
                _connectableModules = new ModuleType[2]; //corridors can connect to both rooms and junctions
                _connectableModules[0] = ModuleType.Room;
                _connectableModules[1] = ModuleType.Junction; 
                break;

            case ModuleType.Junction:
                _connectableModules = new ModuleType[1]; //junctions can only connect to corridors
                _connectableModules[0] = ModuleType.Corridor;
                break;
        }
    }

    /// <summary>
    /// Marks the exit as connected. After this is called,
    /// CheckIfConnected will always return true.
    /// </summary>
	public void MarkAsConnected() { _connected = true; }

    /// <summary>
    /// Returns the value of _connected. If true, there is no need to connect anything more to this exit.
    /// If false, this exits needs something to connect to!
    /// </summary>
    /// <returns></returns>
    public bool CheckIfConnected() { return _connected; }

    /// <summary>
    /// Converts a module's tag to a ModuleType enum. If the string given is invalid,
    /// returns the "InvalidType" ModuleType enum.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public ModuleType ConvertTagToModuleType(string tag)
    {
        if (tag == "Room")
        {
            return ModuleType.Room;
        }
        else if (tag == "Corridor")
        {
            return ModuleType.Corridor;
        }
        else if (tag == "Junction")
        {
            return ModuleType.Junction;
        }
        else
        {
            return ModuleType.InvalidType;
        }
    }

    public ModuleType GetModuleType() { return _moduleType; }

    public ModuleType[] GetConnectableModules() { return _connectableModules; }
}
