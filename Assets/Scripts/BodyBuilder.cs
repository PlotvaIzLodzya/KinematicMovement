using UnityEngine;

public static class BodyBuilder
{
    public static IBody Create(GameObject go)
    {
        IBody body = null;
        if (go.TryGetComponent(out Rigidbody rb3D))
        {
            body = new Body3D(rb3D);
        }
        else if (go.TryGetComponent(out Rigidbody2D rb2d))
        {
            body = new Body2D(rb2d);
        }

        if (body == null)
        {
            throw new MissingComponentException($"{go.name} doesn't have rigidbody component");
        }

        return body;
    }
}

public static class CollisionBuilder
{
    public static ICollision Create(GameObject go, ILayerMaskProvider layerMaskProvider)
    {
        ICollision collision = null;
        if(go.TryGetComponent(out Collider2D collider))
        {
            collision = collider switch
            {
                CircleCollider2D circleCollider => new CircleCollision2D(circleCollider, go.transform, layerMaskProvider),
                BoxCollider2D boxCollider => new BoxCollision2D(boxCollider, go.transform, layerMaskProvider),
                CapsuleCollider2D capsuleCollider => new CapsuleCollision2D(capsuleCollider, go.transform, layerMaskProvider),
                _ => throw new MissingComponentException($"{go.name} don't have supported 2D collider")
            };
        }

        if(go.TryGetComponent(out Collider collider3d))
        {
            collision = collider3d switch
            {
                SphereCollider sphereCollider => new SphereCollision3D(sphereCollider, go.transform, layerMaskProvider),
                BoxCollider boxCollider => new BoxCollision3D(boxCollider, go.transform, layerMaskProvider),
                CapsuleCollider capsuleCollider => new CapsuleCollision3D(capsuleCollider, go.transform, layerMaskProvider),
                _ => throw new MissingComponentException($"{go.name} don't have supported 3D collider")
            };
        }

        if (collision == null)
            throw new MissingReferenceException($"{go.name} don't have supported collider");

        return collision;
    }
}
