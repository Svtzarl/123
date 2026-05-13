using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private bool _shouldApplyForce;
    private bool _forceApplied;

    public PlayerJumpState(PlayerController _player, PlayerStateMachine _playerState)
        : base(_player, _playerState) { }

    public override void Enter()
    {
        Vector3 v = player.Rb.linearVelocity;
        player.Rb.linearVelocity = new Vector3(v.x, 0f, v.z);
        _shouldApplyForce = true;
        _forceApplied = false;
    }

    public override void PhysicsUpdate()
    {
        if (_shouldApplyForce)
        {
            player.Rb.AddForce(Vector3.up * player.JumpForce, ForceMode.Impulse);
            _shouldApplyForce = false;
            _forceApplied = true;
        }
    }

    public override void LogicUpdate()
    {
        if (_forceApplied)
            playerState.ChangeState(player.AirState);
    }
}