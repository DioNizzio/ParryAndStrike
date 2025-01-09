using System;
using UnityEngine;

[Serializable] public struct SlashData
{
    public enum Direction
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        DIAGONAL1,
        DIAGONAL2,
        DIAGONAL3,
        DIAGONAL4
    }

    public Transform PointA;
    public Transform PointB;
    public Direction AttackDirection;
    public Direction CounterDirection;
}
