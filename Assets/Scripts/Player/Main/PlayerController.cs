using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Настройки движения
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _acceleration = 30f;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _accelerationInAir = 5f;
    [SerializeField] private float _accelerationInAirMax = 15f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _maxLastGroundTime = 0.35f;
    [SerializeField] private float _maxLastJumpPressTime = 0.35f;

    private bool _isGround;
    private Collider _collider;
    private Vector3 _halfExtents;


    // Компоненты
    private Rigidbody _rb;
    private PlayerInputReader _inputReader;
    private PlayerStateMachine _stateMachine;

    // Свойства для доступа
    public Rigidbody Rb => _rb;
    public PlayerInputReader InputReader => _inputReader;
    public Collider Collider => _collider;

    public float MoveSpeed => _moveSpeed;
    public float Acceleration => _acceleration;
    public float DeAcceleration => _deAcceleration;
    public float AccelerationInAir => _accelerationInAir;
    public float AccelerationInAirMax => _accelerationInAirMax;
    public float JumpForce => _jumpForce;
    public float MaxLastGroundTime => _maxLastGroundTime;
    public float MaxLastJumpPressTime => _maxLastJumpPressTime;

    public bool IsGround => _isGround;
    public Vector3 GroundNormal => _groundNormal;


    // Список состояний
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }

    // Проверка на землю
    [SerializeField] private float _maxAngleJump = 45f;
    [SerializeField] private float _checkGroundDistance = 0.2f;
    private Vector3 _groundNormal;
    private void Start()
    {
        _stateMachine.Initialize(IdleState);
    }

    private void Awake()
    {
        // Получение компонентов
        _rb = GetComponent<Rigidbody>();
        _inputReader = GetComponent<PlayerInputReader>();
        _collider = GetComponent<Collider>();

        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.freezeRotation = true;

        // Инициализация состояний
        _stateMachine = new PlayerStateMachine();
        MoveState = new PlayerMoveState(this, _stateMachine);
        IdleState = new PlayerIdleState(this, _stateMachine);
        JumpState = new PlayerJumpState(this, _stateMachine);
        AirState = new PlayerAirState(this, _stateMachine);
    }

    private void Update()
    {
        if (_stateMachine?.CurrentState == null)
            return;

        _stateMachine.CurrentState.HandleInput();
        _stateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        if (_stateMachine?.CurrentState == null)
            return;

        CheckGround();
        _stateMachine.CurrentState.PhysicsUpdate();
    }

    private void CheckGround()
    {
        if (_collider == null) return;

        Bounds b = _collider.bounds;
        float radius = Mathf.Min(b.extents.x, b.extents.z) * 0.85f;
        Vector3 origin = new Vector3(b.center.x, b.min.y + radius + 0.01f, b.center.z);

        bool hit = Physics.SphereCast(
            origin, radius, Vector3.down, out RaycastHit hitInfo,
            _checkGroundDistance + 0.01f,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore);

        if (hit && hitInfo.collider != _collider)
        {
            float angle = Vector3.Angle(Vector3.up, hitInfo.normal);
            if (angle <= _maxAngleJump)
            {
                _isGround = true;
                _groundNormal = hitInfo.normal;
                return;
            }
        }

        _isGround = false;
        _groundNormal = Vector3.up;
    }


private void OnDrawGizmos()
    {
        if (_collider == null) return;

        Bounds b = _collider.bounds;
        Vector3 checkCenter = new Vector3(b.center.x, b.min.y + 0.05f, b.center.z);
        Vector3 halfExtents = new Vector3(b.extents.x * 0.9f, 0.1f, b.extents.z * 0.9f);
        float sphereRadius = Mathf.Min(halfExtents.x, halfExtents.z);

        // Фон сферы
        Gizmos.color = _isGround
            ? new Color(0f, 1f, 0f, 0.15f)
            : new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawSphere(checkCenter, sphereRadius);

        // Обводка сферы
        Gizmos.color = _isGround
            ? new Color(0f, 1f, 0f, 1f)
            : new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawWireSphere(checkCenter, sphereRadius);

        // Линия от центра вниз до точки проверки
        Gizmos.color = new Color(1f, 1f, 0f, 0.8f);
        Gizmos.DrawLine(b.center, checkCenter);

        // Точка в центре сферы
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(checkCenter, 0.03f);

        // Нормаль земли если стоим
        if (_isGround)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(checkCenter, _groundNormal * 0.5f);
        }
    }
}