using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.VelocityCompute;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Jump
{
    public static class JumpBuilder
    {
        public static JumpBehaviour Create(JumpBehaviour behaviour, IVeloictyComputeProvider velocitProvider, ICoroutineRunner coroutineRunner)
        {
            if(behaviour == null)
                return ScriptableObject.CreateInstance<DefaultJump>().Init(velocitProvider, coroutineRunner);

            return behaviour.Init(velocitProvider, coroutineRunner);
        }
    }
}