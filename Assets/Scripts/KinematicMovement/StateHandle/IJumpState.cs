namespace PlotvaIzLodzya.KinematicMovement.StateHandle
{
    public interface IJumpState
    {
        bool IsOnPlatform { get; }
        bool Grounded { get; }
        bool IsJumping { get; }
    }
}