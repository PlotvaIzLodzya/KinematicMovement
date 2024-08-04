using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Body
{
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
}