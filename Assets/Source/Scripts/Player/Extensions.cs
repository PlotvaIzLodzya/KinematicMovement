using UnityEngine;

namespace PlotvaIzLodzya.Extensions
{
    public static class Extensions
    {
        public static T GetComponentNullAwarness<T>(this MonoBehaviour mono) where T : MonoBehaviour
        {
            var name = typeof(T).Name;
            return mono.GetComponent<T>() ?? throw new MissingComponentException($"Cant get {name}");
        }
    }
}
