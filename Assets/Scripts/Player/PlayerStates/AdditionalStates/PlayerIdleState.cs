using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerController _player, PlayerStateMachine _playerState) : base(_player, _playerState)
    {

    }

    private Vector2 _moveInput;

    public override void HandleInput()
    {
        _moveInput = player.InputReader.MoveInput;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (_moveInput.sqrMagnitude > 0.001f)
        {
            playerState.ChangeState(player.MoveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector3 flatVel = Vector3.ProjectOnPlane(player.Rb.linearVelocity, player.GroundNormal);
        Vector3 brakeVel = Vector3.MoveTowards(flatVel, Vector3.zero, player.DeAcceleration * Time.fixedDeltaTime);
        player.Rb.AddForce(brakeVel - flatVel, ForceMode.VelocityChange);
    }
}
