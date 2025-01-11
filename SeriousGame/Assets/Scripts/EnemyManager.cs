using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform _collider;

    [SerializeField] private SlashData[] _slashData;

    private List<SlashData> _counterSlashData;

    private SlashData.Direction _playerDirection;
    private SlashData.Direction _current_direction;

    [SerializeField] private ThrustData[] _thrustData;

    private SlashData _parryDirection;
    private SlashData _slashDirection;

    private ThrustData _thrustArea;

    private Vector3 _attackVector;
    //private Vector3 _defendVector;

    private Vector3 _spawnPosition;

    public enum Moves
    {
        IDLE,
        SLASH,
        THRUST,
        DEFEND_THRUST,
        PARRY,
        DODGE
    }
    public enum Turn
    {
        PLAYER,
        ENEMY_ATTACK,
        ENEMY_DEFEND
    }

    public Moves current_move;
    private Turn _current_turn;

    public GameObject Points;
    public GameObject BodyPoints;

    public bool enemyFinished;

    private Vector3 _attackPointA;
    private Vector3 _attackPointB;
    private Vector3 _defensePointA;
    private Vector3 _defensePointB;

    private void Awake()
    {
        _current_direction = SlashData.Direction.NONE;
        current_move = Moves.IDLE;
        _current_turn = Turn.PLAYER;
        enemyFinished = false;
        _counterSlashData = new List<SlashData>();
        _spawnPosition = _collider.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_current_turn == Turn.PLAYER) return;
        else
        {

        }
    }
    public void PlayerTurn()
    {
        _current_turn = Turn.PLAYER;
        //Points.SetActive(false);
        //BodyPoints.SetActive(false);
    }

    public void EnemyAttackTurn()
    {
        _collider.transform.position = _spawnPosition;
        _current_turn = Turn.ENEMY_ATTACK;
        //Points.SetActive(true);
        //BodyPoints.SetActive(true);
    }
    public void EnemyDefendTurn()
    {
        _current_turn = Turn.ENEMY_DEFEND;
        //Points.SetActive(true);
        //BodyPoints.SetActive(true);
    }

    public void GetPlayerDirection(SlashData.Direction direction)
    {
        if (string.Equals(SlashData.Direction.HORIZONTAL, direction))
        {
            _playerDirection = SlashData.Direction.HORIZONTAL;
        }
        else if (string.Equals(SlashData.Direction.VERTICAL, direction))
        {
            _playerDirection = SlashData.Direction.VERTICAL;
        }
        else if (string.Equals(SlashData.Direction.DIAGONAL1, direction))
        {
            _playerDirection = SlashData.Direction.DIAGONAL1;
        }
        else if (string.Equals(SlashData.Direction.DIAGONAL2, direction))
        {
            _playerDirection = SlashData.Direction.DIAGONAL2;
        }
        else if (string.Equals(SlashData.Direction.DIAGONAL3, direction))
        {
            _playerDirection = SlashData.Direction.DIAGONAL3;
        }
        else if (string.Equals(SlashData.Direction.DIAGONAL4, direction))
        {
            _playerDirection = SlashData.Direction.DIAGONAL4;
        }
        else
        {
            Debug.LogError("Player direction invalid");
        }
        _current_turn = Turn.ENEMY_DEFEND;
        Defend();
    }

    public void PlayerThrust()
    {
        _current_turn = Turn.ENEMY_DEFEND;
        DefendThrust();
    }

    public void Attack()
    {
        //randomly choose to slash or thrust
        int random = Random.Range(1, 11);
        if (random <= 7)
        {
            current_move = Moves.SLASH;
        }
        else
        {
            current_move = Moves.THRUST;
        }

        if (current_move == Moves.SLASH)
        {
            //choose a random attack
            int slashAux = Random.Range(0, 101);
            _slashDirection = (SlashData)_slashData.GetValue(slashAux);

            //calculate random point A inside Circle A
            var centerAttackPointA = _slashDirection.PointA.transform.position;
            var radiusAttackPointA = _slashDirection.PointA.transform.GetComponent<SphereCollider>().radius;
            _attackPointA = GetVectorInsidePoint(centerAttackPointA, radiusAttackPointA);

            //calculate random point B inside Circle B
            var centerAttackPointB = _slashDirection.PointB.transform.position;
            var radiusAttackPointB = _slashDirection.PointB.transform.GetComponent<SphereCollider>().radius;
            _attackPointB = GetVectorInsidePoint(centerAttackPointB, radiusAttackPointB);

            //calculate final parry vector
            //_attackVector = _attackPointB - _attackPointA;

            enemyFinished = true;
        }
        else if (current_move == Moves.THRUST)
        {
            //choose a random area
            int thrustAux = Random.Range(0, 6);
            _thrustArea = (ThrustData)_thrustData.GetValue(thrustAux);
            var thrustPointCenter = _thrustArea.PointA.transform.position;
            var thrustAreaCapsuleCollider = _thrustArea.PointA.transform.GetComponent<CapsuleCollider2D>();

            if (thrustAux == 0)
            {
                float randomX = Random.Range(0, 0.38f);
                float randomY = Random.Range(0, 0.38f);

                _attackVector = thrustPointCenter + new Vector3(randomX, randomY, 0);
            }
            else if (thrustAux == 1)
            {
                float randomX = Random.Range(0, 0.68f);
                float randomY = Random.Range(0, 0.68f);

                _attackVector = thrustPointCenter + new Vector3(randomX, randomY, 0);
            }
            else if (thrustAux == 2)
            {
                //find a random point inside capsules
                _attackVector = GetRandomPointInCapsule(thrustAreaCapsuleCollider);
            }
            else if (thrustAux == 3)
            {
                _attackVector = GetRandomPointInCapsule(thrustAreaCapsuleCollider);
            }
            else if (thrustAux == 4)
            {
                _attackVector = GetRandomPointInCapsule(thrustAreaCapsuleCollider);
            }
            else if (thrustAux == 5)
            {
                _attackVector = GetRandomPointInCapsule(thrustAreaCapsuleCollider);
            }

            enemyFinished = true;
        }
    }

    public void Defend()
    {
        //randomly choose to parry or dodge
        int random = Random.Range(0, 11);
        if (random <= 6)
        {
            current_move = Moves.PARRY;
        }
        else
        {
            current_move = Moves.DODGE;
        }

        if (current_move == Moves.PARRY)
        {
            //add all counter moves to the player attack direction on a array
            foreach (var item in _slashData)
            {
                if (item.CounterDirection == _playerDirection)
                {
                    _counterSlashData.Add(item);
                }
            }

            //choose one of the counters randomly
            int defenseAux = Random.Range(0, _counterSlashData.Count);
            Debug.Log("defenseAux: " + defenseAux);
            Debug.Log("slashData count: " + _counterSlashData.Count);
            _parryDirection = _counterSlashData[defenseAux];

            //calculate random point A inside Circle A
            var centerDefensePointA = _parryDirection.PointA.transform.position;
            var radiusDefensePointA = _parryDirection.PointA.transform.GetComponent<SphereCollider>().radius;
            _defensePointA = GetVectorInsidePoint(centerDefensePointA, radiusDefensePointA);

            //calculate random point B inside Circle B
            var centerDefensePointB = _parryDirection.PointB.transform.position;
            var radiusDefensePointB = _parryDirection.PointB.transform.GetComponent<SphereCollider>().radius;
            _defensePointB = GetVectorInsidePoint(centerDefensePointB, radiusDefensePointB);

            //calculate final parry vector
            //_defendVector = _defensePointB - _defensePointA;

            enemyFinished = true;
        }
        else if (current_move == Moves.DODGE)
        {
            int randomDirection = Random.Range(1, 4);
            
            if (randomDirection == 1) //1 = UP
            {
                _collider.transform.position = new Vector3(0, 1.6f, 0);
                Debug.Log("Enemy dodged up");
            }
            /*else if (randomDirection == 2) //1 = DOWN
            {
                _collider.transform.position = new Vector3(0, -1.6f, 0);
                also need to shrink sprite and collider
               _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.down * 2;
            }*/
            else if (randomDirection == 2) //1 = LEFT
            {
                _collider.transform.position = new Vector3(-1.6f, 0, 0);
                Debug.Log("Enemy dodged left");
            }
            else if (randomDirection == 3) //1 = RIGHT
            {
                _collider.transform.position = new Vector3(1.6f, 0, 0);
                Debug.Log("Enemy dodged right");
            }
            enemyFinished = true;
        }
        Debug.Log("Enemy Finished Defense");
    }

    public void DefendThrust()
    {
        //randomly choose to defend thrust or dodge
        int random = Random.Range(1, 11);
        if (random <= 5)
        {
            current_move = Moves.DEFEND_THRUST;
        }
        else
        {
            current_move = Moves.DODGE;
        }

        if (current_move == Moves.DEFEND_THRUST)
        {
            //choose random area
            int thrustAux = Random.Range(0, 6);
            _thrustArea = (ThrustData)_thrustData.GetValue(thrustAux);

            //game manager receives area and has to check if player point is inside chosen area
            enemyFinished = true;
        }
        else if (current_move == Moves.DODGE)
        {
            int randomDirection = Random.Range(1, 4);

            if (randomDirection == 1) //1 = UP
            {
                _collider.transform.position = new Vector3(0, 1.6f, 0);
                Debug.Log("Enemy dodged up");
            }
            /*else if (randomDirection == 2) //1 = DOWN
            {
                _collider.transform.position = new Vector3(0, -1.6f, 0);
                also need to shrink collider and sprite crouch
                
            }*/
            else if (randomDirection == 2) //1 = LEFT
            {
                _collider.transform.position = new Vector3(-1.6f, 0, 0);
                Debug.Log("Enemy dodged left");
            }
            else if (randomDirection == 3) //1 = RIGHT
            {
                _collider.transform.position = new Vector3(1.6f, 0, 0);
                Debug.Log("Enemy dodged right");
            }
            enemyFinished = true;
        }
    }

    public Vector3 GetVectorInsidePoint(Vector3 center, float radius)
    {
        float randomX = Random.Range(0, 0.62f);
        float randomY = Random.Range(0, 0.62f);

        return center + new Vector3(randomX, randomY, 0);
    }

    public static Vector3 GetRandomPointInCapsule(CapsuleCollider2D capsuleCollider)
    {
        // Get the transform of the capsule for position and rotation
        Transform capsuleTransform = capsuleCollider.transform;

        // Get the properties of the capsule
        Vector2 center = capsuleCollider.offset;
        float width = capsuleCollider.size.x;
        float height = capsuleCollider.size.y;

        // Determine if the capsule is horizontal or vertical
        bool isHorizontal = capsuleCollider.direction == CapsuleDirection2D.Horizontal;

        // Adjust for the capsule's scale
        float scaleX = capsuleTransform.localScale.x;
        float scaleY = capsuleTransform.localScale.y;
        width *= scaleX;
        height *= scaleY;

        // Calculate the radius of the capsule's circular ends
        float radius = Mathf.Min(width, height) / 2f;

        // Randomly choose a point inside the capsule
        Vector2 randomPoint;
        while (true)
        {
            // Generate a random point inside the rectangular bounds of the capsule
            float randomX = Random.Range(-width / 2f, width / 2f);
            float randomY = Random.Range(-height / 2f, height / 2f);

            randomPoint = new Vector2(randomX, randomY);

            // Check if the point is inside the capsule's rounded ends or central rectangle
            if (IsPointInsideCapsule(randomPoint, width, height, radius, isHorizontal))
                break;
        }

        // Convert the local random point to world space
        Vector2 worldPoint = capsuleTransform.TransformPoint(center + randomPoint);

        // Return the random point as a Vector3 (z = 0 for 2D)
        return new Vector3(worldPoint.x, worldPoint.y, 0f);
    }

    private static bool IsPointInsideCapsule(Vector2 point, float width, float height, float radius, bool isHorizontal)
    {
        if (isHorizontal)
        {
            // Check against horizontal capsule bounds
            float rectWidth = width - 2 * radius;
            if (Mathf.Abs(point.x) <= rectWidth / 2f && Mathf.Abs(point.y) <= radius)
                return true;

            // Check against the left and right semicircles
            Vector2 leftCircleCenter = new Vector2(-rectWidth / 2f, 0f);
            Vector2 rightCircleCenter = new Vector2(rectWidth / 2f, 0f);

            return (point - leftCircleCenter).sqrMagnitude <= radius * radius ||
                   (point - rightCircleCenter).sqrMagnitude <= radius * radius;
        }
        else
        {
            // Check against vertical capsule bounds
            float rectHeight = height - 2 * radius;
            if (Mathf.Abs(point.y) <= rectHeight / 2f && Mathf.Abs(point.x) <= radius)
                return true;

            // Check against the top and bottom semicircles
            Vector2 topCircleCenter = new Vector2(0f, rectHeight / 2f);
            Vector2 bottomCircleCenter = new Vector2(0f, -rectHeight / 2f);

            return (point - topCircleCenter).sqrMagnitude <= radius * radius ||
                   (point - bottomCircleCenter).sqrMagnitude <= radius * radius;
        }
    }

    /*public Vector3 GetDefendVector()
    {
        return _defendVector;
    }*/
    public Vector3 GetDefensePointA()
    {
        return _defensePointA;
    }
    public Vector3 GetDefensePointB()
    {
        return _defensePointB;
    }

    public Vector3 GetAttackVector()
    {
        return _attackVector;
    }
    public Vector3 GetAttackPointA()
    {
        return _attackPointA;
    }
    public Vector3 GetAttackPointB()
    {
        return _attackPointB;
    }

    public Transform GetEnemyArea()
    {
        return _thrustArea.PointA;
    }
}
