using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public interface IBody
    {
        Vector3 Position { get; }
        void MovePosition(Vector3 postion);
    }
}

