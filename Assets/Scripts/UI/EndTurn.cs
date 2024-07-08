using UnityEngine;

public class EndTurn : MonoBehaviour
{
    public void TurnOff()
    {
        if(TeamTurnManager.Instance.ActiveTeam.CMode == TeamsInitializer.Team.ControlMode.Player)
        {
            TeamTurnManager.Instance.StartNewSubTurn();
        }
        else
        {
            Debug.Log("ходит другая команда");
        }
    }
}
