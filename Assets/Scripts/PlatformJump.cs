using UnityEngine;

public abstract class PlatformJump : ScriptableObject, IExteranlMovemnt
{
    private ExteranlVelocityAccumulator _velocityAccumulator;
    public Vector3 Velocity { get; set; }
    public IJumpState State { get; private set; }

    public virtual void Init(ExteranlVelocityAccumulator accumulator, IJumpState state)
    {
        _velocityAccumulator = accumulator;
        State = state;
    }

    public abstract Vector3 OnSet(Vector3 additionalSpeed, Vector3 angularVelocity);
    public abstract Vector3 VelocityUpdate(Vector3 platformVelocity, Vector3 characterVelocity);

    public void SetValue(Vector3 platformVelocity, Vector3 angularVelocity)
    {
        Velocity = OnSet(platformVelocity, angularVelocity);
    }

    public void UpdateState(Vector3 characterVelocity)
    {
        AccumulatorHandle();
        Velocity = VelocityUpdate(Velocity, characterVelocity);
    }

    private void AccumulatorHandle()
    {
        if (State.IsJumping)
            _velocityAccumulator.TryAdd(this);
        else
            _velocityAccumulator.TryRemove(this);

        if (State.IsJumping == false)
            Velocity = Vector3.zero;
    }

}
