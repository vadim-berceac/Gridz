using System.Collections.Generic;
using UnityEngine;
using static TeamsInitializer;

public class TeamTurnManager : MonoBehaviour
{
    [SerializeField][Range(0, 48)] private int _maxTurnCount;
    private int _currentTurn;
    private int _currentSubTurn;
    private Team _activeTeam;
    private Queue<Team> _teams = new();
    private static TeamTurnManager _instance;

    public int CurrentTurn => _currentTurn;
    public Team ActiveTeam => _activeTeam;
    public Queue<Team> Teams => _teams;
    public static TeamTurnManager Instance => _instance;

    private void Start()
    {
        Initialize();
        StartNewSubTurn();
    }

    private void Initialize()
    {
        CreateQueue(ref _teams);
        _instance = this;
    }

    private void CreateQueue(ref Queue<Team> teams)
    {
        if(TeamsInitializer.Instance == null)
        {
            Debug.LogWarning("No teams created!");
            return;
        }

        teams = new(TeamsInitializer.Instance.Teams);
    }

    public void StartNewSubTurn()
    {
        if(_currentTurn < _maxTurnCount)
        {
            if(_currentSubTurn < _teams.Count)
            {
                _currentSubTurn++;
                if (_activeTeam != null)
                {
                    _teams.Enqueue(_activeTeam);
                }
                _activeTeam = _teams.Dequeue();
                Debug.Log(_currentTurn + " " + _activeTeam.CMode);
            }
            else
            {
                _currentSubTurn = 0;
                _currentTurn ++;    
            }
        }
        else
        {
            //invoke match end
        }        
    }
}
