using UnityEngine;

[CreateAssetMenu(fileName = nameof(FullFollowPlatformJump), menuName = "KinematicMovement/" + nameof(FullFollowPlatformJump), order = 1)]
public class FullFollowPlatformJump : PlatformJump
{
    public override Vector3 OnSet(Vector3 additionalSpeed)
    {
        return additionalSpeed;
    }

    public override Vector3 VelocityUpdate(Vector3 currentVelocity, Vector3 velocity)
    {
        return currentVelocity;
    }
}
