using PlotvaIzLodzya.KinematicMovement.Body;
using PlotvaIzLodzya.KinematicMovement.Platforms;
using UnityEngine;

public class MovingPlatform: Platform
{
    public Vector3 Vel;

    public override void UpdateBody(IBody body, float deltaTime)
    {
        var nextPos = body.Position + Vel * deltaTime;
        body.Position = nextPos;
        //body.Rotation *= Quaternion.Euler(Vector3.up * 90 * deltaTime);
        //body.Position = body.Position.RotateAroundPivot(Vector3.zero, Quaternion.Euler(Vector3.up * 10 * deltaTime));
    }
}
