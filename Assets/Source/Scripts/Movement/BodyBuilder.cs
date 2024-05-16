using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public static class BodyBuilder
    {
        public static IBody Create(GameObject gameObject)
        {
            var rb2D = gameObject.GetComponent<Rigidbody2D>();
            var rb3D = gameObject.GetComponent<Rigidbody>();

            if(rb2D != null)
                return new Body2D(rb2D);
            if(rb3D != null)
                return new Body3D(rb3D);

            throw new MissingComponentException($"{gameObject.name} has not rigidbody");
        }
    }
}

