using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement.CollideAndSlide.CollisionDetection
{
    public interface ICollisionHandler
    {
        bool IsCollide(Vector3 pos, Vector3 dir, out RaycastHit hit, float dist);
    }
}

