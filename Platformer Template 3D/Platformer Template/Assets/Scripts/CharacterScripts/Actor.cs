using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Actor : MonoBehaviour, IDamageable, IMoving
{
    [Header("Actor Settings")]
    [SerializeField] internal Rigidbody _body;
    public int Health { get; internal set; }

    [field:SerializeField] public Vector3 MoveDir { get; internal set; } = Vector3.zero;
    [field:SerializeField] public float Speed { get; internal set; } = 0;

    public virtual void Awake()
    {
        // If the body field isn't set manually, it will be set to the component attached to this object.
        if (_body == null)
        {
            _body = GetComponent<Rigidbody>();
        }
    }

    public virtual void FixedUpdate()
    {
        _body.AddForce(MoveDir * Speed);
    }

    void IDamageable.TakeDamage(int damage)
    {
        Health -= damage;
    }
}
