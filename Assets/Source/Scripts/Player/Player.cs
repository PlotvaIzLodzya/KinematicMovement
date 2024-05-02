using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.Player;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private MovementConfig _movementConfig;

    private Movement _movement;

    private void Awake()
    {
        _movement = Extensions.GetComponent<Movement>(this);
        _movement.Init(_movementConfig);
    }

    private void Update()
    {
        _movement.Move(_playerInput.Direction);
    }
}
