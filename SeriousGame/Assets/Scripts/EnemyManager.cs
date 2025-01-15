using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameManager _game;

    [SerializeField] private Transform _collider;
    [SerializeField] private SpriteRenderer _enemySpriteRenderer;
    
    [SerializeField] private List<EnemyInitialPoses> _enemyInitialPoses;
    [SerializeField] private List<EnemyAttackingPoses> _enemyAttackingPoses;
    [SerializeField] private List<EnemyDefendingPoses> _enemyDefendingPoses; //and dodging

    [SerializeField] private SlashData[] _slashData;

    [SerializeField] private ThrustData[] _thrustData;
    
    private List<SlashData> _counterSlashData;

    private SlashData.Direction _playerDirection;
    private SlashData.Direction _current_direction;

    private SlashData _parryDirection;
    private SlashData _slashDirection;

    private ThrustData _thrustArea;

    private Vector3 _attackVector;
    //private Vector3 _defendVector;

    private Vector3 _spawnPosition;
    private Vector2 _spawnColliderSize;
    private Vector2 _spawnColliderOffset;

    public Coroutine coroutine;
    public Coroutine coroutine1;

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
        _spawnPosition = _collider.transform.position;
        _spawnColliderSize = _collider.GetComponent<CapsuleCollider2D>().size;
        _spawnColliderOffset = _collider.GetComponent<CapsuleCollider2D>().offset;
        _current_direction = SlashData.Direction.NONE;
        current_move = Moves.IDLE;
        _current_turn = Turn.PLAYER;
        enemyFinished = false;
        _counterSlashData = new List<SlashData>();
    }

    public void PlayerTurn()
    {
        _current_turn = Turn.PLAYER;
    }

    public void EnemyAttackTurn()
    {
        //ChangeSpriteToIdle();
        _collider.transform.position = _spawnPosition;
        _collider.GetComponent<CapsuleCollider2D>().size = _spawnColliderSize;
        _collider.GetComponent<CapsuleCollider2D>().offset = _spawnColliderOffset;
        _current_turn = Turn.ENEMY_ATTACK;
        Attack();
    }
    public void EnemyDefendTurn()
    {
        _current_turn = Turn.ENEMY_DEFEND;
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

    public Sprite GetInitialSprite(SlashData attackData)
    {
        if (attackData.AttackDirection == SlashData.Direction.VERTICAL && (attackData.PointA.transform.name.Equals("Point (1)") || (attackData.PointB.transform.name.Equals("Point (6)") && attackData.PointA.transform.name.Equals("Point (3)")) || (attackData.PointB.transform.name.Equals("Point (8)") && attackData.PointA.transform.name.Equals("Point (4)")) || attackData.PointA.transform.name.Equals("Point (5)") || attackData.PointA.transform.name.Equals("Point (7)")))
        {
            return _enemyInitialPoses[2].pose;
        }
        else if (attackData.AttackDirection == SlashData.Direction.VERTICAL && (attackData.PointA.transform.name.Equals("Point (2)") || (attackData.PointB.transform.name.Equals("Point (7)") && attackData.PointA.transform.name.Equals("Point (3)")) || attackData.PointA.transform.name.Equals("Point (6)")))
        {
            return _enemyInitialPoses[3].pose;
        
        }else if (attackData.AttackDirection == SlashData.Direction.VERTICAL && ((attackData.PointB.transform.name.Equals("Point (5)") && attackData.PointA.transform.name.Equals("Point (4)")) || attackData.PointA.transform.name.Equals("Point (8)")))
        {
            return _enemyInitialPoses[4].pose;
        }
        else if (attackData.AttackDirection == SlashData.Direction.HORIZONTAL && (attackData.PointA.transform.name.Equals("Point (3)") || attackData.PointA.transform.name.Equals("Point (6)") || attackData.PointA.transform.name.Equals("Point (7)") || (attackData.PointA.transform.name.Equals("Point (1)") && attackData.PointB.transform.name.Equals("Point (5)")) || (attackData.PointA.transform.name.Equals("Point (2)") && attackData.PointB.transform.name.Equals("Point (8)"))))
        {
            return _enemyInitialPoses[5].pose;
        }
        else if (attackData.AttackDirection == SlashData.Direction.HORIZONTAL && (attackData.PointA.transform.name.Equals("Point (4)") || attackData.PointA.transform.name.Equals("Point (5)") || attackData.PointA.transform.name.Equals("Point (8)") || (attackData.PointA.transform.name.Equals("Point (1)") && attackData.PointB.transform.name.Equals("Point (7)")) || (attackData.PointA.transform.name.Equals("Point (2)") && attackData.PointB.transform.name.Equals("Point (6)"))))
        {
            return _enemyInitialPoses[6].pose;
        }
        else if (attackData.AttackDirection == SlashData.Direction.DIAGONAL3)
        {
            return _enemyInitialPoses[7].pose;
        }
        else if (attackData.AttackDirection == SlashData.Direction.DIAGONAL1)
        {
            return _enemyInitialPoses[8].pose;
        }
        else if (attackData.AttackDirection == SlashData.Direction.DIAGONAL2)
        {
            return _enemyInitialPoses[9].pose;
        }
        else if (attackData.AttackDirection == SlashData.Direction.DIAGONAL4)
        {
            return _enemyInitialPoses[10].pose;
        }
        else
        {
            Debug.LogError("No pose selected");
            return null;
        }
    }

    public void ChangeToAttackSprite()
    {
        if (_enemySpriteRenderer.sprite.name.Equals("THRUSTPose"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[0].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("VERTICALUPDOWNPose"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[1].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("VERTICALDOWNUPLEFTPose"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[2].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("VERTICALDOWNUPRIGHTPose"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[3].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("HORIZONTALPoseLEFT_RIGHT"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[4].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("HORIZONTALPoseRIGHT_LEFT"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[5].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("DIAGONAL1PoseRIGHT_LEFT_UP_DOWN"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[6].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("DIAGONAL2PoseLEFT_RIGHT_DOWN_UP"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[7].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("DIAGONAL3PoseLEFT_RIGHT_UP_DOWN"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[8].pose;
        }
        else if (_enemySpriteRenderer.sprite.name.Equals("DIAGONAL4PoseRIGHT_LEFT_DOWN_UP"))
        {
            _enemySpriteRenderer.sprite = _enemyAttackingPoses[9].pose;
        }
    }

    public Sprite GetDefensiveParrySprite(SlashData parryData)
    {
        if (parryData.CounterDirection == SlashData.Direction.HORIZONTAL && parryData.PointA.transform.name.Equals("Point (6)"))
        {
            return _enemyDefendingPoses[0].pose;
        }
        else if (parryData.CounterDirection == SlashData.Direction.HORIZONTAL && parryData.PointA.transform.name.Equals("Point (8)"))
        {
            return _enemyDefendingPoses[1].pose;
        }
        else if (parryData.CounterDirection == SlashData.Direction.HORIZONTAL)
        {
            return _enemyDefendingPoses[2].pose;
        }
        else if (parryData.CounterDirection == SlashData.Direction.VERTICAL && (parryData.PointA.transform.name.Equals("Point (1)") || parryData.PointA.transform.name.Equals("Point (2)") || parryData.PointA.transform.name.Equals("Point (5)") || parryData.PointA.transform.name.Equals("Point (8)")))
        {
            return _enemyDefendingPoses[3].pose;
        }
        else if (parryData.CounterDirection == SlashData.Direction.VERTICAL && (parryData.PointA.transform.name.Equals("Point (6)") || parryData.PointA.transform.name.Equals("Point (7)")))
        {
            return _enemyDefendingPoses[4].pose;
        }
        else if (parryData.CounterDirection == SlashData.Direction.DIAGONAL1)
        {
            return _enemyDefendingPoses[5].pose;
        }
        else if (parryData.CounterDirection == SlashData.Direction.DIAGONAL2)
        {
            return _enemyDefendingPoses[6].pose;
        }
        else if (parryData.CounterDirection == SlashData.Direction.DIAGONAL3)
        {
            return _enemyDefendingPoses[7].pose;
        }
        else if (parryData.CounterDirection == SlashData.Direction.DIAGONAL4)
        {
            return _enemyDefendingPoses[8].pose;
        }
        else
        {
            Debug.LogError("No pose selected");
            return null;
        }
    }
    public Sprite GetDefensiveThrustSprite(ThrustData bodyPart)
    {
        if (bodyPart.PointA.transform.name.Equals("HEAD"))
        {
            return _enemyDefendingPoses[9].pose;
        }
        else if (bodyPart.PointA.transform.name.Equals("TORSO"))
        {
            return _enemyDefendingPoses[2].pose;
        }
        else if (bodyPart.PointA.transform.name.Equals("LEFT_ARM"))
        {
            return _enemyDefendingPoses[6].pose;
        }
        else if (bodyPart.PointA.transform.name.Equals("RIGHT_ARM"))
        {
            return _enemyDefendingPoses[8].pose;
        }
        else if (bodyPart.PointA.transform.name.Equals("LEFT_LEG"))
        {
            return _enemyDefendingPoses[0].pose;
        }
        else if (bodyPart.PointA.transform.name.Equals("RIGHT_LEG"))
        {
            return _enemyDefendingPoses[1].pose;
        }
        else
        {
            Debug.LogError("No pose selected");
            return null;
        }
    }

    public void ChangeSpriteToIdle()
    {
        _enemySpriteRenderer.sprite = _enemyInitialPoses[0].pose;
    }

    public void Attack()
    {
        if (_game.GetEnemyPoints() != 12 && _game.GetPlayerPoints() != 12)
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
                int slashAux = Random.Range(0, 116);
                _slashDirection = (SlashData)_slashData.GetValue(slashAux);

                //calculate random point A inside Circle A
                var centerAttackPointA = _slashDirection.PointA.transform.position;
                var radiusAttackPointA = _slashDirection.PointA.transform.GetComponent<SphereCollider>().radius;
                _attackPointA = GetVectorInsidePoint(centerAttackPointA, radiusAttackPointA);

                //calculate random point B inside Circle B
                var centerAttackPointB = _slashDirection.PointB.transform.position;
                var radiusAttackPointB = _slashDirection.PointB.transform.GetComponent<SphereCollider>().radius;
                _attackPointB = GetVectorInsidePoint(centerAttackPointB, radiusAttackPointB);

                enemyFinished = true;
                _enemySpriteRenderer.sprite = GetInitialSprite(_slashDirection);
                coroutine = StartCoroutine(_game.TimeForPlayerToDefend());
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
                    float randomX = Random.Range(0, 0.3f);
                    float randomY = Random.Range(0, 0.3f);

                    _attackVector = thrustPointCenter + new Vector3(randomX, randomY, 0);
                }
                else if (thrustAux == 1)
                {
                    float randomX = Random.Range(0, 0.5f);
                    float randomY = Random.Range(0, 0.5f);

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
                _enemySpriteRenderer.sprite = _enemyInitialPoses[1].pose;
                coroutine = StartCoroutine(_game.TimeForPlayerToDefend());
            }
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

            _enemySpriteRenderer.sprite = GetDefensiveParrySprite(_parryDirection);
            _game.EnemyFinishedDefending();
            coroutine1 = StartCoroutine(_game.EnemyWait2Seconds());
        }
        else if (current_move == Moves.DODGE)
        {
            int randomDirection = Random.Range(1, 4);
            
            if (randomDirection == 1) //1 = UP
            {
                _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.up * 0.8f;
                _collider.GetComponent<CapsuleCollider2D>().size = new Vector2(0.9f, 2.6f);
                _enemySpriteRenderer.sprite = _enemyDefendingPoses[10].pose;
                Debug.Log("Enemy dodged up");
            }
            else if (randomDirection == 2) //1 = DOWN
            {
                //shrink collider
               _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.down * 0.65f;
               _collider.GetComponent<CapsuleCollider2D>().size = new Vector2(1, 2.2f);
               _enemySpriteRenderer.sprite = _enemyDefendingPoses[11].pose;
            }
            else if (randomDirection == 2) //1 = LEFT
            {
                _collider.transform.position = new Vector3(-1.6f, 0, 0);
                _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.down * 0.1f;
                _collider.GetComponent<CapsuleCollider2D>().size = new Vector2(0.9f, 2.8f);
                _enemySpriteRenderer.sprite = _enemyDefendingPoses[12].pose;
            }
            else if (randomDirection == 3) //1 = RIGHT
            {
                _collider.transform.position = new Vector3(1.6f, 0, 0);
                _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.down * 0.1f;
                _collider.GetComponent<CapsuleCollider2D>().size = new Vector2(0.9f, 2.8f);
                _enemySpriteRenderer.sprite = _enemyDefendingPoses[13].pose;
            }
            _game.EnemyFinishedDefending();
            coroutine1 = StartCoroutine(_game.EnemyWait2Seconds());
        }
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

            _enemySpriteRenderer.sprite = GetDefensiveThrustSprite(_thrustArea);
            _game.EnemyFinishedDefending();
            coroutine1 = StartCoroutine(_game.EnemyWait2Seconds());
        }
        else if (current_move == Moves.DODGE)
        {
            int randomDirection = Random.Range(1, 4);

            if (randomDirection == 1) 
            {
                _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.up * 0.8f;
                _collider.GetComponent<CapsuleCollider2D>().size = new Vector2(0.9f, 2.6f);
                _enemySpriteRenderer.sprite = _enemyDefendingPoses[10].pose;
            }
            else if (randomDirection == 2) 
            {
                //shrink collider
                _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.down * 0.65f;
                _collider.GetComponent<CapsuleCollider2D>().size = new Vector2(1, 2.2f);
                _enemySpriteRenderer.sprite = _enemyDefendingPoses[11].pose;
            }
            else if (randomDirection == 2) 
            {
                _collider.transform.position = new Vector3(-1.6f, 0, 0);
                _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.down * 0.1f;
                _collider.GetComponent<CapsuleCollider2D>().size = new Vector2(0.9f, 2.8f);
                _enemySpriteRenderer.sprite = _enemyDefendingPoses[12].pose;
            }
            else if (randomDirection == 3)
            {
                _collider.transform.position = new Vector3(1.6f, 0, 0);
                _collider.GetComponent<CapsuleCollider2D>().offset = Vector2.down * 0.1f;
                _collider.GetComponent<CapsuleCollider2D>().size = new Vector2(0.9f, 2.8f);
                _enemySpriteRenderer.sprite = _enemyDefendingPoses[13].pose;
            }
            _game.EnemyFinishedDefending();
            coroutine1 = StartCoroutine(_game.EnemyWait2Seconds());
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
