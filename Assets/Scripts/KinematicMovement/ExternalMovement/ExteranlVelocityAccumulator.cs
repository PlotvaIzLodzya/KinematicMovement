using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.Platforms;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using System.Collections.Generic;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.ExternalMovement
{
    public interface IPlatformProvider
    {
        IPlatform Platform { get; }
    }

    public class ExteranlVelocityAccumulator : IPlatformProvider
    {
        private List<IExternalMovement> _movements;

        private IExteranlMovementState _state;

        public IPlatform Platform { get; private set; }
        public Vector3 TotalVelocity => GetTotalVelocity();
        private bool _haveRotation => Platform != null;

        public ExteranlVelocityAccumulator(IExteranlMovementState state)
        {
            _movements = new();
            _state = state;
        }

        public bool Have(IExternalMovement movemnt)
        {
            return _movements.Contains(movemnt);
        }

        public bool TryAdd(IExternalMovement m)
        {
            if (m is IPlatform platform)
            {
                if (_state.TrySetOnPlatform(platform))
                {
                    Platform = platform;
                    return _movements.TryAdd(m);
                }
                else
                {
                    return false;
                }
            }

            return _movements.TryAdd(m);
        }

        public bool TryRemove(IExternalMovement m)
        {
            bool removed = false;
            if (m == Platform)
            {
                Platform = null;
                _state.LeavePlatform(m as IPlatform);
                removed = true;
            }
            if (_movements.Contains(m))
            {
                _movements.Remove(m);
                removed = true;
            }

            return removed;
        }

        public Vector3 GetPositionByRotation(Vector3 currentPos)
        {
            if (_haveRotation == false)
                return currentPos;

            return currentPos.RotateAroundPivot(Platform.Position, Platform.RotationVelocity);
        }

        private Vector3 GetTotalVelocity()
        {
            var velocity = Vector3.zero;

            foreach (var ext in _movements)
            {
                velocity += ext.Velocity;
            }

            return velocity;
        }
    }
}