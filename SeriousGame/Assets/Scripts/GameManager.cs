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

    private StateMachine _stateMachine;

    public enum Turns {
        IDLE,
        PLAYER_ATTACK,
        PLAYER_DEFEND,
        ENEMY_ATTACK,
        ENEMY_DEFEND
    }

    private Turns _current_turn;

    private Vector3 _playerFinalVector;    
    private Vector3 _playerPointA;
    private Vector3 _playerPointB;

    private Vector3 _enemyFinalVector;
    private Vector3 _enemyDefensePointA;
    private Vector3 _enemyDefensePointB;
    private Vector3 _enemyAttackPointA;
    private Vector3 _enemyAttackPointB;

    private Vector3 _playerThrustPoint;

    private Transform _enemyArea;
    private Transform _playerArea;

    private PlayerManager.BodyPart _playerBodyPart;
    private string _enemyBodyPart;

    private void Awake() {
        _mainCamera = Camera.main;
        _current_turn = Turns.IDLE;
        _stateMachine = new StateMachine(this);
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();

        /*Debug.Log(_current_turn);
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
                EnemyFinishedDefending();

                _current_turn = Turns.ENEMY_ATTACK;
                _enemy.enemyFinished = false;
                _enemy.Attack();
            }
        }*/
    }

    public void PlayerAttack()
    {
        _player.PlayerAttackTurn();
        _enemy.PlayerTurn();
    }
    public void PlayerDefend()
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
    public void EnemyAttack()
    {
        _player.EnemyTurn();
        _enemy.EnemyAttackTurn();

        if (_enemy.enemyFinished)
        {
            _current_turn = Turns.PLAYER_DEFEND;
            _enemy.enemyFinished = false;
        }
    }
    public void EnemyDefend()
    {
        _player.EnemyTurn();
        _enemy.EnemyDefendTurn();

        if (_enemy.enemyFinished)
        {
            EnemyFinishedDefending();

            _current_turn = Turns.ENEMY_ATTACK;
            _enemy.enemyFinished = false;
            _enemy.Attack();
        }
    }

    public Turns GetCurrentTurn()
    {
        return _current_turn;
    }

    public void CheckHit()
    {
        var ray = Physics2D.Raycast(_enemyAttackPointA, _enemyAttackPointB, 50.0f, LayerMask.GetMask("PlayerBody"));
        if(ray.collider == null)
        {
            Debug.Log("Didn't hit anything");
        }
        else
        {
            Debug.Log("Hit -> true");
        }
    }

    public void PlayerFinishedDefending()
    {
        Debug.Log("Player finished defending");
        Debug.Log("current player move: " + _inputHandler.current_move);
        if (_inputHandler.current_move == InputHandler.Moves.PARRY)
        {           
            //_playerFinalVector = _player.GetCalculatedVector();
            _playerPointA = _player.GetFirstPoint();
            _playerPointB = _player.GetSecondPoint();

            //_enemyFinalVector = _enemy.GetAttackVector();
            _enemyAttackPointA = _enemy.GetAttackPointA();
            _enemyAttackPointB = _enemy.GetAttackPointB();

            //check intersection
            if (checkIntersection(_playerPointA, _playerPointB, _enemyAttackPointA, _enemyAttackPointB))
            {
                //it was true, so player got blocked -> play sound of parrying
                Debug.Log("Parry -> true");

            }
            else
            {
                CheckHit();
                Debug.Log("Parry -> false");
            }

            /*else if () //hits collider
            {
                //it was true, so player hits enemy -> play sound of hit and take damage
                Debug.Log("Hit -> true");     
            
            }
            else //then missed collider
            {
                //missed enemy -> play sound of missing
                Debug.Log("Hit -> false");
            }*/
        }
        else if (_inputHandler.current_move == InputHandler.Moves.THRUST_DEFENSE)
        {
            //get enemy point and player area
            _playerArea = _player.GetBodyPart();
            _playerBodyPart = _player.GetCurrentBodyPart();

            _enemyFinalVector = _enemy.GetAttackVector();
            _enemyArea = _enemy.GetEnemyArea();
            _enemyBodyPart = _enemyArea.transform.name;

            if (checkPointInsideArea(_playerArea, _enemyFinalVector, _playerBodyPart, _enemyBodyPart))
            {
                Debug.Log("Parry -> true");
            }
            else
            {
                CheckHit();
                Debug.Log("Parry -> false");
            }
            /*else if () //hits collider
            {
                Debug.Log("Hit -> true");
            }
            else if () //then missed collider
            {
                Debug.Log("Hit -> false");
            }*/
            
            //_player.HidePoints();
        }
        else if (_inputHandler.current_move == InputHandler.Moves.DODGE)
        {
            Debug.Log("dodge");
            CheckHit();
            /*if () //hits collider
            {
                //it was true, so player hits enemy -> play sound of hit and take damage
                Debug.Log("Hit -> true");
            }
            else //then missed collider
            {
                //missed enemy -> play sound of missing
                Debug.Log("Hit -> false");
            }*/
            //tell player to reset collider to zero
            _player.ResetCollider();
        }
        _current_turn = Turns.PLAYER_ATTACK;
    }

    public void EnemyFinishedDefending()
    {
        Debug.Log("Enemy finished defending");
        if (_enemy.current_move == EnemyManager.Moves.PARRY)
        {
            //_enemyFinalVector = _enemy.GetDefendVector();
            _enemyDefensePointA = _enemy.GetDefensePointA();
            _enemyDefensePointB = _enemy.GetDefensePointB();
            
            //_playerFinalVector = _player.GetCalculatedVector();
            _playerPointA = _player.GetFirstPoint();
            _playerPointB = _player.GetSecondPoint();

            //check intersection
            if (checkIntersection(_playerPointA, _playerPointB, _enemyDefensePointA, _enemyDefensePointB))
            {
                //it was true, so player got blocked -> play sound of parrying
                Debug.Log("Parry -> true");
            }
            else
            {
                Debug.Log("Parry -> false");
            }
            /*else if () //hits collider
            {
                //it was true, so player hits enemy -> play sound of hit and take damage
                Debug.Log("Hit -> true");
            }
            else //then missed collider
            {
                //missed enemy -> play sound of missing
                Debug.Log("Hit -> false");
            }*/
        }
        else if (_enemy.current_move == EnemyManager.Moves.DEFEND_THRUST)
        {
            //get player point and enemy area
            _playerThrustPoint = _player.GetThrustPoint();
            _playerBodyPart = _player.GetCurrentBodyPart();

            _enemyArea = _enemy.GetEnemyArea();
            _enemyBodyPart = _enemyArea.transform.name;

            if (checkPointInsideArea(_enemyArea, _playerThrustPoint, _playerBodyPart, _enemyBodyPart))
            {
                Debug.Log("Parry -> true");
            }
            else
            {
                Debug.Log("Parry -> false");
            }
            /*else if () //hits collider
            {
                Debug.Log("Hit -> true");
            }
            else if () //then missed collider
            {
                Debug.Log("Hit -> false");
            }*/
        }
        else if (_enemy.current_move == EnemyManager.Moves.DODGE)
        {
            Debug.Log("enemy dodge");
            /*if () //hits collider
            {
                //it was true, so player hits enemy -> play sound of hit and take damage
                Debug.Log("Hit -> true");
            }
            else //then missed collider
            {
                //missed enemy -> play sound of missing
                Debug.Log("Hit -> false");
            }*/

        }
    }

    public static bool checkIntersection(Vector3 playerPointA, Vector3 playerPointB, Vector3 enemyPointA, Vector3 enemyPointB)
    {
        // Parametric vectors (directions) in the x-y plane
        Vector3 playerDirection = playerPointB - playerPointA; // Player vector's direction
        Vector3 enemyDirection = enemyPointB - enemyPointA; // Enemy's vector direction

        Vector3 intersectionPoint;

        // Denominator (cross product of the directions)
        float denominator = playerDirection.x * enemyDirection.y - playerDirection.y * enemyDirection.x;

        // If the denominator is 0, the vectors are parallel and will not intersect.
        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            intersectionPoint = Vector3.zero;
            return false;
        }

        // Find the intersection parameter (t and s)
        float t = ((enemyPointA.x - playerPointA.x) * enemyDirection.y - (enemyPointA.y - playerPointA.y) * enemyDirection.x) / denominator;

        // If t is outside the range [0, 1], the intersection point is outside the vector range.
        if (t < 0 || t > 1)
        {
            intersectionPoint = Vector3.zero;
            return false;
        }

        // Calculate the intersection point
        intersectionPoint = playerPointA + t * playerDirection;

        // Check if the intersection is within the bounds of the second vector as well (enemy vector).
        float s = ((enemyPointA.x - playerPointA.x) * playerDirection.y - (enemyPointA.y - playerPointA.y) * playerDirection.x) / denominator;

        if (s < 0 || s > 1)
        {
            intersectionPoint = Vector3.zero;
            return false;
        }

        // The intersection point is within the bounds of both vectors
        return true;
    }

    public static bool checkPointInsideArea(Transform area, Vector3 point, PlayerManager.BodyPart playerBodyPart, string enemyBodyPart)
    {
        string bodyName = area.name;
        Collider2D collider = Physics2D.OverlapPoint(point, LayerMask.GetMask("Body"));
        if(collider == null)
        {
            return false;
        }
        else if (collider.gameObject.name == bodyName)
        {
            return true;
        }
        else
        {
            return false;
        }
        /*//do different body parts cause of different collider formats
        if (playerBodyPart == PlayerManager.BodyPart.HEAD || playerBodyPart == PlayerManager.BodyPart.TORSO || enemyBodyPart.Equals("HEAD") || enemyBodyPart.Equals("TORSO"))
        {
            // Get the local position of the point relative to the transform
            Vector3 localPoint = area.InverseTransformPoint(point);

            // Get the half extents of the transform based on its scale
            Vector3 halfExtents = area.localScale / 2.0f;

            // Check if the local point is within the bounds of the transform
            return Mathf.Abs(localPoint.x) <= halfExtents.x &&
                   Mathf.Abs(localPoint.y) <= halfExtents.y &&
                   Mathf.Abs(localPoint.z) <= halfExtents.z;
            
        }
        else if (playerBodyPart == PlayerManager.BodyPart.LEFT_ARM || playerBodyPart == PlayerManager.BodyPart.RIGHT_ARM || playerBodyPart == PlayerManager.BodyPart.LEFT_LEG || playerBodyPart == PlayerManager.BodyPart.RIGHT_LEG
            || enemyBodyPart.Equals("LEFT_ARM") || enemyBodyPart.Equals("RIGHT_ARM") || enemyBodyPart.Equals("LEFT_LEG") || enemyBodyPart.Equals("RIGHT_LEG"))
        {
            // Assume the CapsuleCollider2D is attached to the same GameObject as the Transform
            CapsuleCollider2D capsuleCollider = area.GetComponent<CapsuleCollider2D>();

            // Get the capsule properties
            Vector2 center = capsuleCollider.offset;
            float width = capsuleCollider.size.x * area.localScale.x;
            float height = capsuleCollider.size.y * area.localScale.y;
            bool isHorizontal = capsuleCollider.direction == CapsuleDirection2D.Horizontal;

            // Convert the point to the capsule's local space
            Vector2 localPoint = area.InverseTransformPoint(point);

            // Adjust the center based on the offset
            localPoint -= center;

            // Calculate the radius of the capsule's circular ends
            float radius = Mathf.Min(width, height) / 2f;

            // Check if the point is inside the capsule's bounds
            if (isHorizontal)
            {
                // Horizontal capsule
                float rectWidth = width - 2 * radius;

                // Check against the central rectangle
                if (Mathf.Abs(localPoint.x) <= rectWidth / 2f && Mathf.Abs(localPoint.y) <= radius)
                    return true;

                // Check against the circular ends
                Vector2 leftCircleCenter = new Vector2(-rectWidth / 2f, 0f);
                Vector2 rightCircleCenter = new Vector2(rectWidth / 2f, 0f);

                return (localPoint - leftCircleCenter).sqrMagnitude <= radius * radius ||
                       (localPoint - rightCircleCenter).sqrMagnitude <= radius * radius;
            }
            else
            {
                // Vertical capsule
                float rectHeight = height - 2 * radius;

                // Check against the central rectangle
                if (Mathf.Abs(localPoint.y) <= rectHeight / 2f && Mathf.Abs(localPoint.x) <= radius)
                    return true;

                // Check against the circular ends
                Vector2 topCircleCenter = new Vector2(0f, rectHeight / 2f);
                Vector2 bottomCircleCenter = new Vector2(0f, -rectHeight / 2f);

                return (localPoint - topCircleCenter).sqrMagnitude <= radius * radius ||
                       (localPoint - bottomCircleCenter).sqrMagnitude <= radius * radius;
            }
        }
        return false;*/
    }

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
        Debug.Log("Player Attacked");
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
        
        PlayerFinishedDefending();
    }
    public void GetBodyPart(InputHandler.BodyPart bodyPart)
    {
        if (bodyPart == InputHandler.BodyPart.HEAD)
        {
            _player.SetBodyPart("HEAD");
        }
        else if (bodyPart == InputHandler.BodyPart.TORSO)
        {
            _player.SetBodyPart("TORSO");
        }
        else if (bodyPart == InputHandler.BodyPart.LEFT_ARM)
        {
            _player.SetBodyPart("LEFT_ARM");
        }
        else if (bodyPart == InputHandler.BodyPart.RIGHT_ARM)
        {
            _player.SetBodyPart("RIGHT_ARM");
        }
        else if (bodyPart == InputHandler.BodyPart.LEFT_LEG)
        {
            _player.SetBodyPart("LEFT_LEG");
        }
        else if (bodyPart == InputHandler.BodyPart.RIGHT_LEG)
        {
            _player.SetBodyPart("RIGHT_LEG");
        }

        PlayerFinishedDefending();
    }

    public void StartGame()
    {
        _current_turn = Turns.PLAYER_ATTACK;
    }

    //func to reset game and be called on yes button from exit menu
    public void ResetGame(){
        
    }
}