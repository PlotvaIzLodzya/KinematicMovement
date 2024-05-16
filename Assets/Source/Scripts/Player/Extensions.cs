using UnityEngine;

namespace PlotvaIzLodzya.Extensions
{
    public static class Extensions
    {
        public static T GetComponentNullAwarness<T>(this MonoBehaviour mono) where T : Component
        {
            var name = typeof(T).Name;
            return mono.GetComponent<T>() ?? throw new MissingComponentException($"Cant get {name}");
        }

        public static Vector3 ClampMagnitude(this Vector3 value, float minMagnitude)
        {
            if(value.sqrMagnitude > minMagnitude*minMagnitude) 
                return value;

            var dir = value.normalized;

            value = dir * minMagnitude;

            return value;
        }

        public static Vector3 ClampMagnitude(this Vector3 value, float minMagnitude, float maxMagnitude)
        {
            value = Vector3.ClampMagnitude(value, maxMagnitude);
            value = value.ClampMagnitude(minMagnitude);

            return value;
        }


    }
}
