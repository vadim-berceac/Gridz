using UnityEngine;

public class EndTurn : MonoBehaviour
{
    [SerializeField] private bool _canTurnAny;
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
}
