using Godot;

namespace BotchedBatchBrewery.Game.Player;

/// <summary>
/// Smooth, fixed-offset follow camera for the player prototype.
/// </summary>
public partial class FollowCamera : Camera3D
{
    [Export]
    public Vector3 Offset { get; set; } = new(0.0f, 9.0f, 5.0f);

    [Export(PropertyHint.Range, "0.1,30.0,0.1")]
    public float SmoothSpeed { get; set; } = 7.0f;

    [Export(PropertyHint.Range, "-5.0,5.0,0.1")]
    public float LookAtHeightOffset { get; set; } = 0.8f;

    private Node3D? _target;

    public override void _Ready()
    {
        _target = GetParent() as Node3D;
        if (_target is null)
        {
            GD.PushError($"{Name}: FollowCamera must be a child of a Node3D target.");
            SetProcess(false);
            return;
        }

        TopLevel = true;
        GlobalPosition = _target.GlobalPosition + Offset;
        LookAt(_target.GlobalPosition + Vector3.Up * LookAtHeightOffset);
    }

    public override void _Process(double delta)
    {
        if (_target is null)
        {
            return;
        }

        float weight = 1.0f - Mathf.Exp(-SmoothSpeed * (float)delta);
        Vector3 targetPosition = _target.GlobalPosition + Offset;
        GlobalPosition = GlobalPosition.Lerp(targetPosition, weight);
        LookAt(_target.GlobalPosition + Vector3.Up * LookAtHeightOffset);
    }
}
