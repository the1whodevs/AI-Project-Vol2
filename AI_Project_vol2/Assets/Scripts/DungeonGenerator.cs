using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] _roomPrefabs; //index + 1 = number of exits
    [SerializeField] private GameObject[] _junctionPrefabs; //index + 3 = number of exits 
    [SerializeField] private GameObject _corridorPrefab; //exactly 2 exits

    private int _minModulesToSpawn; //the algorithm will keep spawning until this number is reached

    private GameObject _currentModule; //the current module we're checking to connect all exits

    private Transform _mapTransform; //the transform that is parent to all modules

    private Queue _exits = new Queue(); //all exits of each module connected

    private int _moduleCount = 0;

    // Use this for initialization
    void Start ()
    {
        _mapTransform = new GameObject("Map").transform;

        _currentModule = PickStartingModule(); //first pick a starting module

        Instantiate(_currentModule, Vector3.zero, Quaternion.identity, _mapTransform); //then spawn it
        _moduleCount++;

        //then note down all of its exits and pick one at random
        int numOfExits = _currentModule.transform.childCount;

        for (int i = 0; i < numOfExits; i++)
        {
            _exits.Enqueue(_currentModule.transform.GetChild(i).GetComponent<ExitInfo>());
        }

        ExitInfo.ModuleType[] connectableModules = _currentModule.transform.GetComponentInChildren<ExitInfo>().GetConnectableModules();

        //then, WHILE allExits are NOT connected, connect!
        ExitInfo currentExit;

        for (int i = 0; i < _exits.Count; i++)
        {
            Debug.Log(_exits.Count);

            currentExit = (ExitInfo)_exits.Dequeue();
            Debug.Log("Dequeued exit to currentExit!");

            if (!currentExit.CheckIfConnected())
            {
                Debug.Log("Got into if!");

                bool canConnect = false;
                ExitInfo.ModuleType modTypeToConnect = ExitInfo.ModuleType.InvalidType;

                for (int k = 0; k < connectableModules.Length; k++)
                {
                    Debug.Log(connectableModules[k]);
                }

                while (!canConnect)
                {
                    int modType = Random.Range(1, 4); //1 = Room, 2 = Corridor, 3 = Junction

                    switch (modType)
                    {
                        case 1:
                            modTypeToConnect = ExitInfo.ModuleType.Room;
                            Debug.Log("Switch > room!");
                            break;
                        case 2:
                            modTypeToConnect = ExitInfo.ModuleType.Corridor;
                            Debug.Log("Switch > corridor!");
                            break;
                        case 3:
                            modTypeToConnect = ExitInfo.ModuleType.Junction;
                            Debug.Log("Switch > junction!");
                            break;
                    }

                    ArrayList canConnectMods = new ArrayList();

                    for (int j = 0; j < connectableModules.Length; j++)
                    {
                        Debug.Log("Got into for: " + j);
                        canConnectMods.Add(connectableModules[j]);
                    }
                    Debug.Log("Out of for!");


                    if (canConnectMods.Contains(modTypeToConnect))
                    {
                        Debug.Log("Can connect!");
                        canConnect = true;
                    }
                    else
                    {
                        Debug.Log("Can't connect!");
                        canConnect = false;
                    }
                }

               
                Debug.Log("Got into if (canConnect!)");
                GameObject moduleToConnect;

                if (modTypeToConnect == ExitInfo.ModuleType.Room)
                {
                    moduleToConnect = Instantiate(_roomPrefabs[Random.Range(0, _roomPrefabs.Length)]);
                    Debug.Log("Making a room!");
                }
                else if (modTypeToConnect == ExitInfo.ModuleType.Corridor)
                {
                    moduleToConnect = Instantiate(_corridorPrefab);
                    Debug.Log("Making a corridor!");
                }
                else //modTypeToConnect == ExitInfo.ModuleType.Junction)
                {
                    moduleToConnect = Instantiate(_junctionPrefabs[Random.Range(0, _junctionPrefabs.Length)]);
                    Debug.Log("Making a junction!");
                }

                float yPos = currentExit.gameObject.transform.position.y;
                float xPos = currentExit.gameObject.transform.position.x;
                float zPos = currentExit.gameObject.transform.position.z - moduleToConnect.transform.localScale.z / 2;
                Vector3 pos = new Vector3(xPos, yPos, zPos);
                Debug.Log("Pos to set: " + pos);
                moduleToConnect.transform.position = pos;
                Debug.Log("Pos set!");
                currentExit.MarkAsConnected();
                Debug.Log("Marked connected!");               
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    private GameObject PickStartingModule()
    {
        int typeRandom = Random.Range(0, 2); //0 is room, 1 is junction. We won't starting with a corridor ever since we want at least 3 exits
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

        return _startingModule;
    }
}
