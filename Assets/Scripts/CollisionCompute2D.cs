using UnityEngine;

public abstract class CollisionCompute2D<T> : CollisionCompute where T : Collider2D
{
    protected T Collider;

    protected CollisionCompute2D(T collider, Transform transform, ILayerMaskProvider layerMaskProvider) : base(transform, layerMaskProvider)
    {
        Collider = collider;
    }
}
