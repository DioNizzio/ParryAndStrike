using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Vector3 firstPoint;

    public Vector3 secondPoint;

    public enum Direction{
        HORIZONTAL,
        VERTICAL,
        DIAGONAL1,
        DIAGONAL2
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFirstPoint(Vector3 worldFirstPosition){
        firstPoint = worldFirstPosition;
    }

    public void SetSecondPoint(Vector3 worldSecondPosition){
        secondPoint = worldSecondPosition;
    }

    public Vector3 GetFirstPoint(){
        return firstPoint;
    }
    public Vector3 GetSecondPoint(){
        return secondPoint;
    }

    /*public Direction TransformVectorToDirectionEnum(Vector3 AttackVector){

        return;
    }*/
}
