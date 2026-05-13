using UnityEngine;

public abstract class PlayerGroundState : PlayerState
{
    private float _lastGroundTime = float.NegativeInfinity;

    public PlayerGroundState(PlayerController _player, PlayerStateMachine _playerState)
        : base(_player, _playerState) { }

    public override void Enter()
    {
        _lastGroundTime = Time.time;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.IsGround)
            _lastGroundTime = Time.time;

        CheckJump();
        CheckFall();
    }

    public override void PhysicsUpdate()
    {
        Vector3 gravityCompensation = -Vector3.ProjectOnPlane(Physics.gravity, player.GroundNormal);
        player.Rb.AddForce(gravityCompensation, ForceMode.Acceleration);
    }

    public virtual void CheckJump()
    {
        bool jumpBuffered = Time.time - player.InputReader.LastJumpPressTime < player.MaxLastJumpPressTime;
        bool coyoteValid = Time.time - _lastGroundTime < player.MaxLastGroundTime;

        if (jumpBuffered && coyoteValid)
        {
            player.InputReader.ResetLastJumpPressTime();
            playerState.ChangeState(player.JumpState);
        }
    }

    public virtual void CheckFall()
    {
        if (!player.IsGround && Time.time - _lastGroundTime > player.MaxLastGroundTime)
            playerState.ChangeState(player.AirState);
    }
}