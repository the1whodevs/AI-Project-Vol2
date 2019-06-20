using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleType { Room, Corridor, Junction, InvalidType }

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] _roomPrefabs; //index + 1 = number of exits
    [SerializeField] private GameObject[] _junctionPrefabs; //index + 3 = number of exits 
    [SerializeField] private GameObject _corridorPrefab; //exactly 2 exits

    [SerializeField] private int _minIterations; //the algorithm will keep spawning until this number is reached

    private GameObject _currentModule; //the current module we're checking to connect all exits
    private ModuleInfo _currentModuleInfo; //the current moduleInfo of the module we're checking to connect all exits

    private Transform _mapTransform; //the transform that is parent to all modules

    private Queue<GameObject> _exits = new Queue<GameObject>(); //all exits of each module connected
    private Queue<GameObject> _allModulesToConnect = new Queue<GameObject>(); //all modules that need to be connected

    private int _moduleCount = 0;

    // Use this for initialization
    void Start ()
    {
        GenerateRandomDungeon();
    }

    private void GenerateRandomDungeon() //TODO: Make it so this is repeated until enough modules are in the map!
    {
        SetStartingModule();

        _currentModule = Instantiate(_allModulesToConnect.Dequeue(), Vector3.zero, Quaternion.identity); //Instantiate the module 

        for (int j = 0; j < _minIterations; j++)
        {
            _currentModuleInfo = _currentModule.GetComponent<ModuleInfo>();

            int childrenCount = _currentModule.transform.childCount;

            for (int i = 0; i < childrenCount; i++)
            {
                GameObject _currentExit = _currentModule.transform.GetChild(i).gameObject;
                ExitInfo _exitInfoToCheck = _currentExit.GetComponent<ExitInfo>();

                if (!_exitInfoToCheck.CheckIfConnected())
                {
                    _exits.Enqueue(_currentExit);
                }
            }

            while (_exits.Count > 0) //&& _moduleCount < _minIterations
            {
                GameObject _exitToConnectA = _exits.Dequeue(); //dequeues an exit gameobject that also has an exitinfo component that is not marked as connected.

                GameObject _suitableModule = FindSuitableModule(_currentModuleInfo); //select an appropriate module to instantiate
                //_allModulesToConnect.Enqueue(_suitableModule);
                GameObject _exitToConnectB = _suitableModule.transform.Find("Exit").gameObject;

                ConnectExits(_exitToConnectA,
                    _exitToConnectB); //pick an exit from the _suitableModule, and connect it with a !connected exit of the _currentModule 
            }

            Debug.Log(_moduleCount);
            Debug.Log(_minIterations);
        }

    }

    /// <summary>
    /// Tries to connect two exits. Returns true when it's a success, false when it's a failure.
    /// Two exits are considered connected when their position are the same, and the Z+ axes are
    /// opposite while the Y+ axes are matching.
    /// </summary>
    /// <param name="exitA"></param>
    /// <param name="exitB"></param>
    /// <returns></returns>
    private void ConnectExits(GameObject exitA, GameObject exitB)
    {
        //Match the positions of the exits, have their Transform.forward opposite and their Transform.up the same.
        while (exitA.transform.forward != exitB.transform.forward * -1)
        {
            exitB.transform.parent.Rotate(90 * Vector3.up);
        }

        while (exitA.transform.position != exitB.transform.position)
        {
            exitB.transform.parent.position -= exitB.transform.forward * 0.1f; 
        }

        exitA.GetComponent<ExitInfo>().MarkAsConnected();
        exitB.GetComponent<ExitInfo>().MarkAsConnected();
    }

    /// <summary>
    /// Finds and instantiates a suitable module based on ModuleInfo given.
    /// </summary>
    /// <param name="moduleToSuit"></param>
    /// <returns></returns>
    private GameObject FindSuitableModule(ModuleInfo moduleToSuit)
    {
        bool typeSuits = false;
        ModuleType moduleToInstantiate = ModuleType.InvalidType;

        while (!typeSuits)
        {
            int randType = Random.Range(0, 3); //0 is Room, 1 is Junction, 2 is Corridor

            switch (randType)
            {
                case 0:
                    typeSuits = moduleToSuit.CanConnectToModule(ModuleType.Room);
                    moduleToInstantiate = ModuleType.Room;
                    break;
                case 1:
                    typeSuits = moduleToSuit.CanConnectToModule(ModuleType.Junction);
                    moduleToInstantiate = ModuleType.Junction;
                    break;
                case 2:
                    typeSuits = moduleToSuit.CanConnectToModule(ModuleType.Corridor);
                    moduleToInstantiate = ModuleType.Corridor;
                    break;
            }
        }

        GameObject gameObjectToReturn;
        int exitRand = -1;

        switch (moduleToInstantiate)
        {
            case ModuleType.InvalidType:
                Debug.LogError("We shouldn't have exited the while loop!");
                break;
            case ModuleType.Room:
                exitRand = Random.Range(0, _roomPrefabs.Length);
                gameObjectToReturn = Instantiate(_roomPrefabs[exitRand]);
                _moduleCount++;
                _allModulesToConnect.Enqueue(gameObjectToReturn);
                return gameObjectToReturn;
            case ModuleType.Junction:
                exitRand = Random.Range(0, _junctionPrefabs.Length); // remember that index + 3 = number of exits!
                gameObjectToReturn = Instantiate(_junctionPrefabs[exitRand]);
                _allModulesToConnect.Enqueue(gameObjectToReturn);
                _moduleCount++;
                return gameObjectToReturn;
            case ModuleType.Corridor:
                gameObjectToReturn = Instantiate(_corridorPrefab);
                _allModulesToConnect.Enqueue(gameObjectToReturn);
                _moduleCount++;
                return gameObjectToReturn;
        }

        Debug.LogError("I got out of the switch statement and didn't find a suitable module!!");
        return gameObjectToReturn = new GameObject("I should never exist.");
    }

    private void SetStartingModule()
    {
        int typeRandom = -1; //0 is room, 1 is junction, 2 is corridor 

        typeRandom = Random.Range(0, 2); //no corridors for startingModule!

        int exitsRandom = -1; //if this is -1 after the typeRandom if statements, something went wrong!

        GameObject _startingModule = new GameObject("Starting Module");

        if (typeRandom == 0)
        {
            //room
            exitsRandom = Random.Range(0, _roomPrefabs.Length); // remember that index + 1 = number of exits!
            _startingModule = _roomPrefabs[exitsRandom];
        }
        else if (typeRandom == 1)
        {
            //junction
            exitsRandom = Random.Range(0, _junctionPrefabs.Length); // remember that index + 3 = number of exits!
            _startingModule = _junctionPrefabs[exitsRandom];
        }

        _moduleCount++;
        _allModulesToConnect.Enqueue(_startingModule);
        //return Instantiate(_startingModule, Vector3.zero, Quaternion.identity); //Instantiate the module
    }
}
