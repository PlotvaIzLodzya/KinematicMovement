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
    public static ICollision Create(GameObject go, IBody body, ILayerMaskProvider layerMaskProvider)
    {
        ICollision collision = null;
        if(go.TryGetComponent(out Collider2D collider))
        {
            var body2D = body as Body2D;
            collision = collider switch
            {
                CircleCollider2D circleCollider => new CircleCollision2D(circleCollider, body2D, layerMaskProvider),
                BoxCollider2D boxCollider => new BoxCollision2D(boxCollider, body2D, layerMaskProvider),
                CapsuleCollider2D capsuleCollider => new CapsuleCollision2D(capsuleCollider, body2D, layerMaskProvider),
                _ => throw new MissingComponentException($"{go.name} don't have supported 2D collider")
            };
        }

        if(go.TryGetComponent(out Collider collider3d))
        {
            var body3D = body as Body3D;
            collision = collider3d switch
            {
                SphereCollider sphereCollider => new SphereCollision3D(sphereCollider, body3D, layerMaskProvider),
                //BoxCollider boxCollider => new BoxCollision3D(boxCollider, body3D, layerMaskProvider),
                CapsuleCollider capsuleCollider => new CapsuleCollision3D(capsuleCollider, body3D, layerMaskProvider),
                _ => throw new MissingComponentException($"{go.name} don't have supported 3D collider")
            };
        }

        if (collision == null)
            throw new MissingReferenceException($"{go.name} don't have supported collider");

        return collision;
    }
}
