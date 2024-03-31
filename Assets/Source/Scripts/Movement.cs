using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private PlayerInput _input;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private bool _applyGravity;
    [SerializeField] private Vector3 _exteranalForce;

    private int _collideDepth;
    private float _clipPreventingValue;
    private Vector3 _gravity;
    private Vector3 _worldUp;
    private Vector3 _characterUp;
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rb;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        _rb= GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _clipPreventingValue = 0.015f;
        _collideDepth = 5;
        _gravity = Vector3.down * 9.8f;
        _worldUp = Vector3.up;
        _characterUp = Vector3.up;
    }

    private void Update()
    {
        var vel = _input.Direction * _speed;
        IsGrounded = IsCollide(transform.position, -_characterUp, out RaycastHit hit, 2 * _clipPreventingValue);
        
        if (_applyGravity && IsGrounded == false)
        {
            vel += _gravity;
        }
        vel += _exteranalForce;

        Move(vel, transform.position);
    }

    private void Move(Vector3 vel, Vector3 pos)
    {
        vel = CollideAndSlide_recursive(vel * Time.deltaTime, pos, 0);
        transform.position += vel;
    }

    private Vector3 ProjectOnSurface(Vector3 vector, Vector3 normal)
    {
        vector = Vector3.ProjectOnPlane(vector, normal).normalized * vector.magnitude;
        
        return vector;
    }


    private Vector3 CollideAndSlide_recursive(Vector3 vel, Vector3 currentPos, int depth)
    {
        if(depth>= _collideDepth)
        {
            return Vector3.zero;
        }

        float dist = vel.magnitude + _clipPreventingValue;
        var dir = vel.normalized;
        RaycastHit hit;

        if (IsCollide(currentPos, dir, out hit, dist))
        {
            var velToNextStep = dir * (hit.distance - _clipPreventingValue);
            var leftOverVel = vel - velToNextStep;

            var nextPos = currentPos + velToNextStep;

            float angle = Vector3.Angle(_worldUp, hit.normal);

            var projectedleftOverVel = ProjectOnSurface(leftOverVel, hit.normal).normalized*leftOverVel.magnitude;

            projectedleftOverVel = HandleSlope(angle, vel, projectedleftOverVel, hit.normal);

            vel = velToNextStep + CollideAndSlide_recursive(projectedleftOverVel, nextPos, depth+1);
            
            return vel;
        }

        return vel;
    }

    private bool IsCollide(Vector3 currentPos, Vector3 dir, out RaycastHit hit, float dist)
    {
        return CapsuleCast(currentPos, dir, out hit, dist);
        //return SphereCast(currentPos, dir, out hit, dist);
    }

    private bool CapsuleCast(Vector3 currentPos, Vector3 dir, out RaycastHit hit, float dist)
    {
        var p1 = currentPos +_capsuleCollider.center + _characterUp * -_capsuleCollider.height *0.5f;
        var p2 = p1 + _characterUp *_capsuleCollider.height;

        return Physics.CapsuleCast(p1, p2, _capsuleCollider.radius, dir, out hit, dist);
    }

    //private bool SphereCast(Vector3 currentPos, Vector3 dir, out RaycastHit hit, float dist)
    //{
    //    return Physics.SphereCast(currentPos, _capsuleCollider.radius, dir, out hit, dist, _collisionMask);
    //}

    private bool IsTooSteepSlope(float angle)
    {
        return angle >= _maxSlopeAngle;
    }

    private Vector3 HandleSlope(float slopeAngle, Vector3 vel, Vector3 projectedleftOverVel, Vector3 surfaceNormal)
    {
        if (IsGrounded == false)
            return projectedleftOverVel;

        if (IsTooSteepSlope(slopeAngle))
        {
            projectedleftOverVel.y = vel.y;
            projectedleftOverVel = ScaleByHorizontalVelocity(vel, projectedleftOverVel, surfaceNormal);
        }
        else
        {
            vel.y = projectedleftOverVel.y;
            projectedleftOverVel = vel.normalized * projectedleftOverVel.magnitude;
        }

        return projectedleftOverVel;
    }

    public Vector3 ScaleByHorizontalVelocity(Vector3 vel, Vector3 projectedVel, Vector3 surfaceNormal)
    {
        vel.y = 0;
        surfaceNormal.y = 0;
        float scale = 1 + Vector3.Dot(vel.normalized, surfaceNormal.normalized);

        var scaledVel = projectedVel * scale;

        return scaledVel;
    }

}
