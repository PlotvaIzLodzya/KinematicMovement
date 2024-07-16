using UnityEngine;

public class Test2d : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private CircleCollider2D _collider;
    private CircleCollision2D _collision;
    private void OnDrawGizmos()
    {
        //_collision = new CircleCollision2D(_collider, transform);
        //var hit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.left   , 0.015f, _layerMask);
        //var pos = hit.point;
        //var info = _collision.GetHit(transform.position, Vector3.left, 0.015f, _layerMask); ;
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(pos + hit.normal * hit.distance, 0.5f);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position + info.normal * info.distance, 0.5f);
    }
}
