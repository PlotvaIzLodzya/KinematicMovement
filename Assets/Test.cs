using PlotvaIzLodzya.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private BoxCollider _box;

    private void OnDrawGizmos()
    {
        Debug.Log(_box.size/2);
        Physics.BoxCast(transform.position, _box.size*0.5f, Vector3.forward, out RaycastHit hit, transform.rotation, 0.015f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hit.point, 0.2f);
    }
}
