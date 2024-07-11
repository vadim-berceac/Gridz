using System.Collections.Generic;
using UnityEngine;
using static TeamsInitializer;

public class TeamTurnManager : MonoBehaviour
{
    [SerializeField][Range(1, 48)] private int _maxTurnCount;
    private int _currentTurn = 1;
    private int _currentSubTurn = 0;
    private Team _activeTeam;
    private Queue<Team> _teamQueue;
    private static TeamTurnManager _instance;

    public int CurrentTurn => _currentTurn;
    public Team ActiveTeam => _activeTeam;
    public Queue<Team> TeamQueue => _teamQueue;
    public static TeamTurnManager Instance => _instance;

    private void Start()
    {
        Initialize();
        StartNewSubTurn();
    }

    private void Initialize()
    {
        CreateQueue(ref _teamQueue);
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
        _currentSubTurn++;
        if (_currentSubTurn > 4)
        {
            _currentSubTurn = 1;
            _currentTurn++;
        }

        if (_currentTurn >= _maxTurnCount + 1)
        {
            //invoke match end
            return;
        }

        if (_activeTeam != null)
        {
            _teamQueue.Enqueue(_activeTeam);
        }
        _activeTeam = _teamQueue.Dequeue();
        Debug.Log(_currentTurn + " " + _currentSubTurn + " "+ _activeTeam.CMode);
    }
}
