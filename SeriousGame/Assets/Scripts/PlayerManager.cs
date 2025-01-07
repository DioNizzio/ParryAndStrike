using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform _collider;

    private Vector3 _firstPoint;

    private Vector3 _secondPoint;

    private Vector3 _finalVector;

    public enum Direction{
        NONE,
        HORIZONTAL,
        VERTICAL,
        DIAGONAL1,
        DIAGONAL2
    }

    private Direction _current_direction;

    private void Awake()
    {
        _current_direction = Direction.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFirstPoint(Vector3 worldFirstPosition)
    {
        _firstPoint = worldFirstPosition;
    }

    public void SetSecondPoint(Vector3 worldSecondPosition)
    {
        _secondPoint = worldSecondPosition;
    }

    public Vector3 GetFirstPoint()
    {
        return _firstPoint;
    }
    public Vector3 GetSecondPoint()
    {
        return _secondPoint;
    }
    public void calculateVector()
    {
        _finalVector = _secondPoint - _firstPoint;
        Debug.Log(_finalVector);
    }
    public Vector3 GetCalculatedVector()
    {
        return _finalVector = _secondPoint - _firstPoint;
    }

    public Direction TransformVectorToDirectionEnum()
    {
        if(Vector3.Dot(_finalVector, Vector3.left) == 1 || Vector3.Dot(_finalVector, Vector3.left) == -1)
        {
            _current_direction = Direction.HORIZONTAL;
        }
        else if (Vector3.Dot(_finalVector, Vector3.up) == 1 || Vector3.Dot(_finalVector, Vector3.up) == -1) 
        {
            _current_direction = Direction.VERTICAL;
        }
        else if (Vector3.Dot(_finalVector, new Vector3(1, 1, 0)) >= 0.5 || Vector3.Dot(_finalVector, new Vector3(1, 1, 0)) <= -0.5)
        {
            _current_direction = Direction.DIAGONAL1;
        }
        else if (Vector3.Dot(_finalVector, new Vector3(-1, 1, 0)) >= 0.5 || Vector3.Dot(_finalVector, new Vector3(-1, 1, 0)) <= -0.5)
        {
            _current_direction = Direction.DIAGONAL2;
        }

        return _current_direction;
    }
}
