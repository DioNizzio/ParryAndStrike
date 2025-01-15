using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private GameManager.Turns _current_turn;

    private GameManager _gameManager;
    
    public StateMachine(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void Update()
    {
        var _newTransition = CheckTransition();
        if (_newTransition != _current_turn)
        {
            ChangeState(_newTransition);
        }
        UpdateState();
    }

    private void ChangeState(GameManager.Turns newState)
    {
        ExitState();
        _current_turn = newState;
        EnterState();
    }

    private void EnterState()
    {
        switch (_current_turn)
        {
            case GameManager.Turns.IDLE:
                break;
            case GameManager.Turns.PLAYER_ATTACK:                
                _gameManager.PlayerAttack();
                break;
            case GameManager.Turns.PLAYER_DEFEND:
                _gameManager.PlayerDefend();
                break;
            case GameManager.Turns.ENEMY_ATTACK:
                _gameManager.EnemyAttack();
                break;
            case GameManager.Turns.ENEMY_DEFEND:
                _gameManager.EnemyDefend();
                break;
        }
    }

    private GameManager.Turns CheckTransition()
    {
        return _gameManager.GetCurrentTurn();
    }

    private void UpdateState()
    {

    }

    private void ExitState()
    {

    }
}
