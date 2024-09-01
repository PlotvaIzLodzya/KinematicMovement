using PlotvaIzLodzya.KinematicMovement.ExternalMovement;
using PlotvaIzLodzya.KinematicMovement.StateHandle;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.VelocityCompute
{
    public static class VelocityHandlerBuilder
    {
        public static VelocityHandler Create(VelocityHandler velocityHandler, MovementConfig config, MovementState state, IPlatformProvider platformProvier) 
        { 
            if(velocityHandler == null)
            {
                velocityHandler = ScriptableObject.CreateInstance<VelocityHandler>();
                Debug.Log(velocityHandler == null);
            }

            velocityHandler.Init(state, config, platformProvier);
            return velocityHandler;
        }
    }
}