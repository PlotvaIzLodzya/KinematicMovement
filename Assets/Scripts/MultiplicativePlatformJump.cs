using UnityEngine;

[CreateAssetMenu(fileName = nameof(MultiplicativePlatformJump), menuName = "KinematicMovement/" + nameof(MultiplicativePlatformJump), order = 1)]
public class MultiplicativePlatformJump : PlatformJump
{
    [SerializeField] private float _horizontalMultiplier = 0.8f;

    public override void Init(ExteranlVelocityAccumulator velocityAccumulator, IJumpState state)
    {
        base.Init(velocityAccumulator, state);
    }

    public override Vector3 OnSet(Vector3 additionalSpeed)
    {
        var velocity = additionalSpeed;
        var scaled = velocity.Horizontal() * _horizontalMultiplier;
        scaled.y = additionalSpeed.y;

        return scaled;
    }

    public override Vector3 VelocityUpdate(Vector3 currentVelocity, Vector3 velocity)
    {
        return currentVelocity;
    }
}
