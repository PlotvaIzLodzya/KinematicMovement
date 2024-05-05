using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.Player;
using PlotvaIzLodzya.Player.Movement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    private Movement _movement;
    private Jump _jump;
    private void Awake()
    {
        _movement = this.GetComponentNullAwarness<Movement>();
        _jump = new(_movement);
    }

    private void Update()
    {
        if(_playerInput.IsAnyDirectionButtonHolded)
            _movement.Move(_playerInput.Direction);

        if (_playerInput.JumpButtonPressed)
            _jump.Perform();
    }
}
