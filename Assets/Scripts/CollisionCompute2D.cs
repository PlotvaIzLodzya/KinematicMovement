using UnityEngine;

public abstract class CollisionCompute2D<T> : CollisionCompute where T : Collider2D
{
    protected T Collider;

    protected CollisionCompute2D(T collider, Transform transform) : base(transform)
    {
        Collider = collider;
    }
}
