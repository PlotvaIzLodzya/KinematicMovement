using UnityEngine;

public abstract class CollisionCompute2D<T> : CollisionCompute<Body2D> where T : Collider2D
{
    protected T Collider;

    protected CollisionCompute2D(T collider, Body2D body, ILayerMaskProvider layerMaskProvider) : base(body, layerMaskProvider)
    {
        Collider = collider;
    }
}
