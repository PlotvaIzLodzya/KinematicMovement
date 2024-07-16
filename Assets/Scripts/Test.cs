using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test: MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody _rb;
    public float radius = 3f; // show penetration into the colliders located inside a sphere of this radius
    public int maxNeighbours = 16; // maximum amount of neighbours visualised

    private Collider[] neighbours;
    private void OnDrawGizmos()
    {
        var hits = Physics.OverlapSphere(transform.position + Vector3.down * 0.015f, 0.5f, _layerMask);

        var pos = GetClosestPoint2(hits, transform.position + Vector3.down * 0.015f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, 0.5f);        
    }

    public Vector3 GetClosestPoint2(Collider[] colliders, Vector3 position)
    {
        var closestPosition = Vector3.zero;
        Collider closestCollider = null;
        var closestDistance = float.MinValue;
        //Physics.queriesHitBackfaces = true;

        HitInfo hitInfo = new HitInfo();
        foreach (var collider in colliders)
        {

            Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation, _collider, position, transform.rotation, out Vector3 dir, out float distance);
            var dist = distance;
            if (dist > closestDistance)
            {
                closestDistance = dist;
                closestCollider = collider;
                closestPosition = position - dir * closestDistance;
                hitInfo = new HitInfo(closestPosition, dir, distance, closestDistance, true, collider.transform);
                //Debug.Log(dist);
            }
        }

        //if(closestCollider != null)
        //    Debug.Log($"{closestCollider.transform.name} dist {closestDistance}");
        return closestPosition;
    }

    public HitInfo GetHit(Vector3 position, LayerMask layerMask)
    {
        var hit = Physics.OverlapSphere(position, 0.5f, layerMask);

        var pos = GetHitInfo(hit, position);
        return pos;
    }

    public HitInfo GetHitInfo(Collider[] colliders, Vector3 position)
    {
        var closestDistance = float.MinValue;

        HitInfo hitInfo = new HitInfo();
        foreach (var collider in colliders)
        {

            Physics.ComputePenetration(collider, collider.transform.position, collider.transform.rotation, _collider, transform.position, transform.rotation, out Vector3 dir, out float distance);
            var dist = distance;
            if (dist > closestDistance)
            {
                closestDistance = dist;
                var closestPosition = position - dir * closestDistance;
                hitInfo = new HitInfo(closestPosition, dir, distance, closestDistance, true, collider.transform);
            }
        }

        return hitInfo;
    }

    public Vector3 GetClosestPoint(Collider[] colliders, Vector3 position)
    {
        var closestPosition = Vector3.zero;
        Collider closestCollider = null;
        var closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            //Debug.Log(collider.name);
            var posOnBounds = collider.ClosestPoint(position);
            //var posOnBounds = _collider.ClosestPoint(collider.transform.position);
            var dist = Vector3.Distance(posOnBounds, position);
            if(dist < closestDistance )
            {
                closestDistance = dist;
                closestPosition = posOnBounds;
            }
        }

        if(closestCollider != null)
        {
            Physics.ComputePenetration(closestCollider, closestCollider.transform.position, closestCollider.transform.rotation, _collider, transform.position, transform.rotation, out Vector3 dir, out float distance);

            closestPosition = transform.position + dir * distance;
        }
        return closestPosition;
    }
}
