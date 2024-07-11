using UnityEngine;
using UnityEngine.UI;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private bool _canTurnAny;
    [SerializeField] private Text _textCount;
    [SerializeField] private Text _teamName;

    public void Start()
    {
        _textCount.text = TeamTurnManager.Instance.CurrentTurn.ToString();
        if(TeamTurnManager.Instance.ActiveTeam != null)
        {
            _teamName.text = TeamTurnManager.Instance.ActiveTeam.CMode.ToString();
        }        
        TeamTurnManager.Instance.OnTurnChanged += TurnChanged;
        TeamTurnManager.Instance.OnTeamChanged += TeamChanged;
    }

    public void TurnOff()
    {
        if(TeamTurnManager.Instance.ActiveTeam.CMode == TeamsInitializer.Team.ControlMode.Player
            || _canTurnAny)
        {
            TeamTurnManager.Instance.StartNewSubTurn();
        }
        else
        {
            Debug.Log("ходит другая команда");
        }
    }

    private void TurnChanged(int value)
    {
        _textCount.text = value.ToString();
    }

    private void TeamChanged(string name)
    {
        _teamName.text = name;
    }

    private void OnDisable()
    {
        TeamTurnManager.Instance.OnTurnChanged -= TurnChanged;
        TeamTurnManager.Instance.OnTeamChanged -= TeamChanged;
    }
}
