using UnityEngine;

public class Mover2 : MonoBehaviour
{
    public Vector3 Vel;

    private void FixedUpdate()
    {
        transform.position += Vel * Time.fixedDeltaTime;
        //transform.rotation *= Quaternion.Euler(Vector3.up * 10 * Time.fixedDeltaTime);
        //body.Position = body.Position.RotatePointAroundPivot(Vector3.zero, Quaternion.Euler(Vector3.up * 10 * deltaTime));
    }
}
