using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorVol2 : MonoBehaviour
{

    #region Serialized Fields

    [SerializeField] private GameObject[] _roomPrefabs; //index + 1 = number of exits
    [SerializeField] private GameObject[] _junctionPrefabs; //index + 3 = number of exits 
    [SerializeField] private GameObject _corridorPrefab; //exactly 2 exits

    [SerializeField] private float _rotAdjustment;
    [SerializeField] private float _posAdjustment;

    [SerializeField] private int _minModulesToHave; //the algorithm will keep spawning until this number is reached 

    #endregion

    #region Private Fields

    private GameObject _currentModule; //the current module we're checking to connect all exits
    private GameObject _moduleToConnect; //the module to connect the currentModule to

    private Transform _mapTransform; //the transform that is parent to all modules

    private Queue<GameObject> _exits = new Queue<GameObject>(); //all exits of each module connected

    #endregion

    #region Counters

    private int _moduleCount = 0;

    private int _roomCount = 0;
    private int _junctionCount = 0;
    private int _corridorCount = 0;

    #endregion

    // Use this for initialization
    void Start()
    {
        InitializeWithFirstModule();
    }

    private void InitializeWithFirstModule()
    {
        _currentModule = PickStartingModule();
        _currentModule = Instantiate(_currentModule, Vector3.zero, Quaternion.identity);

        //At this point, the _currentModule and _moduleToConnect should both be all set up and matching!
        //Enqueue the exits of the currentModule, and fill them up
        for (int i = 0; i < _currentModule.transform.childCount; i++)
        {
            _exits.Enqueue(_currentModule.transform.Find("Exit").gameObject);
            Debug.Log("Queued Exit!");
        }

        //Connect all the exits with a suitable module
        while (_exits.Count > 0)
        {
            //First get a suitable module type
            switch (FindSuitableModule(_currentModule.GetComponent<ModuleInfo>()))
            {
                case ModuleType.Corridor:
                    _moduleToConnect = Instantiate(_corridorPrefab);
                    _corridorCount++;
                    _moduleToConnect.name = "Corridor " + _corridorCount;
                    break;
                case ModuleType.Room:
                    int roomExitRand = Random.Range(0, _roomPrefabs.Length);
                    _moduleToConnect = Instantiate(_roomPrefabs[roomExitRand]);
                    _roomCount++;
                    _moduleToConnect.name = "Room " + _roomCount;
                    break;
                case ModuleType.Junction:
                    int junctionRandExit = Random.Range(0, _junctionPrefabs.Length);
                    _moduleToConnect = Instantiate(_junctionPrefabs[junctionRandExit]);
                    _junctionCount++;
                    _moduleToConnect.name = "Junction " + _junctionCount;
                    break;
                case ModuleType.InvalidType:
                    Debug.LogError("No suitable module to connect!");
                    break;
            }
            _moduleCount++;

            //then connect the exits
            GameObject exitA = _exits.Dequeue(); //we're not adjusting this, this is stable
            GameObject exitB = _moduleToConnect.transform.Find("Exit").gameObject; //we're adjusting this!

            //make sure the transform.forwards are opposite
            exitA.transform.parent.rotation = Quaternion.Euler(Vector3.zero);
            exitB.transform.parent.rotation = Quaternion.Euler(Vector3.zero);

            while (exitA.transform.forward != -1 * exitB.transform.forward)
            {
                exitB.transform.parent.Rotate(_rotAdjustment * Vector3.up);
            } 

            //Make sure the exits are looking at each other
            float _angle = Vector3.Angle(exitA.transform.forward, exitB.transform.forward);
            int direction;

            if (Mathf.Approximately(_angle, 180.0f))
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }

            //try first X, then Z matching
            //y must be the same!
            while (exitA.transform.position != exitB.transform.position) //CRASHES!!!
            {
                exitB.transform.position += exitB.transform.forward * direction * _posAdjustment;
            }
        }
    }


    private GameObject PickStartingModule()
    {
        //For rooms: Index + 1 = Number of Exits
        int exitRand = Random.Range(2, _roomPrefabs.Length); //starting module has at least 3 exits and is a room!
        
        //We'll always start at 0,0,0 for now!
        GameObject _startingMod = _roomPrefabs[exitRand];
        _startingMod.name = "Starting Module";
        return _startingMod;
    }

    /// <summary>
    /// Finds a suitable module based on ModuleInfo given.
    /// </summary>
    /// <param name="moduleToSuit"></param>
    /// <returns></returns>
    private ModuleType FindSuitableModule(ModuleInfo moduleToSuit)
    {
        bool typeSuits = false;
        ModuleType moduleToReturn = ModuleType.InvalidType;

        while (!typeSuits)
        {
            int randType = Random.Range(0, 3); //0 is Room, 1 is Junction, 2 is Corridor

            switch (randType)
            {
                case 0:
                    typeSuits = moduleToSuit.CanConnectToModule(ModuleType.Room);
                    moduleToReturn = ModuleType.Room;
                    break;
                case 1:
                    typeSuits = moduleToSuit.CanConnectToModule(ModuleType.Junction);
                    moduleToReturn = ModuleType.Junction;
                    break;
                case 2:
                    typeSuits = moduleToSuit.CanConnectToModule(ModuleType.Corridor);
                    moduleToReturn = ModuleType.Corridor;
                    break;
            }
        }

        return moduleToReturn;
    }

}
