using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int Health { get; }
    void TakeDamage(int damage);
}

public interface IMoving
{
    Vector3 MoveDir { get; }
    float Speed { get; }
}
