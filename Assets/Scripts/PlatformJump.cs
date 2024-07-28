using UnityEngine;

public abstract class PlatformJump : ScriptableObject, IExteranlMovemnt
{
    private ExteranlVelocityAccumulator _velocityAccumulator;

    public Vector3 Velocity { get; set; }
    public IJumpState State { get; private set; }

    public virtual void Init(ExteranlVelocityAccumulator velocityAccumulator, IJumpState state)
    {
        _velocityAccumulator = velocityAccumulator;
        State = state;
    }

    public void Set()
    {
        Velocity = OnSet(_velocityAccumulator.TotalVelocity);
    }

    public void UpdateState(Vector3 velocity)
    {
        AccumulatorHandle();
        Velocity = VelocityUpdate(Velocity, velocity);
    }

    private void AccumulatorHandle()
    {
        if (State.IsJumping)
            _velocityAccumulator.TryAdd(this);
        else
            _velocityAccumulator.TryRemove(this);

        if (State.IsJumping == false)
        {
            Velocity = Vector3.zero;
        }
    }

    public abstract Vector3 OnSet(Vector3 additionalSpeed);
    public abstract Vector3 VelocityUpdate(Vector3 currentVelocity, Vector3 characterVelocity);
}
