using PlotvaIzLodzya.Extensions;
using PlotvaIzLodzya.Player;
using PlotvaIzLodzya.Player.Movement.CollideAndSlide;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    private Movement _movement;

    private void Awake()
    {
        _movement = Extensions.GetComponent<Movement>(this);
    }

    private void Update()
    {
        _movement.Move(_playerInput.Direction);
    }
}
