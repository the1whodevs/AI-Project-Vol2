using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleInfo : MonoBehaviour
{
    private ModuleType _moduleType;

    private bool _canConnectToCorridor = false;
    private bool _canConnectToRoom = false;
    private bool _canConnectToJunction = false;

    // Use this for initialization
    void Awake ()
    {
        _moduleType = ConvertTagToModuleType(tag);

        switch (_moduleType)
        {
            case ModuleType.Room:
                _canConnectToCorridor = true;
                break;

            case ModuleType.Corridor:
                _canConnectToRoom = true;
                _canConnectToJunction = true;
                break;

            case ModuleType.Junction:
                _canConnectToCorridor = true;
                break;
        }

    }

    public bool CanConnectToModule(ModuleType typeToConnect)
    {
        switch (typeToConnect)
        {
            case ModuleType.Corridor:
                return _canConnectToCorridor;
            case ModuleType.Junction:
                return _canConnectToJunction;
            case ModuleType.Room:
                return _canConnectToRoom;
            default:
                Debug.LogError("Invalid typeToConnect. Type sent was: " + typeToConnect);
                return false; //in case we don't have a valid ModuleType, return false to avoid connecting wrong modules together!
        }
    }

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

    public ModuleType GetModuleType()
    {
        return _moduleType;
    }

}
