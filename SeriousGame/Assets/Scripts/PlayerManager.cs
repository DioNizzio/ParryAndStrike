using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform _collider;

    [SerializeField] private GameObject _AttackMenu;
    [SerializeField] private GameObject _DefendMenu;
    [SerializeField] private GameObject _ParryAbility;
    [SerializeField] private GameObject _DefendThrustAbility;

    [SerializeField] private GameObject _Points;

    [SerializeField] private ThrustData[] _thrustData;

    private Vector3 _firstPoint;
    private Vector3 _secondPoint; 
    
    private Vector3 _thrustPoint;

    private Vector3 _finalVector;

    private Transform _bodyPart;

    private Vector3 _spawnPosition;

    public enum BodyPart
    {
        NONE,
        HEAD,
        TORSO,
        LEFT_ARM,
        RIGHT_ARM,
        LEFT_LEG,
        RIGHT_LEG
    }

    /*public enum Direction{
        NONE,
        HORIZONTAL,
        VERTICAL,
        DIAGONAL1,
        DIAGONAL2
    }*/

    public enum Turn
    {
        IDLE,
        PLAYER,
        ENEMY
    }

    private string _current_direction;
    private string _dodge_direction;
    private BodyPart _current_body_part;
    private Turn _current_turn;

    private void Awake()
    {
        _current_turn = Turn.IDLE;
        _current_body_part = BodyPart.NONE;
        _spawnPosition = _collider.transform.position;
        //_current_direction = Direction.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayerAttackTurn()
    {
        _current_turn = Turn.PLAYER;
        _DefendMenu.SetActive(false);
        _AttackMenu.SetActive(true);
    }

    public void PlayerDefendTurn(string enemyMove)
    {
        _current_turn = Turn.PLAYER;
        _AttackMenu.SetActive(false);
        _DefendMenu.SetActive(true);
        if (enemyMove.Equals("PARRY")){
            _DefendThrustAbility.SetActive(false);
            _ParryAbility.SetActive(true);
        }
        else if (enemyMove.Equals("DEFEND_THRUST"))
        {
            _ParryAbility.SetActive(false);
            _DefendThrustAbility.SetActive(true);
        }
    }

    public void EnemyTurn()
    {
        _current_turn = Turn.ENEMY;
    }

    public void SetFirstPoint(Vector3 worldFirstPosition)
    {
        _firstPoint = worldFirstPosition;
    }

    public void SetSecondPoint(Vector3 worldSecondPosition)
    {
        _secondPoint = worldSecondPosition;
    }
    public void SetThrustPoint(Vector3 thrustPoint)
    {
        _thrustPoint = thrustPoint;
    }
    public Vector3 GetThrustPoint()
    {
        return _thrustPoint;
    }

    public Transform GetBodyPart()
    {
        return _bodyPart;
    }

    public BodyPart GetCurrentBodyPart()
    {
        return _current_body_part;
    }

    public void calculateVector()
    {
        _finalVector = _secondPoint - _firstPoint;
        Debug.Log(_finalVector);
    }
    /*public Vector3 GetCalculatedVector()
    {
        return _finalVector;
    }*/

    public Vector3 GetFirstPoint()
    {
        return _firstPoint;
    }
    public Vector3 GetSecondPoint()
    {
        return _secondPoint;
    }

    public void DodgeDirection(string direction)
    {
        if (direction.Equals("UP"))
        {
            _collider.transform.position = new Vector3(0, 1.6f, 0);
        }
        if (direction.Equals("DOWN"))
        {
            _collider.transform.position = new Vector3(0, -1.6f, 0);
        }
        if (direction.Equals("LEFT"))
        {
            _collider.transform.position = new Vector3(-1.6f, 0, 0);
        }
        if (direction.Equals("RIGHT"))
        {
            _collider.transform.position = new Vector3(1.6f, 0, 0);
        }
    }
    public void SetBodyPart(string bodyPart)
    {
        //_Points.SetActive(true);
        if (bodyPart.Equals("HEAD"))
        {
            _current_body_part = BodyPart.HEAD;
            _bodyPart = _thrustData[0].PointA.transform;
        }
        if (bodyPart.Equals("TORSO"))
        {
            _current_body_part = BodyPart.TORSO;
            _bodyPart = _thrustData[1].PointA.transform;
        }
        if (bodyPart.Equals("LEFT_ARM"))
        {
            _current_body_part = BodyPart.LEFT_ARM;
            _bodyPart = _thrustData[2].PointA.transform;
        }
        if (bodyPart.Equals("RIGHT_ARM"))
        {
            _current_body_part = BodyPart.RIGHT_ARM;
            _bodyPart = _thrustData[3].PointA.transform;
        }
        if (bodyPart.Equals("LEFT_LEG"))
        {
            _current_body_part = BodyPart.LEFT_LEG;
            _bodyPart = _thrustData[4].PointA.transform;
        }
        if (bodyPart.Equals("RIGHT_LEG"))
        {
            _current_body_part = BodyPart.RIGHT_LEG;
            _bodyPart = _thrustData[5].PointA.transform;
        }
    }

    public void HidePoints()
    {
        //_Points.SetActive(false);
    }

    public void ResetCollider()
    {
        _collider.transform.position = _spawnPosition;
    }

    public SlashData.Direction TransformVectorToDirectionEnum()
    {
        if(Vector3.Dot(_finalVector, Vector3.left) == 1 || Vector3.Dot(_finalVector, Vector3.left) > 0.8 || Vector3.Dot(_finalVector, Vector3.left) == -1 || Vector3.Dot(_finalVector, Vector3.left) < -0.8)
        {
            //_current_direction = "HORIZONTAL";
            return SlashData.Direction.HORIZONTAL;
        }
        else if (Vector3.Dot(_finalVector, Vector3.up) == 1 || Vector3.Dot(_finalVector, Vector3.up) > 0.8 || Vector3.Dot(_finalVector, Vector3.up) == -1 || Vector3.Dot(_finalVector, Vector3.up) < -0.8)
        {
            //_current_direction = "VERTICAL";
            return SlashData.Direction.VERTICAL;
        }
        else if (Vector3.Dot(_finalVector, new Vector3(1, 1, 0)) <= -0.5)
        {
            //_current_direction = "DIAGONAL1";
            return SlashData.Direction.DIAGONAL1;
        }
        else if (Vector3.Dot(_finalVector, new Vector3(1, 1, 0)) >= 0.5)
        {
            //_current_direction = "DIAGONAL2";
            return SlashData.Direction.DIAGONAL2;
        }
        else if (Vector3.Dot(_finalVector, new Vector3(-1, 1, 0)) <= -0.5)
        {
            //_current_direction = "DIAGONAL3";
            return SlashData.Direction.DIAGONAL3;
        }
        else if (Vector3.Dot(_finalVector, new Vector3(-1, 1, 0)) >= 0.5)
        {
            //_current_direction = "DIAGONAL4";
            return SlashData.Direction.DIAGONAL4;
        }
        else
        {
            Debug.LogError("Invalid slashData direction");
        }

        return SlashData.Direction.NONE;//_current_direction;
    }
}
