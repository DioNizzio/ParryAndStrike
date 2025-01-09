using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform _collider;

    [SerializeField] private SlashData[] _slashData;

    private SlashData[] _counterSlashData;

    private SlashData.Direction _playerDirection;
    private SlashData.Direction _current_direction;

    [SerializeField] private ThrustData[] _thrustData;

    private SlashData _parryDirection;
    private SlashData _slashDirection;

    private ThrustData _thrustArea;

    private Vector3 _attackVector;
    private Vector3 _defendVector;

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

    private void Awake()
    {
        _current_direction = SlashData.Direction.NONE;
        current_move = Moves.IDLE;
        _current_turn = Turn.PLAYER;
        enemyFinished = false;
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
        Points.SetActive(false);
        BodyPoints.SetActive(false);
    }

    public void EnemyAttackTurn()
    {
        _collider.transform.position = Vector3.zero;
        _current_turn = Turn.ENEMY_ATTACK;
        Points.SetActive(true);
        BodyPoints.SetActive(true);
    }
    public void EnemyDefendTurn()
    {
        _current_turn = Turn.ENEMY_DEFEND;
        Points.SetActive(true);
        BodyPoints.SetActive(true);
    }

    public void GetPlayerDirection(string direction)
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
            var attackPointA = GetVectorInsidePoint(centerAttackPointA, radiusAttackPointA);

            //calculate random point B inside Circle B
            var centerAttackPointB = _slashDirection.PointB.transform.position;
            var radiusAttackPointB = _slashDirection.PointB.transform.GetComponent<SphereCollider>().radius;
            var attackPointB = GetVectorInsidePoint(centerAttackPointB, radiusAttackPointB);

            //calculate final parry vector
            _attackVector = attackPointB - attackPointA;

            enemyFinished = true;
        }
        else if (current_move == Moves.THRUST)
        {
            //choose a random area
            int thrustAux = Random.Range(0, 6);
            _thrustArea = (ThrustData)_thrustData.GetValue(thrustAux);
            var thrustPointCenter = _thrustArea.PointA.transform.position;

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
            }
            else if (thrustAux == 3)
            {

            }
            else if (thrustAux == 4)
            {

            }
            else if (thrustAux == 5)
            {

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
            int i = 0;
            foreach (SlashData item in _slashData)
            {
                if (item.CounterDirection == _playerDirection)
                {
                    _counterSlashData.SetValue(item, i);
                    i++;
                }
            }

            //choose one of the counters randomly
            int defenseAux = Random.Range(0, i+1);
            _parryDirection = (SlashData)_counterSlashData.GetValue(defenseAux);

            //calculate random point A inside Circle A
            var centerDefensePointA = _parryDirection.PointA.transform.position;
            var radiusDefensePointA = _parryDirection.PointA.transform.GetComponent<SphereCollider>().radius;
            var defensePointA = GetVectorInsidePoint(centerDefensePointA, radiusDefensePointA);

            //calculate random point B inside Circle B
            var centerDefensePointB = _parryDirection.PointB.transform.position;
            var radiusDefensePointB = _parryDirection.PointB.transform.GetComponent<SphereCollider>().radius;
            var defensePointB = GetVectorInsidePoint(centerDefensePointB, radiusDefensePointB);

            //calculate final parry vector
            _defendVector = defensePointB - defensePointA;

            enemyFinished = true;
        }
        else if (current_move == Moves.DODGE)
        {
            int randomDirection = Random.Range(1, 4);
            
            if (randomDirection == 1) //1 = UP
            {
                _collider.transform.position = new Vector3(0, 1.6f, 0);
            }
            /*else if (randomDirection == 2) //1 = DOWN
            {
                _collider.transform.position = new Vector3(0, -1.6f, 0);
                also need to shrink sprite and collider
            }*/
            else if (randomDirection == 2) //1 = LEFT
            {
                _collider.transform.position = new Vector3(-1.6f, 0, 0);
            }
            else if (randomDirection == 3) //1 = RIGHT
            {
                _collider.transform.position = new Vector3(1.6f, 0, 0);
            }
            enemyFinished = true;
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
            enemyFinished = true;
        }
        else if (current_move == Moves.DODGE)
        {
            int randomDirection = Random.Range(1, 4);

            if (randomDirection == 1) //1 = UP
            {
                _collider.transform.position = new Vector3(0, 1.6f, 0);
            }
            /*else if (randomDirection == 2) //1 = DOWN
            {
                _collider.transform.position = new Vector3(0, -1.6f, 0);
                also need to shrink sprite and collider
            }*/
            else if (randomDirection == 2) //1 = LEFT
            {
                _collider.transform.position = new Vector3(-1.6f, 0, 0);
            }
            else if (randomDirection == 3) //1 = RIGHT
            {
                _collider.transform.position = new Vector3(1.6f, 0, 0);
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

    public Vector3 GetDefendVector()
    {
        return _defendVector;
    }

    public Vector3 GetAttackVector()
    {
        return _attackVector;
    }

    public Transform GetEnemyArea()
    {
        return _thrustArea.PointA;
    }
}
