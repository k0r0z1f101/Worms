using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private GameObject _menu;
    private List<Team> _teams = new List<Team>();
    private List<List<int>> _hasNotPlayed;
    private int _startingTeam;
    private int _currentTeam;
    private float _startTimeTurn;
    private GameObject _windPrefab;
    private List<GameObject> _windObj = new List<GameObject>();
    private int _windStrength = 0;
    private bool _windRandom = false;
    private GameObject _turnTimer;

    [Header("Profondeur du terrain")] [SerializeField] private int _depth = 3;
    [Header("Hauteur du terrain")] [SerializeField] private int _height = 50;
    [Header("Largeur du terrain")] [SerializeField] private int _width = 70;
    [Header("Nombre de trous (espace vide)")] [SerializeField] private int _holesQty = 30;
    [Header("Largeur des trous")] [SerializeField] private float _maxDistanceFromHole = 5.29f;
    [Header("Nombre de vers (par équipe)")] [SerializeField] private int _wormsQty;
    [Header("Nombre d'effets de vent")] [SerializeField] private int _windQty = 8;
    
    void Awake()
    {
        DirectoryInfo skyboxDir = new DirectoryInfo(Application.dataPath+"/Skybox/Resources");
        FileInfo[] fileNames = skyboxDir.GetFiles("*.mat");
        print(fileNames[0].Name);
        //Material _skybox = (Material)AssetDatabase.LoadAssetAtPath("Assets/Skybox/" + fileNames[Random.Range(0, fileNames.Length - 1)].Name, typeof(Material));
        Material _skybox = Resources.Load<Material>(fileNames[Random.Range(0, fileNames.Length - 1)].Name.Replace(".mat", "")) as Material;
        RenderSettings.skybox = _skybox;
        //_menu = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Menu.prefab", typeof(GameObject));
        _menu = Resources.Load<GameObject>("Menu") as GameObject;
        Instantiate(_menu);
    }

    public void TeamsUI()
    {
        //GameObject ui = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/GameUICanvas.prefab", typeof(GameObject));
        GameObject ui = Resources.Load<GameObject>("GameUICanvas");
        print(ui.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta);
        for(int t = 0; t < _teams.Count; ++t)
        {
            ui.transform.GetChild(0).GetChild(t).GetComponent<RectTransform>().sizeDelta = new Vector2(
                _teams[t].GetScore() + 130.0f, 0.0f);
        }
        ui.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().reverseArrangement = _teams[0].GetScore() >= _teams[1].GetScore() ? false : true;
        Instantiate(ui);
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
            //AudioClip windClip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Sounds/Light Breeze Field Sound FX.mp3", typeof(AudioClip));
            AudioClip windClip = Resources.Load<AudioClip>("Light Breeze Field Sound FX") as AudioClip;
            windAudio.clip = windClip;
            windAudio.loop = true;
            windAudio.playOnAwake = false;
            windAudio.pitch = 1.0f + _windStrength * 0.5f;
            windAudio.Play();

            //_windPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Wind.prefab", typeof(GameObject));
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

        if (_windRandom)
        {
            _windStrength = Random.Range(0, 3);
            SetWind();
        }
    }

    void StartWormTurn()
    {
        _startTimeTurn = Time.time;
        _turnTimer = GameObject.Find("GameUICanvas(Clone)/RoundTimer/CountdownTimer");
        _turnTimer.GetComponent<RoundTimer>().SetTimer(60);
        _turnTimer.GetComponent<RoundTimer>().StartTimer();
        print(_turnTimer);
    }

    void EndWormTurn()
    {
        bool endOfRound = true;
        for (int h = 0; h < _hasNotPlayed.Count; ++h)
        {
            endOfRound = _hasNotPlayed[h].Count > 0 ? false : endOfRound;
        }
        if(endOfRound)
            StartRound();
        ++_currentTeam;
        _currentTeam = _currentTeam >= _teams.Count ? 0 : _currentTeam;
        StartWormTurn();
    }
}