using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController _player, PlayerStateMachine _playerState) : base(_player, _playerState)
    {

    }

    private Vector2 _moveInput;

    public override void HandleInput()
    {
        base.HandleInput();
        _moveInput = player.InputReader.MoveInput;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.IsGround == true)
        {
            if(player.Rb.linearVelocity.y <= 0f)
            {
                if (_moveInput.sqrMagnitude > 0.001f)
                {
                    playerState.ChangeState(player.MoveState);
                }
                else
                {
                    playerState.ChangeState(player.IdleState);
                }
            }
        }

    }

    public override void PhysicsUpdate()
    {
        if (_moveInput.sqrMagnitude < 0.001f) return;

        Vector3 wishDir = (player.transform.forward * _moveInput.y
                         + player.transform.right * _moveInput.x).normalized;
        Vector3 flatVel = new Vector3(player.Rb.linearVelocity.x, 0f, player.Rb.linearVelocity.z);

        float currentSpeed = Vector3.Dot(flatVel, wishDir);
        float addableSpeed = player.AccelerationInAirMax - currentSpeed;
        if (addableSpeed <= 0f) return;

        float accelThisFrame = Mathf.Min(player.AccelerationInAir * Time.fixedDeltaTime, addableSpeed);
        player.Rb.AddForce(wishDir * accelThisFrame, ForceMode.VelocityChange);
    }
}
