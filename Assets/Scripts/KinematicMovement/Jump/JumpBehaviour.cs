using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.KinematicMovement.VelocityCompute;
using System.Collections;
using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Jump
{
    public abstract class JumpBehaviour : ScriptableObject, IJumpBehaviour
    {
        private IVeloictyComputeProvider _velocityProvider;
        private ICoroutineRunner _coroutineRunner;
        protected IVelocityCompute VelocityCompute => _velocityProvider.Current;

        public virtual JumpBehaviour Init(IVeloictyComputeProvider velocityProvider, ICoroutineRunner coroutineRunner)
        {
            _velocityProvider = velocityProvider;
            _coroutineRunner = coroutineRunner;
            return this;
        }

        public abstract void CancelJump();
        public abstract void Jump(float speed);

        public Coroutine StartCoroutine(IEnumerator coroutine) => _coroutineRunner.StartCoroutine(coroutine);
        public void StopCoroutine(Coroutine coroutine) => _coroutineRunner.StopCoroutine(coroutine);
    }
}