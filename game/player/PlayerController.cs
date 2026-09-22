using Godot;

namespace BotchedBatchBrewery.Game.Player;

/// <summary>
/// Owns player locomotion and lightweight visual feedback.
/// Input is read through gameplay-specific actions defined in project.godot.
/// </summary>
public partial class PlayerController : CharacterBody3D
{
    private static readonly Vector3 StretchScale = new(0.72f, 1.38f, 0.72f);
    private static readonly Vector3 SquashScale = new(1.38f, 0.68f, 1.38f);

    [Export(PropertyHint.Range, "0.1,20.0,0.1")]
    public float Speed { get; set; } = 6.0f;

    [Export(PropertyHint.Range, "0.1,50.0,0.1")]
    public float Acceleration { get; set; } = 22.0f;

    [Export(PropertyHint.Range, "0.1,50.0,0.1")]
    public float Friction { get; set; } = 20.0f;

    [Export(PropertyHint.Range, "0.1,20.0,0.1")]
    public float JumpVelocity { get; set; } = 5.5f;

    [Export(PropertyHint.Range, "0.0,0.5,0.01")]
    public float CoyoteTime { get; set; } = 0.12f;

    [Export(PropertyHint.Range, "0.1,30.0,0.1")]
    public float TurnSpeed { get; set; } = 14.0f;

    [Export(PropertyHint.Range, "0.1,30.0,0.1")]
    public float ScaleSpringSpeed { get; set; } = 14.0f;

    [Export(PropertyHint.Range, "0.0,0.5,0.01")]
    public float BobAmplitude { get; set; } = 0.06f;

    [Export(PropertyHint.Range, "0.1,10.0,0.1")]
    public float BobFrequency { get; set; } = 2.4f;

    [Export]
    public Camera3D? Camera
    {
        get; set;
    }

    private MeshInstance3D? _mesh;
    private float _coyoteTimer;
    private bool _wasOnFloor;
    private float _bobTime;
    private float _meshBaseY;

    public override void _Ready()
    {
        _mesh = GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if (_mesh is null)
        {
            GD.PushError($"{Name}: MeshInstance3D child not found.");
        }
        else
        {
            _meshBaseY = _mesh.Position.Y;
        }

        Camera ??= GetNodeOrNull<Camera3D>("Camera3D");
        if (Camera is null)
        {
            GD.PushWarning($"{Name}: No Camera3D found; movement is world-relative.");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Vector3 velocity = Velocity;
        bool onFloor = IsOnFloor();

        if (!onFloor)
        {
            velocity += GetGravity() * dt;
        }

        _coyoteTimer = onFloor ? CoyoteTime : Mathf.Max(0.0f, _coyoteTimer - dt);
        if (Input.IsActionJustPressed("jump") && _coyoteTimer > 0.0f)
        {
            velocity.Y = JumpVelocity;
            _coyoteTimer = 0.0f;
            ApplyMeshScale(StretchScale);
        }

        if (!_wasOnFloor && onFloor)
        {
            ApplyMeshScale(SquashScale);
        }
        _wasOnFloor = onFloor;

        Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 moveDirection = GetMoveDirection(input);
        float rate = moveDirection.IsZeroApprox() ? Friction : Acceleration;
        Vector3 targetVelocity = moveDirection * Speed;
        velocity.X = Mathf.MoveToward(velocity.X, targetVelocity.X, rate * dt);
        velocity.Z = Mathf.MoveToward(velocity.Z, targetVelocity.Z, rate * dt);

        UpdateVisuals(moveDirection, velocity, onFloor, dt);

        Velocity = velocity;
        MoveAndSlide();
    }

    private Vector3 GetMoveDirection(Vector2 input)
    {
        if (input.IsZeroApprox())
        {
            return Vector3.Zero;
        }

        if (Camera is null)
        {
            return new Vector3(input.X, 0.0f, input.Y).Normalized();
        }

        Vector3 cameraForward = -Camera.GlobalBasis.Z;
        cameraForward.Y = 0.0f;
        cameraForward = cameraForward.Normalized();

        Vector3 cameraRight = Camera.GlobalBasis.X;
        cameraRight.Y = 0.0f;
        cameraRight = cameraRight.Normalized();

        return (cameraRight * input.X + cameraForward * -input.Y).Normalized();
    }

    private void UpdateVisuals(Vector3 moveDirection, Vector3 velocity, bool onFloor, float dt)
    {
        if (_mesh is null)
        {
            return;
        }

        if (!moveDirection.IsZeroApprox())
        {
            float targetAngle = Mathf.Atan2(moveDirection.X, moveDirection.Z);
            float turnWeight = 1.0f - Mathf.Exp(-TurnSpeed * dt);
            _mesh.Rotation = _mesh.Rotation with
            {
                Y = Mathf.LerpAngle(_mesh.Rotation.Y, targetAngle, turnWeight),
            };
        }

        float horizontalSpeed = new Vector2(velocity.X, velocity.Z).Length();
        if (onFloor && horizontalSpeed > 0.5f)
        {
            _bobTime += dt * BobFrequency * Mathf.Tau;
            _mesh.Position = _mesh.Position with
            {
                Y = _meshBaseY + Mathf.Sin(_bobTime) * BobAmplitude,
            };
        }
        else
        {
            _bobTime = 0.0f;
            _mesh.Position = _mesh.Position with
            {
                Y = Mathf.MoveToward(_mesh.Position.Y, _meshBaseY, 4.0f * dt),
            };
        }

        float scaleWeight = 1.0f - Mathf.Exp(-ScaleSpringSpeed * dt);
        _mesh.Scale = _mesh.Scale.Lerp(Vector3.One, scaleWeight);
    }

    private void ApplyMeshScale(Vector3 scale)
    {
        if (_mesh is not null)
        {
            _mesh.Scale = scale;
        }
    }
}
