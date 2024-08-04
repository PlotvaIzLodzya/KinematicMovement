using PlotvaIzLodzya.KinematicMovement.Platforms;

namespace PlotvaIzLodzya.KinematicMovement.StateHandle
{
    public interface IExteranlMovementState
    {
        bool IsEnteredPlatform { get; }
        bool IsOnPlatform { get; }
        bool IsLeftPlatform { get; }

        public bool TrySetOnPlatform(IPlatform platform);
        public void LeavePlatform(IPlatform platform);
    }
}