using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _player;

    [SerializeField] private GameObject _playerCollider;
    [SerializeField] private GameObject _enemyCollider;


    [SerializeField] private UIManager _ui;
    [SerializeField] private GameObject _attackMenuUI;
    [SerializeField] private GameObject _defendMenuUI;
    [SerializeField] private GameObject _thurstDefenseMenuUI;
    [SerializeField] private GameObject _arrowsMenuUI;
    [SerializeField] private GameObject _enemyPointsText;
    [SerializeField] private GameObject _playerPointsText;
    [SerializeField] private GameObject _timeToAttackText;
    [SerializeField] private GameObject _playerWinsMenu;
    [SerializeField] private GameObject _enemyWinsMenu;
    [SerializeField] private GameObject _playerSword;
    [SerializeField] private GameObject _playerSwordThrust;
    [SerializeField] private GameObject _playerSwordDefend;
    [SerializeField] private GameObject _playerSwordAttack1;
    [SerializeField] private GameObject _playerSwordAttack2;

    [SerializeField] private EnemyManager _enemy;

    [SerializeField] private InputHandler _inputHandler;

    [SerializeField] private AudioSource _swordHit;
    [SerializeField] private AudioSource _swordMiss;
    [SerializeField] private AudioSource _swordUnseathe;
    [SerializeField] private AudioSource _swordsHit;

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

    private int _enemyPoints;
    private int _playerPoints;

    private bool isWaiting = false;
    private float timer = 5f;

    private Coroutine _coroutineWait2Seconds;
    private Coroutine _coroutineEnemyWait2Seconds;
    private Coroutine _waitCoroutine;

    private void Awake() {
        _mainCamera = Camera.main;
        _current_turn = Turns.IDLE;
        _stateMachine = new StateMachine(this);
        _enemyPoints = 0;
        _playerPoints = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
        if (isWaiting)
        {
            timer -= Time.deltaTime;
            _timeToAttackText.GetComponent<TMP_Text>().text = "Time left to attack: " + Mathf.RoundToInt(timer);
        }
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
    }

    public Turns GetCurrentTurn()
    {
        return _current_turn;
    }

    public IEnumerator TimeForPlayerToDefend()
    {
        isWaiting = true;
        _timeToAttackText.SetActive(true);
        yield return new WaitForSeconds(5f);
        isWaiting = false;
        _timeToAttackText.SetActive(false);
        timer = 5f;
        _enemy.ChangeToAttackSprite();
        if(_enemy.current_move == EnemyManager.Moves.SLASH)
        {
            _enemyAttackPointA = _enemy.GetAttackPointA();
            _enemyAttackPointB = _enemy.GetAttackPointB();
            CheckIfPlayerWasHit();
        }
        else if (_enemy.current_move == EnemyManager.Moves.THRUST)
        {
            _enemyFinalVector = _enemy.GetAttackVector();
            CheckIfPlayerWasHitWithThrust();
        }

        _inputHandler.current_move = InputHandler.Moves.IDLE;
        _defendMenuUI.SetActive(false);
        if(_inputHandler.current_state == InputHandler.State.SMALLCIRCUMFERENCE)
        {
            DeleteSmallCircumference();
        }
        _arrowsMenuUI.SetActive(false);
        _thurstDefenseMenuUI.SetActive(false);
        _attackMenuUI.SetActive(false);
        yield return new WaitForSeconds(2f);
        _attackMenuUI.SetActive(true);
        _enemy.ChangeSpriteToIdle();
        _current_turn = Turns.PLAYER_ATTACK;
    }

    public IEnumerator Wait2Seconds()
    {
        yield return new WaitForSeconds(2f);
        _enemy.ChangeSpriteToIdle();
        _player.ResetCollider();
        _playerSword.SetActive(true);
        _playerSwordDefend.SetActive(false);
    }
    public IEnumerator EnemyWait2Seconds()
    {
        yield return new WaitForSeconds(2f);        
        //_enemy.ChangeSpriteToIdle();

        if (_inputHandler.current_move == InputHandler.Moves.THRUST)
        {
            _playerSwordThrust.SetActive(false);
            _playerSword.SetActive(true);
        }
        else if (_inputHandler.current_move == InputHandler.Moves.IDLE || _inputHandler.current_move == InputHandler.Moves.SLASH)
        {
            _playerSwordThrust.SetActive(false);
            _playerSwordAttack2.SetActive(false);
            _playerSword.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        _enemy.ChangeSpriteToIdle();
        _current_turn = Turns.ENEMY_ATTACK;
        //_enemy.Attack();
    }

    public void CheckIfPlayerWasHit()
    {
        var ray = Physics2D.Raycast(_enemyAttackPointA, _enemyAttackPointB, 50.0f, LayerMask.GetMask("PlayerBody"));
        if(ray.collider == null)
        {
            PlayMissSound();
        }
        else
        {
            PlayHitSound();
            _enemyPoints += 1;
            _enemyPointsText.transform.GetComponent<TMP_Text>().text = "Enemy Points: " + _enemyPoints;
            if (_enemyPoints == 12)
            {
                EnemyWins();
            }
        }
    }
    public void CheckIfPlayerWasHitWithThrust()
    {
        var ray = Physics2D.Raycast(_mainCamera.transform.position, _enemyFinalVector, 50.0f, LayerMask.GetMask("PlayerBody"));
        //var ray = Physics2D.OverlapCapsule(new Vector2(_enemyFinalVector.x, _enemyFinalVector.y), _playerCollider.GetComponent<CapsuleCollider2D>().size, _playerCollider.GetComponent<CapsuleCollider2D>().direction, 0f, LayerMask.GetMask("PlayerBody"));
        if (ray.collider == null)
        {
            PlayMissSound();
        }
        else
        {        
            Debug.Log(ray.collider.name);

            PlayHitSound();
            _enemyPoints += 1;
            _enemyPointsText.transform.GetComponent<TMP_Text>().text = "Enemy Points: " + _enemyPoints;
            if (_enemyPoints == 12)
            {
                EnemyWins();
            }
        }
    }
    public void CheckIfEnemyWasHit()
    {
        var ray = Physics2D.Raycast(_playerPointA, _playerPointB, 50.0f, LayerMask.GetMask("EnemyBody"));
        if(ray.collider == null)
        {
            PlayMissSound();
        }
        else
        {
            PlayHitSound();
            _playerPoints += 1;
            _playerPointsText.transform.GetComponent<TMP_Text>().text = "Player Points: " + _playerPoints;
            if (_playerPoints == 12)
            {
                PlayerWins();
            }
        }
    }
    public void CheckIfEnemyWasHitWithThrust()
    {
        var ray = Physics2D.Raycast(_mainCamera.transform.position, _playerThrustPoint, 50.0f, LayerMask.GetMask("EnemyBody"));
        //var ray = Physics2D.OverlapCapsule(new Vector2(_playerThrustPoint.x, _playerThrustPoint.y), _enemyCollider.GetComponent<CapsuleCollider2D>().size, _enemyCollider.GetComponent<CapsuleCollider2D>().direction, 3f, LayerMask.GetMask("EnemyBody"));
        if(ray.collider == null)
        {
            PlayMissSound();
        }
        else
        {
            Debug.Log(ray.collider.name);

            PlayHitSound();
            _playerPoints += 1;
            _playerPointsText.transform.GetComponent<TMP_Text>().text = "Player Points: " + _playerPoints;
            if (_playerPoints == 12)
            {
                PlayerWins();
            }
        }
    }

    public void PlayerFinishedDefending()
    {
        _enemy.StopCoroutine(_enemy.coroutine);
        if (isWaiting)
        {
            isWaiting = false;
            _timeToAttackText.SetActive(false);
            timer = 5f;
        }
        
        //Change to enemy attack sprite
        _enemy.ChangeToAttackSprite();
        if (_inputHandler.current_move != InputHandler.Moves.DODGE)
        {
            _playerSwordDefend.SetActive(true);
            _playerSword.SetActive(false);
        }

        if (_inputHandler.current_move == InputHandler.Moves.PARRY)
        {           
            _playerPointA = _player.GetFirstPoint();
            _playerPointB = _player.GetSecondPoint();

            _enemyAttackPointA = _enemy.GetAttackPointA();
            _enemyAttackPointB = _enemy.GetAttackPointB();

            //check intersection
            if (checkIntersection(_playerPointA, _playerPointB, _enemyAttackPointA, _enemyAttackPointB))
            {
                //it was true, so player got blocked -> play sound of parrying
                PlaySwordsHitSound();
            }
            else
            {
                CheckIfPlayerWasHit();
            }
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
                PlaySwordsHitSound();
            }
            else
            {
                CheckIfPlayerWasHitWithThrust();
            }
        }
        else if (_inputHandler.current_move == InputHandler.Moves.DODGE)
        {
            if (_enemy.current_move == EnemyManager.Moves.SLASH)
            {
                _enemyAttackPointA = _enemy.GetAttackPointA();
                _enemyAttackPointB = _enemy.GetAttackPointB();

                CheckIfPlayerWasHit();
            }
            else if (_enemy.current_move == EnemyManager.Moves.THRUST)
            {
                _enemyFinalVector = _enemy.GetAttackVector();
                CheckIfPlayerWasHitWithThrust();
            }
            else
            {
                Debug.LogError("Enemy had different current move than slash or thrust");
            }
        }
        _coroutineWait2Seconds = StartCoroutine(Wait2Seconds());
        _current_turn = Turns.PLAYER_ATTACK;
    }

    public void EnemyFinishedDefending()
    {
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
                PlaySwordsHitSound();
            }
            else
            {
                CheckIfEnemyWasHit();
            }
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
                PlaySwordsHitSound();
            }
            else
            {
                CheckIfEnemyWasHitWithThrust();
            }
        }
        else if (_enemy.current_move == EnemyManager.Moves.DODGE)
        {
            if (_inputHandler.current_move == InputHandler.Moves.IDLE || _inputHandler.current_move == InputHandler.Moves.SLASH) //Should be Slash but in InputHandler when the player finishes the slash it changes to IDLE because the player could still click on the big circumference if current_move was still slash
            {
                _playerPointA = _player.GetFirstPoint();
                _playerPointB = _player.GetSecondPoint();
                CheckIfEnemyWasHit();
            }
            else if (_inputHandler.current_move == InputHandler.Moves.THRUST)
            {
                _playerThrustPoint = _player.GetThrustPoint();
                CheckIfEnemyWasHitWithThrust();
            }
            else
            {
                Debug.LogError("Player had different current move than slash or thrust: " + _inputHandler.current_move);
            }
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
        _playerSword.SetActive(false);
        _playerSwordAttack1.SetActive(true);
        StartCoroutine(WaitAndShowSwordAttack2());
        _current_turn = Turns.ENEMY_DEFEND;        
        _enemy.GetPlayerDirection(_player.TransformVectorToDirectionEnum());
    }

    public IEnumerator WaitAndShowSwordAttack2()
    {
        yield return new WaitForSeconds(0.5f);
        _playerSwordAttack1.SetActive(false);
        _playerSwordAttack2.SetActive(true);
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

        _playerSword.SetActive(false);
        _playerSwordThrust.SetActive(true);

        _current_turn = Turns.ENEMY_DEFEND;
        _enemy.PlayerThrust();
    }

    public void GetDodgeDirection(InputHandler.Dodge dodgeDirection)
    {
        if (dodgeDirection == InputHandler.Dodge.UP)
        {
            _player.DodgeDirection("UP");
        }
        else if (dodgeDirection == InputHandler.Dodge.DOWN)
        {
            _player.DodgeDirection("DOWN");
        }
        else if (dodgeDirection == InputHandler.Dodge.LEFT)
        {
            _player.DodgeDirection("LEFT");
        }
        else if (dodgeDirection == InputHandler.Dodge.RIGHT)
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

    public void PlayMissSound()
    {
        _swordMiss.Play();
    }
    public void PlayHitSound()
    {
        _swordHit.Play();
    }
    public void PlaySwordsHitSound()
    {
        _swordsHit.Play();
    }

    public int GetEnemyPoints()
    {
        return _enemyPoints;
    }
    public int GetPlayerPoints()
    {
        return _playerPoints;
    }

    public void StartGame()
    {
        _playerPointsText.GetComponent<TMP_Text>().text = "Player Points: " + _playerPoints;
        _enemyPointsText.GetComponent<TMP_Text>().text = "Enemy Points: " + _enemyPoints;
        _swordUnseathe.Play();
        _current_turn = Turns.PLAYER_ATTACK;
    }

    public void PlayerWins()
    {
        _current_turn = Turns.IDLE;
        _inputHandler.current_move = InputHandler.Moves.IDLE;
        _enemy.current_move = EnemyManager.Moves.IDLE;
        _defendMenuUI.SetActive(false);
        _attackMenuUI.SetActive(false);
        _playerWinsMenu.SetActive(true);
        _playerPointsText.SetActive(false);
        _enemyPointsText.SetActive(false);
        _timeToAttackText.SetActive(false);
    }
    public void EnemyWins()
    {
        _current_turn = Turns.IDLE;
        _inputHandler.current_move = InputHandler.Moves.IDLE;
        _enemy.current_move = EnemyManager.Moves.IDLE;
        _defendMenuUI.SetActive(false);
        _attackMenuUI.SetActive(false);
        _enemyWinsMenu.SetActive(true);
        _playerPointsText.SetActive(false);
        _enemyPointsText.SetActive(false);
        _timeToAttackText.SetActive(false);        
    }
}