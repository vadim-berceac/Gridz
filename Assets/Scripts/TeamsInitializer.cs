using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamsInitializer : MonoBehaviour
{
    [Serializable]
    public class Team
    {
        public Team(ControlMode controlMode) 
        {
            _controlMode = controlMode;
        }
        public enum ControlMode
        {
            Player,
            AIHostile,
            AINeutral,
            AIFreindly
        }
        private readonly ControlMode _controlMode;
        [SerializeField] private List<UnitFSM> _unitsOnTeam = new();

        public ControlMode CMode =>_controlMode;
        public List<UnitFSM> UnitsOnTeam => _unitsOnTeam;
    }

    private List<UnitFSM> _unitsOnScene = new();    
    [SerializeField] private List<Team> _teams = new(); // убрать потом SerializeField
    private static TeamsInitializer _instance;

    public List<UnitFSM> UnitOnScene => _unitsOnScene;
    public List<Team> Teams => _teams;
    public static TeamsInitializer Instance => _instance;

    private void Awake()
    {        
        Initialize();
    }

    private void Initialize()
    {
        FindAllUnits(ref _unitsOnScene);
        CreateTeams(ref _teams);
        SortPlayersByControlType(_unitsOnScene, _teams);
        _instance = this;
    }

    private void FindAllUnits(ref List<UnitFSM> list)
    {
        list = FindObjectsByType<UnitFSM>(FindObjectsSortMode.None).ToList();
    }

    private void CreateTeams(ref List<Team> teams)
    {
        teams.Add(new Team(Team.ControlMode.Player));
        teams.Add(new Team(Team.ControlMode.AIHostile));
        teams.Add(new Team(Team.ControlMode.AINeutral));
        teams.Add(new Team(Team.ControlMode.AIFreindly));
    }

    private void SortPlayersByControlType(List<UnitFSM> allUnits, List<Team> teams)
    {
        if(teams.Count < Enum.GetValues(typeof(Team.ControlMode)).Length)
        {
            Debug.LogWarning("Not all teams are initialized!");
            return;
        }

        foreach (var unit in allUnits)
        {
            if (unit.CMode == UnitFSM.ControlMode.Player)
            {
                teams[0].UnitsOnTeam.Add(unit);
                continue;
            }
            if (unit.CMode == UnitFSM.ControlMode.AIHostile)
            {
                teams[1].UnitsOnTeam.Add(unit);
                continue;
            }
            if (unit.CMode == UnitFSM.ControlMode.AINeutral)
            {
                teams[2].UnitsOnTeam.Add(unit);
                continue;
            }
            if (unit.CMode == UnitFSM.ControlMode.AIFreindly)
            {
                teams[3].UnitsOnTeam.Add(unit);
            }
        }
    }
}

