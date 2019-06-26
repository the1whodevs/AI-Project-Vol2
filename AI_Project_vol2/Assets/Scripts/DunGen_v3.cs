using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunGen_v3 : MonoBehaviour
{
    [SerializeField] private int _minModulesToHave;

    [SerializeField] private GameObject[] _roomPrefabs;
    [SerializeField] private GameObject[] _junctionPrefabs;
    [SerializeField] private GameObject _corridorPrefab;

    private GameObject _currentModule;
    private GameObject _currentExit;
    private Queue<GameObject> _exitsQueue = new Queue<GameObject>();

    private ModuleInfo _currentModInfo;

    private int _instantiatedModules = 0;

    // Use this for initialization
	void Start ()
    {
        _currentModule = PickStartingModule();
        _currentModInfo = _currentModule.GetComponent<ModuleInfo>();

        //queue all of currentModule's exits (all enqueued exits are not connected!)
        for (int i = 0; i < _currentModule.transform.childCount; i++)
        {
            if (!_currentModule.transform.Find("Exit").GetComponent<ExitInfo>().IsConnected())
            {
                _exitsQueue.Enqueue(_currentModule.transform.Find("Exit").gameObject); 
                Debug.Log("Queued child!");
                _currentModule.transform.Find("Exit").gameObject.name = "Queued Exit";
            }
        }

        _currentExit = _exitsQueue.Dequeue();

        while (_exitsQueue.Count > 0)
        {
            //if (_currentExit.GetComponent<ExitInfo>().IsConnected())
            //{
            //    _currentExit = _exitsQueue.Dequeue();
            //    _currentModule = _currentExit.transform.parent.gameObject;
            //    _currentModInfo = _currentModule.GetComponent<ModuleInfo>();
            //}

            //_currentExit = _exitsQueue.Dequeue();

            //Find a suitable module
            GameObject _suitableModule = FindSuitableModule(_currentModInfo);

            //Get one of the suitable module's exits
            GameObject _exitToConnect = _suitableModule.transform.Find("Exit").gameObject;

            //Connect currentExit with suitableModExit
            ConnectExits(_currentExit, _exitToConnect);

            if (_minModulesToHave > _instantiatedModules)
            {
                //After connecting the exits, queue the rest of the exits of the _suitableModule
                for (int i = 0; i < _suitableModule.transform.childCount; i++)
                {
                    if (_suitableModule.transform.Find("Exit"))
                    {
                        if (!_suitableModule.transform.Find("Exit").GetComponent<ExitInfo>().IsConnected())
                        {
                            _exitsQueue.Enqueue(_suitableModule.transform.Find("Exit").gameObject);
                            Debug.Log("Queued child of suitable module!");
                            _suitableModule.transform.Find("Exit").gameObject.name = "Queued Exit";
                        }
                    }
                } 
            }

            if (_exitsQueue.Count > 0) //reaching this point means _currentExit is connected!
            {
                _currentExit = _exitsQueue.Dequeue();
                _currentModule = _currentExit.transform.parent.gameObject;
                _currentModInfo = _currentModule.GetComponent<ModuleInfo>();
            }
        }
    }

    /// <summary>
    /// Two exits are considered connected when exitA.transform.forward == -1 * exitB.transform.forward
    /// && exitA.transform.up == exitB.transform.up
    /// && exitA.transform.position == exitB.transform.position
    /// </summary>
    /// <param name="exitA"></param>
    /// <param name="exitB"></param>
    /// <returns></returns>
    void ConnectExits(GameObject exitA, GameObject exitB)
    {
        //make sure the module is not tilted for whatever reason
        exitB.transform.parent.rotation = Quaternion.Euler(0.0f, exitB.transform.parent.rotation.y, 0.0f);

        //Then move exitB parent on exitA transform position
        exitB.transform.parent.position = exitA.transform.position; //TODO: fix the 0.5f hardcoded value!


        //Then rotate exitB parent until exitB.transform.forward * -1 == exitA.transform.forward
        while (exitA.transform.forward != -1 * exitB.transform.forward)
        {
            exitB.transform.parent.Rotate(Vector3.up * 0.5f); //TODO: fix the 0.5f hardcoded value!
        }

        //Then, pull the exitB parent until the exitA.pos == exitB.pos
        while (exitA.transform.position != exitB.transform.position)
        {
            exitB.transform.parent.position += exitB.transform.forward * -1 * 0.5f; //TODO: fix the 0.5f hardcoded value!
        }

        //Finally, mark the two exits as connected!
        exitA.GetComponent<ExitInfo>().MarkAsConnected();
        exitB.GetComponent<ExitInfo>().MarkAsConnected();
    }

    GameObject FindSuitableModule(ModuleInfo modInfoToSuit)
    {
        bool typeSuits = false;
        ModuleType modType = ModuleType.InvalidType;

        while (!typeSuits)
        {
            int typeRand = Random.Range(0, 4); //0 is room, 1 is junction, 2 is corridor

            switch (typeRand)
            {
                case 0:
                    modType = ModuleType.Room;
                    typeSuits = modInfoToSuit.CanConnectToModule(modType);
                    break;
                case 1:
                    modType = ModuleType.Junction;
                    typeSuits = modInfoToSuit.CanConnectToModule(modType);
                    break;
                case 2:
                    modType = ModuleType.Corridor;
                    typeSuits = modInfoToSuit.CanConnectToModule(modType);
                    break;
            }
        }

        return GetModuleBasedOnType(modType); //position & rotation are not set!
    }

    /// <summary>
    /// Instantiates and returns a module based on type!
    /// </summary>
    /// <param name="modTypeToReturn"></param>
    /// <returns></returns>
    private GameObject GetModuleBasedOnType(ModuleType modTypeToReturn)
    {
        switch (modTypeToReturn)
        {
            case ModuleType.Room:
                int exitRandRoom = Random.Range(0, _roomPrefabs.Length);
                _instantiatedModules++;
                return Instantiate(_roomPrefabs[exitRandRoom]);
            case ModuleType.Junction:
                int exitRandJunction = Random.Range(0, _junctionPrefabs.Length);
                _instantiatedModules++;
                return Instantiate(_junctionPrefabs[exitRandJunction]);
            case ModuleType.Corridor:
                //_instantiatedModules++; //We won't count corridors as modules to make sure nothing ends with a corridor!
                return Instantiate(_corridorPrefab);
        }

        return new GameObject("I should never exist!");
    }

    /// <summary>
    /// Instantiates and returns a starting module!
    /// </summary>
    /// <returns></returns>
    GameObject PickStartingModule()
    {
        int modTypeRandom = Random.Range(0, 2); //0 = room, 1 = junction, no corridors for starting!
        ModuleType modType = ModuleType.InvalidType;
        GameObject failsafeGO;

        if (modTypeRandom == 0)
        {
            modType = ModuleType.Room;
        }
        else if (modTypeRandom == 1)
        {
            modType = ModuleType.Junction;
        }

        switch (modType)
        {
            case ModuleType.Room:
                int exitRand = Random.Range(1, 3); //2 to 4 exits!
                GameObject room = _roomPrefabs[exitRand];
                _instantiatedModules++;
                return Instantiate(room, Vector3.zero, Quaternion.identity);
                
            case ModuleType.Junction:
                int junctionExitRand = Random.Range(0, 2); //3 to 4 exits!
                GameObject junction = _junctionPrefabs[junctionExitRand];
                _instantiatedModules++;
                return Instantiate(junction, Vector3.zero, Quaternion.identity);
                
            case ModuleType.InvalidType:
                break;
        }

        return failsafeGO = new GameObject("Failsafe GO"); //this should never be returned!
    }
}
