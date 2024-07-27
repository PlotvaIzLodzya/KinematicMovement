using UnityEngine;

public class Mover2 : MonoBehaviour
{
    public Vector3 Vel;
    public Vector3 Axis;
    public float rotationSpeed;

    private void FixedUpdate()
    {
        transform.position += Vel * Time.fixedDeltaTime;
        transform.rotation *= Quaternion.Euler(Axis * rotationSpeed * Time.fixedDeltaTime);
    }
}
