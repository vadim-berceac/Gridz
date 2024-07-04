using System.Collections.Generic;
using UnityEngine;
using static TeamsInitializer;

public class TeamTurnManager : MonoBehaviour
{
    [SerializeField][Range(0, 48)] private int _maxTurnCount;
    private int _currentTurn;
    private Team _activeTeam;
    private Queue<Team> _teams = new();
    private static TeamTurnManager _instance;

    public int currentTurn => _currentTurn;
    public Team ActiveTeam => _activeTeam;
    public Queue<Team> Teams => _teams;
    public static TeamTurnManager Instance => _instance;

    private void Start()
    {
        Initialize();
        StartNewTurn();
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

    public void StartNewTurn()
    {
        if(_currentTurn < _maxTurnCount)
        {
            _currentTurn++;
            if(_activeTeam != null)
            {
                _teams.Enqueue(_activeTeam);
            }
            _activeTeam = _teams.Dequeue();
            Debug.Log(_currentTurn + " " + _activeTeam.CMode);
        }
        else
        {
            //invoke match end
        }
        //тут некорректно - ход должен переводиться только после активности всех команд в списке
    }
}
