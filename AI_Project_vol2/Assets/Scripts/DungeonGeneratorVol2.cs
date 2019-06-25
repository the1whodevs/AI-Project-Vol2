using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleType { Room, Corridor, Junction, InvalidType }

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
    private ModuleInfo _currentModuleInfo; //the current moduleInfo of the module we're checking to connect all exits

    private Transform _mapTransform; //the transform that is parent to all modules

    private Queue<GameObject> _exits = new Queue<GameObject>(); //all exits of each module connected
    private Queue<GameObject> _allModulesToConnect = new Queue<GameObject>(); //all modules that need to be connected

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
        _currentModule = PickStartingModule();

        while (_moduleCount < _minModulesToHave)
        {
            //Enqueue all exits
            EnqueueAllExits();

            Debug.Log("Found " + _currentModule.transform.childCount + " children!");
            Debug.Log("Enqueued " + _exits.Count + " children!");

            //Dequeue the _exits one by one and connect them with suitable modules
            ConnectExits();

            //All exits of current module are connected at this point
            //change current module and repeat!

            if (_allModulesToConnect.Count > 0)
            {
                _currentModule = _allModulesToConnect.Dequeue();
            }
            else
            {
                break; //no more modules to spawn! 
            }
        }
    }

    private void ConnectExits()
    {
        while (_exits.Count > 0)
        {
            GameObject exitA = _exits.Dequeue();
            Debug.Log("Exit A is now " + exitA.transform.parent.name);

            GameObject _suitableMod = Instantiate(FindSuitableModule(exitA.transform.GetComponentInParent<ModuleInfo>()), exitA.transform.position,
                Quaternion.identity);
            _moduleCount++;

            GameObject exitB = _suitableMod.transform.Find("Exit").gameObject;

            while (exitB.GetComponent<ExitInfo>().CheckIfConnected())
            {
                exitB.name = "Connected Exit";
                exitB = _suitableMod.transform.Find("Exit").gameObject;

                if (!_suitableMod.transform.Find("Exit"))
                {
                    break;
                }
            }
            Debug.Log("Exit found!");

            float totalRotation = 0;

            //At this point, we should have two modules that can connect, and their exits that we need to connect!
            while (exitA.transform.forward != -1 * exitB.transform.forward)
            {
                //exitA is our constant, we're adjusting exitB to it!
                exitB.transform.parent.Rotate(Vector3.up * _rotAdjustment);
                totalRotation += _rotAdjustment;

                if (_rotAdjustment > 360)
                {
                    //we did a whole circle and didn't match the transform.forwards to be opposite, let's exit the loop and output an error!
                    Debug.LogError("Rotated 360 degrees and couldn't match the exit forwards!");
                    break;
                }
            }
            Debug.Log("Forwards are opposite!");

            int direction;

            float angle = Vector3.Angle(exitA.transform.forward, exitB.transform.forward);

            if (Mathf.Approximately(angle, 180f))
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }

            Debug.Log("DIRECTION DONE!");

            //At this point, we should have two modules with their exits having opposite tranform.forward

            int tries = 0;

            //We've ensured with the dot product that the direction we're moving is the correct direction!
            while (exitA.transform.position != exitB.transform.position)
            {
                exitB.transform.parent.position += exitB.transform.forward * direction * _posAdjustment;
                tries++;

                if (tries > 10000)
                {
                    Debug.Log("BREAK!");
                    Debug.Log(Vector3.Distance(exitA.transform.position, exitB.transform.position));
                    break;
                }
            }

            //And at this point, we should have two connected modules!
            exitA.GetComponent<ExitInfo>().MarkAsConnected();
            exitB.GetComponent<ExitInfo>().MarkAsConnected();

            if (_suitableMod.transform.childCount > 0)
            {
                _allModulesToConnect.Enqueue(_suitableMod);
            }
        }
    }

    private void EnqueueAllExits()
    {
        for (int i = 0; i < _currentModule.transform.childCount; i++)
        {
            GameObject child = _currentModule.transform.Find("Exit").gameObject;
            ExitInfo childExitInfo = child.GetComponent<ExitInfo>();

            if (childExitInfo.CheckIfConnected())
            {
                Debug.Log("Child already connected!");
                child.name = "Connected Exit";
            }
            else
            {
                _exits.Enqueue(child);
                Debug.Log("Child queued!");
                child.name = "Queued Exit";
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    //Instantiates at 0,0,0 a room with at least 3 exits, and returns a reference to it.
    private GameObject PickStartingModule()
    {
        //For rooms: Index + 1 = Number of Exits

        int exitRand = Random.Range(2, _roomPrefabs.Length); //starting module has at least 3 exits and is a room!
        
        //We'll always start at 0,0,0 for now!
        GameObject _startingMod = Instantiate(_roomPrefabs[exitRand], Vector3.zero, Quaternion.identity);
        _moduleCount++;
        _roomCount++;
        _startingMod.name = "Starting Module";
        return _startingMod;
    }

    /// <summary>
    /// Finds a suitable module based on ModuleInfo given.
    /// </summary>
    /// <param name="moduleToSuit"></param>
    /// <returns></returns>
    private GameObject FindSuitableModule(ModuleInfo moduleToSuit)
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

        GameObject gameObjectToReturn;
        int exitRand = -1;

        switch (moduleToReturn)
        {
            case ModuleType.Room:
                exitRand = Random.Range(0, _roomPrefabs.Length);
                gameObjectToReturn = _roomPrefabs[exitRand];
                _roomCount++;
                gameObjectToReturn.name = "Room #" + _roomCount;
                return gameObjectToReturn;

            case ModuleType.Corridor:
                gameObjectToReturn = _corridorPrefab;
                _corridorCount++;
                gameObjectToReturn.name = "Corridor #" + _corridorCount;
                return gameObjectToReturn;

            case ModuleType.Junction:
                exitRand = Random.Range(0, _junctionPrefabs.Length);
                gameObjectToReturn = _junctionPrefabs[exitRand];
                _junctionCount++;
                gameObjectToReturn.name = "Juction #" + _junctionCount;
                return gameObjectToReturn;

            case ModuleType.InvalidType:
                Debug.LogError("We shouldn't be here!");
                break;
        }

        Debug.LogError("I got out of the switch statement and didn't find a suitable module!!");
        return gameObjectToReturn = new GameObject("I should never exist.");
    }

}
