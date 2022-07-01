using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private List<Team> _teams = new List<Team>();
    private List<List<int>> _hasNotPlayed;
    private int _startingTeam;
    private int _currentTeam;
    private float _startTimeTurn;

    [Header("Profondeur du terrain")] [SerializeField] private int _depth = 3;
    [Header("Hauteur du terrain")] [SerializeField] private int _height = 50;
    [Header("Largeur du terrain")] [SerializeField] private int _width = 70;
    [Header("Nombre de trous (espace vide)")] [SerializeField] private int _holesQty = 30;
    [Header("Largeur des trous")] [SerializeField] private float _maxDistanceFromHole = 5.29f;
    [Header("Nombre de vers (par équipe)")] [SerializeField] private int _wormsQty;
    
    void Awake()
    {
        SetTeams();
        gameObject.GetComponent<GenerateLevel>().DrawLevel(ref _depth, ref _height, ref _width, ref _holesQty, ref _maxDistanceFromHole, ref _wormsQty);
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

    void StartGame()
    {
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
    }

    void StartWormTurn()
    {
        _startTimeTurn = Time.time;
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
