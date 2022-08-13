using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private GameObject _menu;
    private List<Team> _teams = new List<Team>();
    private List<List<int>> _hasNotPlayed;
    private int _startingTeam;
    private int _currentTeam;
    private int _currentWorm;
    private float _startTimeTurn;
    private GameObject _windPrefab;
    private List<GameObject> _windObj = new List<GameObject>();
    private int _windStrength = 0;
    private bool _windRandom = false;
    private GameObject _turnTimer;
    private GameObject _destructiblePrefab;
    private bool _turnOver;
    private int[] _followWorm = new int[2];
    private float _zoomOutUntil;
    private AudioClip _switchWormSound;
    [SerializeField] private AudioSource _audioSource;
        
    [SerializeField] private InputActionAsset _switchInput;
    private InputAction _switchWorm;
    private InputAction _explodeRandomWorm;
    private InputAction _zoomOut;

    [Header("Profondeur du terrain")] [SerializeField] private int _depth = 3;
    [Header("Hauteur du terrain")] [SerializeField] private int _height = 50;
    [Header("Largeur du terrain")] [SerializeField] private int _width = 70;
    [Header("Nombre de trous (espace vide)")] [SerializeField] private int _holesQty = 30;
    [Header("Largeur des trous")] [SerializeField] private float _maxDistanceFromHole = 5.29f;
    [Header("Nombre de vers (par équipe)")] [SerializeField] private int _wormsQty;
    [Header("Nombre d'effets de vent")] [SerializeField] private int _windQty = 8;
    
    void Awake()
    {
        SetActions();
        _turnOver = false;
        _followWorm[0] = -1;
        _followWorm[1] = -1;
        DirectoryInfo skyboxDir = new DirectoryInfo(Application.dataPath+"/Skybox/Resources");
        FileInfo[] fileNames = skyboxDir.GetFiles("*.mat");
        //print(fileNames[0].Name);
        //Material _skybox = (Material)AssetDatabase.LoadAssetAtPath("Assets/Skybox/" + fileNames[Random.Range(0, fileNames.Length - 1)].Name, typeof(Material));
        Material _skybox = Resources.Load<Material>(fileNames[Random.Range(0, fileNames.Length - 1)].Name.Replace(".mat", "")) as Material;
        RenderSettings.skybox = _skybox;
        //_menu = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Menu.prefab", typeof(GameObject));
        _menu = Resources.Load<GameObject>("Menu") as GameObject;
        Instantiate(_menu);    
        // _actionMap = _inGameInputs.FindActionMap("InGame");
        // _switchWorm = _actionMap.FindAction("SwitchWorm");
        // print(_switchWorm);
        // _switchWorm.performed += SwitchWorm; //https://www.youtube.com/watch?v=0z-6lxL_sS4
    }

    void Update()
    {
        if (_followWorm[0] != -1)
        {
            //print(_followWorm[0]);
            if (_followWorm[0] != _currentTeam && _followWorm[1] != _currentWorm)
            {
                 FollowWorm();
            }
            else
            {
                FocusOnCurrentWorm();
            }
        }
    }

    public void TeamsUI()
    {
        GameObject ui = Resources.Load<GameObject>("GameUICanvas(Clone)");
        if (!ui)
        {
            ui = Resources.Load<GameObject>("GameUICanvas");
        }
        int total = 0;
        // for(int t = 0; t < _teams.Count; ++t)
        // {
        //     //ui.transform.GetChild(0).GetChild(t).GetComponent<RectTransform>().sizeDelta = new Vector2(_teams[t].GetScore() + 130.0f, 0.0f);
        //     total += _teams[t].GetScore();
        // }
        ui.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + _teams[0].GetScore();
        ui.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + _teams[1].GetScore();
        ui.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().reverseArrangement = _teams[0].GetScore() >= _teams[1].GetScore() ? false : true;
        Instantiate(ui);
    }

    public void UpdateTeamsUI()
    {
        GameObject uiTeam = GameObject.Find("GameUICanvas(Clone)");
        if (!uiTeam)
        {
            uiTeam = Resources.Load<GameObject>("GameUICanvas");
        }

        GameObject team = GameObject.Find("Team_0");
        int teamScore = 0;
        for (int w = 0; w < _wormsQty * 0.5f; ++w)
        {
            Worm worm = team.transform.GetChild(w).GetComponent<Worm>();
            teamScore += worm.GetHealth();
        }
        uiTeam.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + teamScore;
        
        team = GameObject.Find("Team_1");
        teamScore = 0;
        for (int w = 0; w < _wormsQty * 0.5f; ++w)
        {
            Worm worm = team.transform.GetChild(w).GetComponent<Worm>();
            teamScore += worm.GetHealth();
        }
        uiTeam.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + teamScore;
        // uiTeam.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(_teams[0].SetScore() + 130.0f, 0.0f);
        // uiTeam.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(_teams[1].SetScore() + 130.0f, 0.0f);
        //print(_teams[0].GetScore());
        //print(_teams[1].GetScore());
    }
    
    public void SetGame(int wormsQty, int windStrength, int holesNumber, float radius)
    {
        _wormsQty = wormsQty * 2;
        _windRandom = windStrength == 4 ? true : false;
        _windStrength = windStrength == 4 ? Random.Range(0, 3) : windStrength;
        _holesQty = holesNumber;
        _maxDistanceFromHole = radius;
    }

    void SetWind()
    {
        GameObject windParent = GameObject.Find("WindContainer");
        if (windParent)
            Destroy(windParent);

        if (_windStrength != 0)
        {
            windParent = new GameObject("WindContainer");
            windParent.AddComponent<AudioSource>();
            AudioSource windAudio = windParent.GetComponent<AudioSource>();
            AudioClip windClip = Resources.Load<AudioClip>("Light Breeze Field Sound FX") as AudioClip;
            windAudio.clip = windClip;
            windAudio.loop = true;
            windAudio.playOnAwake = false;
            windAudio.pitch = 1.0f + _windStrength * 0.5f;
            windAudio.Play();

            _windPrefab = Resources.Load<GameObject>("Wind") as GameObject;
            GameObject newWind = _windPrefab;
            for (int w = 0; w < _windQty; ++w)
            {
                newWind.transform.position = new Vector3(Random.Range(0, _width), Random.Range(0, _height), -4);
                newWind.transform.rotation = new Quaternion(0, _windStrength > 0 ? 1 : -1, 0, 1);
                _windObj.Add(newWind);
                Instantiate(newWind, windParent.transform);
            }
        }
    }

    void SetTeams()
    {
        _teams.Add(new Team(0, (int)(_wormsQty * 0.5f)));
        _teams.Add(new Team(1, (int)(_wormsQty * 0.5f)));
    }

    public string GetNameFromTeam(int teamNumber, int wormNumber)
    {
        return _teams[teamNumber].GetName(wormNumber);
    }

    public void StartGame()
    {
        GameObject menu = GameObject.Find("Menu(Clone)");
        if(menu)
            Destroy(menu);

        SetTeams();
        TeamsUI();
        gameObject.GetComponent<GenerateLevel>().DrawLevel(ref _depth, ref _height, ref _width, ref _holesQty,
            ref _maxDistanceFromHole, ref _wormsQty);
        SetWind();
        _startingTeam = Random.Range(0, _teams.Count);
        _currentTeam = _startingTeam;
        StartRound();
        StartWormTurn();
    }

    void StartRound()
    {
        _hasNotPlayed = new List<List<int>>();
        for (int t = 0; t < _teams.Count; ++t)
        {
            List<int> worms = new List<int>();
            for (int w = 0; w < (int)(_wormsQty * 0.5f); ++w)
            {
                worms.Add(w);
            }
            _hasNotPlayed.Add(worms);
        }
        //print("not " + _hasNotPlayed.Count);

        if (_windRandom)
        {
            _windStrength = Random.Range(0, 3);
            SetWind();
        }
    }

    void StartWormTurn()
    {
        _startTimeTurn = Time.time;
        _turnOver = false;
        //print("start " + _startTimeTurn + " " + _turnOver);
        _turnTimer = GameObject.Find("GameUICanvas(Clone)/RoundTimer/CountdownTimer");
        _turnTimer.GetComponent<RoundTimer>().SetTimer(60);
        _turnTimer.GetComponent<RoundTimer>().StartTimer();
        _currentWorm = 0;
        GameObject team = GameObject.Find("Team_" + _currentTeam);
        int health = team.transform.GetChild(_currentWorm).GetComponent<Worm>().GetHealth();
        while (health <= 0)
        {
            ++_currentWorm;
            //print(team.transform.GetChild(_currentWorm).name);
            health = team.transform.GetChild(_currentWorm).GetComponent<Worm>().GetHealth();
        }
        team.transform.GetChild(_currentWorm).GetComponent<Worm>().SetCurrent(true);
        FocusOnCurrentWorm();
        //print("start2 " + _startTimeTurn + " " + _turnOver);
        //print(team.transform.GetChild(_currentWorm).name);
        //print(_turnTimer);
    }

    public void EndWormTurn()
    {
        //print("hein" + _hasNotPlayed.Count);
        bool endOfRound = true;
        for (int h = 0; h < _hasNotPlayed.Count; ++h)
        {
            endOfRound = _hasNotPlayed[h].Count > 0 ? false : endOfRound;
        }
        //print("end " + endOfRound);
        if (endOfRound)
        {
            //print("allo");
            StartRound();
        }

        GameObject.Find("Team_" + _currentTeam).transform.GetChild(_currentWorm).GetComponent<Worm>().SetCurrent(false);
        ++_currentTeam;
        _currentTeam = _currentTeam >= _teams.Count ? 0 : _currentTeam;
        StartWormTurn();
    }

    void FocusOnCurrentWorm()
    {
        _followWorm[0] = _currentTeam;
        _followWorm[1] = _currentWorm;
        GameObject team = GameObject.Find("Team_" + _currentTeam);
        int health = team.transform.GetChild(_currentWorm).GetComponent<Worm>().GetHealth();
        while (health <= 0)
        {
            ++_currentWorm;
            //print(team.transform.GetChild(_currentWorm).name);
            health = team.transform.GetChild(_currentWorm).GetComponent<Worm>().GetHealth();
        }
        team.transform.GetChild(_currentWorm).GetComponent<Worm>().SetCurrent(true);

        Vector3 pos = GameObject.Find("Team_" + _currentTeam).transform.GetChild(_currentWorm).transform.position;
        Vector3 zoom = _zoomOutUntil < Time.time ? new Vector3(pos.x, pos.y, pos.z - 10.0f) : new Vector3(_width * 0.5f, _height * 0.5f, pos.z - 40.0f);
        Camera.main.transform.position = zoom;
        //print("start2 " + _startTimeTurn + " " + _turnOver);
    }

    void FollowWorm()
    {
        GameObject team = GameObject.Find("Team_" + _followWorm[0]);
        Transform worm = team.transform.GetChild(_followWorm[1]);
        Vector3 pos = worm.position;
        Camera.main.transform.Translate(new Vector3(pos.x, pos.y, pos.z - 10.0f));
    }
    
    private void SetActions()
    {
        if (_switchInput != null)
        {
            var actionMap = _switchInput.FindActionMap("Worms");
            _switchWorm = actionMap.FindAction("SwitchWorm");
            _switchWorm.performed += SwitchWorm;
            _explodeRandomWorm = actionMap.FindAction("ExplodeRandomWorm");
            _explodeRandomWorm.performed += ExplodeRandomWorm;
            _zoomOut = actionMap.FindAction("ZoomOut");
            _zoomOut.performed += ZoomOut;
        }
    }

    private void ZoomOut(InputAction.CallbackContext context)
    {
        //print("allo");
        if (context.ReadValue<float>() == 1.0f)
        {
            _zoomOutUntil = Time.time + 10.0f;
        }
    }
    
    private void SwitchWorm(InputAction.CallbackContext context)
    {
        //print("allo");
        if (context.ReadValue<float>() == 1.0f)
        {
            _switchWormSound = Resources.Load<AudioClip>("WORMSELECT");
            _audioSource = GetComponent<AudioSource>();
            _audioSource.PlayOneShot(_switchWormSound);
            ++_currentWorm;
            _currentWorm = _currentWorm >= _wormsQty * 0.5f ? 0 : _currentWorm;
            bool isActive = GameObject.Find("Team_" + _currentTeam).transform.GetChild(_currentWorm).gameObject.activeSelf;
            while (!isActive)
            {
                ++_currentWorm;
                _currentWorm = _currentWorm >= _wormsQty * 0.5f ? 0 : _currentWorm;
                isActive = GameObject.Find("Team_" + _currentTeam).transform.GetChild(_currentWorm).gameObject.activeSelf;
            }
            FocusOnCurrentWorm();
        }
    }
    
    void ExplodeRandomWorm(InputAction.CallbackContext context)
    {
        //print("alll");
        if (context.ReadValue<float>() == 1.0f)
        {
            //print("BANG!!");
            int rand = Random.Range(0, 4);
            bool isActive = GameObject.Find("Team_" + Mathf.Abs(_currentTeam - 1)).transform.GetChild(rand).gameObject.activeSelf;
            while (!isActive)
            {
                rand = Random.Range(0, 4);
                isActive = GameObject.Find("Team_" + Mathf.Abs(_currentTeam - 1)).transform.GetChild(rand).gameObject.activeSelf;
            }
            Vector2 pos = GameObject.Find("Team_" + Mathf.Abs(_currentTeam - 1)).transform.GetChild(rand).transform.position;
            float rad = 1.5f;
            ExplodeBlocks(pos, rad);
        }
    }

    
    public void ExplodeBlocks(Vector2 pos, float radius)
    {
        _destructiblePrefab = Resources.Load<GameObject>("Destructible") as GameObject;
        Collider[] blocksConverted = Physics.OverlapSphere(new Vector3(pos.x, pos.y, 1), radius);
        for (int c = 0; c < blocksConverted.Length; ++c)
        {
            if (blocksConverted[c].transform.CompareTag("Ground"))
            {
                GameObject newBlock = _destructiblePrefab;
                newBlock.transform.position = blocksConverted[c].transform.position;
                blocksConverted[c].gameObject.SetActive(false);
                Instantiate(newBlock);
            }
            else
            {
                if (blocksConverted[c].gameObject.activeSelf)
                {
                    blocksConverted[c].GetComponent<Rigidbody>().AddForce(Vector3.up * 10.0f, ForceMode.Impulse);
                    blocksConverted[c].GetComponent<Worm>().SetHealth(Random.Range(15, 30));
                    _followWorm[0] = blocksConverted[c].GetComponent<Worm>().GetTeam();
                    _followWorm[1] = blocksConverted[c].transform.GetSiblingIndex();
                }
                //print(_followWorm[1]);
            }
        }
        
        _zoomOutUntil = Time.time + 5.0f;
        UpdateTeamsUI();
        //TeamsUI();
        EndWormTurn();
    }

    public int GetCurrentTeam()
    {
        return _currentTeam;
    }
}