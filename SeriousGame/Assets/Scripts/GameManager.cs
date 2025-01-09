using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _player;

    [SerializeField] private UIManager _ui;

    [SerializeField] private EnemyManager _enemy;

    [SerializeField] private InputHandler _inputHandler;

    private Camera _mainCamera;

    public enum Turns {
        IDLE,
        PLAYER_ATTACK,
        PLAYER_DEFEND,
        ENEMY_ATTACK,
        ENEMY_DEFEND
    }

    private Turns _current_turn;

    private Vector3 _playerFinalVector;
    private Vector3 _enemyFinalVector;
    private Vector3 _playerThrustPoint;
    private Transform _enemyArea;
    private Transform _playerArea;

    private void Awake() {
        _mainCamera = Camera.main;
        _current_turn = Turns.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        if (_current_turn == Turns.PLAYER_ATTACK)
        {
            _player.PlayerAttackTurn();
            _enemy.PlayerTurn();
        }
        else if (_current_turn == Turns.PLAYER_DEFEND)
        {
            if (_enemy.current_move == EnemyManager.Moves.SLASH)
            {
                _player.PlayerDefendTurn("PARRY");
            }
            else if (_enemy.current_move == EnemyManager.Moves.THRUST)
            {
                _player.PlayerDefendTurn("DEFEND_THRUST");
            }
            _enemy.PlayerTurn();
        }
        else if (_current_turn == Turns.ENEMY_ATTACK)
        {
            _player.EnemyTurn();
            _enemy.EnemyAttackTurn();

            if (_enemy.enemyFinished)
            {
                _current_turn = Turns.PLAYER_DEFEND;
                _enemy.enemyFinished = false;
            }
        }
        else if (_current_turn == Turns.ENEMY_DEFEND)
        {
            _player.EnemyTurn();
            _enemy.EnemyDefendTurn();

            if (_enemy.enemyFinished)
            {
                //EnemyFinishedDefending();

                _current_turn = Turns.ENEMY_ATTACK;
                _enemy.enemyFinished = false;
            }
        }
    }

    /*public void PlayerFinishedDefending()
    {
        if (_inputHandler.current_move == InputHandler.Moves.PARRY)
        {           
            _playerFinalVector = _player.GetCalculatedVector();
            _enemyFinalVector = _enemy.GetAttackVector();

            //check intersection
            if (checkIntersection(_enemyFinalVector, _playerFinalVector))
            {
                //it was true, so player got blocked -> play sound of parrying
                Debug.Log("Parry -> true");
            }
            else if () //hits collider
            {
                //it was true, so player hits enemy -> play sound of hit and take damage
                Debug.Log("Hit -> true");
            }
            else //then missed collider
            {
                //missed enemy -> play sound of missing
                Debug.Log("Hit -> false");
            }
        }
        else if (_inputHandler.current_move == InputHandler.Moves.THRUST)
        {
            //get enemy point and player area
            _playerArea = _player.GetBodyPart();
            _enemyFinalVector = _enemy.GetAttackVector();

            if (checkPointInsideArea(_playerArea, _enemyFinalVector))
            {
                Debug.Log("Parry -> true");
            }
            else if () //hits collider
            {
                Debug.Log("Hit -> true");
            }
            else if () //then missed collider
            {
                Debug.Log("Hit -> false");
            }
            
            _player.HidePoints();
        }
        else if (_inputHandler.current_move == InputHandler.Moves.DODGE)
        {
            if () //hits collider
            {
                //it was true, so player hits enemy -> play sound of hit and take damage
                Debug.Log("Hit -> true");
            }
            else //then missed collider
            {
                //missed enemy -> play sound of missing
                Debug.Log("Hit -> false");
            }
            //tell player to reset collider to zero
            resetCollider();
        }
        _current_turn = Turns.PLAYER_ATTACK;
    }

    public void EnemyFinishedDefending()
    {
        if (_enemy.current_move == EnemyManager.Moves.PARRY)
        {
            _enemyFinalVector = _enemy.GetDefendVector();
            _playerFinalVector = _player.GetCalculatedVector();

            //check intersection
            if (checkIntersection(_enemyFinalVector, _playerFinalVector))
            {
                //it was true, so player got blocked -> play sound of parrying
                Debug.Log("Parry -> true");
            }
            else if () //hits collider
            {
                //it was true, so player hits enemy -> play sound of hit and take damage
                Debug.Log("Hit -> true");
            }
            else //then missed collider
            {
                //missed enemy -> play sound of missing
                Debug.Log("Hit -> false");
            }
        }
        else if (_enemy.current_move == EnemyManager.Moves.DEFEND_THRUST)
        {
            //get player point and enemy area
            _playerThrustPoint = _player.GetThrustPoint();
            _enemyArea = _enemy.GetEnemyArea();

            if (checkPointInsideArea(_enemyArea, _playerThrustpoint))
            {
                Debug.Log("Parry -> true");
            }
            else if () //hits collider
            {
                Debug.Log("Hit -> true");
            }
            else if () //then missed collider
            {
                Debug.Log("Hit -> false");
            }
        }
        else if (_enemy.current_move == EnemyManager.Moves.DODGE)
        {
            if () //hits collider
            {
                //it was true, so player hits enemy -> play sound of hit and take damage
                Debug.Log("Hit -> true");
            }
            else //then missed collider
            {
                //missed enemy -> play sound of missing
                Debug.Log("Hit -> false");
            }

        }
    }

    public bool checkIntersection(Vector3 playerVector, Vector3 enemyVector)
    {

    }

    public bool checkPointInsideArea(Transform area, Vector3 point)
    {
        //do different body parts cause of different formats
        /if (head or torso) GetCurrentBodyPart()
        {
             
        }
        else if (rest of body parts)
        {
        
        }/
    }*/

    public void showSmallCircumferenceAndSaveFirstPoint() {
        Vector3 mouseFirstPosition = Mouse.current.position.ReadValue();
        mouseFirstPosition.z = _mainCamera.nearClipPlane;
        var worldFirstPosition = _mainCamera.ScreenToWorldPoint(mouseFirstPosition);

        //Call func from PlayerManager
        _player.SetFirstPoint(worldFirstPosition);

        //Call func from UIManager
        _ui.InstanceSmallCircumference(worldFirstPosition);
    }

    public void eraseSmallCircumferenceAndSaveSecondPoint() {
        Vector3 mouseSecondPosition = Mouse.current.position.ReadValue();
        mouseSecondPosition.z = _mainCamera.nearClipPlane;
        var worldSecondPosition = _mainCamera.ScreenToWorldPoint(mouseSecondPosition);

        //Call func from PlayerManager    
        _player.SetSecondPoint(worldSecondPosition);

        _player.calculateVector();

        //Call func from UIManager
        DeleteSmallCircumference();
    }

    public void SendDirectionToEnemy()
    {
        _current_turn = Turns.ENEMY_DEFEND;
        _enemy.GetPlayerDirection(_player.TransformVectorToDirectionEnum());
    }

    public void Defended(){
        //check if hit
        //PlayerFinishedDefending();
    }

    public void DeleteSmallCircumference(){
        _ui.DeleteSmallCircumference();
    }

    public void ChoosePlayerThrustPoint()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = _mainCamera.nearClipPlane;
        var thrustPoint = _mainCamera.ScreenToWorldPoint(mousePosition);

        _player.SetThrustPoint(thrustPoint);

        _current_turn = Turns.ENEMY_DEFEND;
        _enemy.PlayerThrust();
    }

    public void GetDodgeDirection(InputHandler.Dodge dodgeDirection)
    {
        if (dodgeDirection.Equals("UP"))
        {
            _player.DodgeDirection("UP");
        }
        else if (dodgeDirection.Equals("DOWN"))
        {
            _player.DodgeDirection("DOWN");
        }
        else if (dodgeDirection.Equals("LEFT"))
        {
            _player.DodgeDirection("LEFT");
        }
        else if (dodgeDirection.Equals("RIGHT"))
        {
            _player.DodgeDirection("RIGHT");
        }
    }
    public void GetBodyPart(InputHandler.BodyPart bodyPart)
    {
        if (bodyPart.Equals("HEAD"))
        {
            _player.SetBodyPart("HEAD");
        }
        else if (bodyPart.Equals("TORSO"))
        {
            _player.SetBodyPart("TORSO");
        }
        else if (bodyPart.Equals("LEFT_ARM"))
        {
            _player.SetBodyPart("LEFT_ARM");
        }
        else if (bodyPart.Equals("RIGHT_ARM"))
        {
            _player.SetBodyPart("RIGHT_ARM");
        }
        else if (bodyPart.Equals("LEFT_LEG"))
        {
            _player.SetBodyPart("LEFT_LEG");
        }
        else if (bodyPart.Equals("RIGHT_LEG"))
        {
            _player.SetBodyPart("RIGHT_LEG");
        }
    }

    public void StartGame()
    {
        _current_turn = Turns.PLAYER_ATTACK;
    }

    //func to reset game and be called on yes button from exit menu
    public void ResetGame(){
        
    }
}