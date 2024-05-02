using UnityEngine;

namespace PlotvaIzLodzya.Extensions
{
    public static class Extensions
    {
        public static T GetComponent<T>(this MonoBehaviour mono) where T : MonoBehaviour
        {
            return mono.GetComponent<T>() ?? throw new MissingComponentException($"Cant get {typeof(T)}");
        }
    }
}
