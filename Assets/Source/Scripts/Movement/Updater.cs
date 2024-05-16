using PlotvaIzLodzya.Movement.Platforms;
using UnityEngine;

namespace PlotvaIzLodzya.Player.Movement
{
    public class Updater: MonoBehaviour
    {
        [SerializeField] private MovingPlatform[] _platforms;
        [SerializeField] private Movement[] _movements;

        private void Update()
        {
            //foreach (var platform in _platforms) 
            //{
            //    platform.CUpdate();
            //}

            //foreach (var movement in _movements)
            //{
            //    movement.CUpdate();
            //}

        }
    }
}

