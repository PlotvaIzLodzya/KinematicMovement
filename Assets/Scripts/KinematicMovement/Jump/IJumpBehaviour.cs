using System;

namespace PlotvaIzLodzya.KinematicMovement.Jump
{ 
    public interface IJumpBehaviour
    {
        void Jump(float speed);
        void CancelJump();
    }
}