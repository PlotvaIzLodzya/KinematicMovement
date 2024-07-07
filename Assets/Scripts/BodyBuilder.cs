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
    public static ICollision Create(GameObject go)
    {
        ICollision collision = null;
        if(go.TryGetComponent(out Collider2D collider))
        {
            collision = collider switch
            {
                CircleCollider2D => new CircleCollision2D(collider, go.transform),
                _ => throw new MissingComponentException($"{go.name} don't have supported collider")
            };
        }


        return collision;
    }
}
