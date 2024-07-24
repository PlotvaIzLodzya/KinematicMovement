using UnityEngine;

public class Mover: Platform
{
    public Vector3 Vel;

    public override void UpdateBody(IBody body, float deltaTime)
    {
        body.Position += Vel * deltaTime;
        //body.Rotation *= Quaternion.Euler(Vector3.up * 10 * deltaTime);
        //body.Position = body.Position.RotatePointAroundPivot(Vector3.zero, Quaternion.Euler(Vector3.up * 10 * deltaTime));
    }
}
