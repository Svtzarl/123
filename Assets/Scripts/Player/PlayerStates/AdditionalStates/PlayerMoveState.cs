using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(PlayerController _player, PlayerStateMachine _playerState):base(_player, _playerState)
    {
        
    }

    private Vector2 _moveInput;

    public override void HandleInput()
    {
        base.HandleInput();
       _moveInput = player.InputReader.MoveInput;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector3 moveDirection = (player.transform.forward * _moveInput.y
                               + player.transform.right * _moveInput.x).normalized;
        moveDirection.y = 0f;
        if (moveDirection.sqrMagnitude < 0.001f) return;
        moveDirection.Normalize();

        Vector3 slopeDir = Vector3.ProjectOnPlane(moveDirection, player.GroundNormal).normalized;
        Vector3 targetVelocity = slopeDir * player.MoveSpeed;
        Vector3 currentFlatVel = Vector3.ProjectOnPlane(player.Rb.linearVelocity, player.GroundNormal);

        // Резкий старт — чем меньше скорость тем резче разгон
        float speedFraction = currentFlatVel.magnitude / player.MoveSpeed;
        float rate = Mathf.Lerp(player.Acceleration * 2f, player.Acceleration, speedFraction);

        // При развороте тормозим резче
        float dot = Vector3.Dot(currentFlatVel.normalized, slopeDir);
        if (dot < 0f) rate = player.DeAcceleration;

        Vector3 newFlatVel = Vector3.MoveTowards(currentFlatVel, targetVelocity, rate * Time.fixedDeltaTime);
        player.Rb.AddForce(newFlatVel - currentFlatVel, ForceMode.VelocityChange);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

       if (_moveInput.sqrMagnitude < 0.001f)
        {
            playerState.ChangeState(player.IdleState);
        }
    }
}
