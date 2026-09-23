using Godot;

namespace BotchedBatchBrewery.Game.Brewing;

/// <summary>
/// Adapts the first brew-batch rule to a fixed, player-operated prototype station.
/// </summary>
public partial class BrewingStation : Node3D
{
    [Export(PropertyHint.Range, "1.0,30.0,0.5")]
    public float BrewDurationSeconds { get; set; } = 6.0f;

    [Export(PropertyHint.Range, "1.0,6.0,0.1")]
    public float InteractionRadius { get; set; } = 2.5f;

    private readonly BrewBatch _batch = new();
    private Label3D? _statusLabel;
    private MeshInstance3D? _kettleMesh;
    private OmniLight3D? _kettleLight;
    private StandardMaterial3D? _kettleMaterial;
    private Node3D? _player;
    private bool _playerInRange;

    public override void _Ready()
    {
        _statusLabel = GetNodeOrNull<Label3D>("StatusLabel");
        _kettleMesh = GetNodeOrNull<MeshInstance3D>("Kettle");
        _kettleLight = GetNodeOrNull<OmniLight3D>("KettleLight");

        if (_statusLabel is null || _kettleMesh is null || _kettleLight is null)
        {
            GD.PushError($"{Name}: Brewing station scene is missing a required child node.");
            SetProcess(false);
            return;
        }

        _kettleMaterial = _kettleMesh.GetActiveMaterial(0) as StandardMaterial3D;
        if (_kettleMaterial is null)
        {
            GD.PushError($"{Name}: Kettle requires a StandardMaterial3D.");
            SetProcess(false);
            return;
        }

        _player = ResolvePlayer();
        ApplyPresentation();
    }

    public override void _Process(double delta)
    {
        UpdatePlayerProximity();

        if (_batch.Advance((float)delta))
        {
            ApplyPresentation();
        }

        if (!_playerInRange)
        {
            return;
        }

        if (Input.IsActionJustPressed("brew_interact") && _batch.AddStandardIngredient(BrewDurationSeconds))
        {
            ApplyPresentation();
        }
        else if (Input.IsActionJustPressed("brew_weird_additive") && _batch.AddStrangeAdditive(BrewDurationSeconds))
        {
            ApplyPresentation();
        }
        else if (Input.IsActionJustPressed("brew_interact") && _batch.Collect())
        {
            ApplyPresentation();
        }
    }

    private void UpdatePlayerProximity()
    {
        _player ??= ResolvePlayer();
        if (_player is null)
        {
            return;
        }

        bool wasInRange = _playerInRange;
        _playerInRange = GlobalPosition.DistanceSquaredTo(_player.GlobalPosition) <= InteractionRadius * InteractionRadius;
        if (_playerInRange != wasInRange)
        {
            ApplyPresentation();
        }
    }

    private Node3D? ResolvePlayer()
    {
        return GetTree().GetFirstNodeInGroup("player") as Node3D
            ?? GetTree().Root.FindChild("Player", true, false) as Node3D;
    }

    private void ApplyPresentation()
    {
        if (_statusLabel is null || _kettleMaterial is null || _kettleLight is null)
        {
            return;
        }

        switch (_batch.Phase)
        {
            case BrewPhase.Empty:
                SetStationVisuals(new Color(0.43f, 0.16f, 0.04f), new Color(1.0f, 0.36f, 0.1f), 0.8f);
                _statusLabel.Text = _playerInRange ? "E: Add grain" : "Brew kettle";
                break;
            case BrewPhase.GrainAdded:
                SetStationVisuals(new Color(0.54f, 0.32f, 0.06f), new Color(1.0f, 0.63f, 0.18f), 1.2f);
                _statusLabel.Text = _playerInRange ? "E: Add hops\nQ: Add strange additive" : "Grain added";
                break;
            case BrewPhase.HopsAdded:
                SetStationVisuals(new Color(0.3f, 0.44f, 0.08f), new Color(0.66f, 1.0f, 0.18f), 1.4f);
                _statusLabel.Text = _playerInRange ? "E: Brew standard batch\nQ: Brew strange batch" : "Hops added";
                break;
            case BrewPhase.Brewing:
                bool isBotched = _batch.Outcome == BrewOutcome.Botched;
                SetStationVisuals(
                    isBotched ? new Color(0.42f, 0.08f, 0.52f) : new Color(0.72f, 0.27f, 0.03f),
                    isBotched ? new Color(0.9f, 0.12f, 1.0f) : new Color(1.0f, 0.38f, 0.08f),
                    2.2f);
                _statusLabel.Text = $"Brewing… {MathF.Ceiling(_batch.RemainingSeconds)}s";
                break;
            case BrewPhase.Ready:
                bool isBotchedBatch = _batch.Outcome == BrewOutcome.Botched;
                SetStationVisuals(
                    isBotchedBatch ? new Color(0.74f, 0.06f, 0.86f) : new Color(0.9f, 0.48f, 0.04f),
                    isBotchedBatch ? new Color(1.0f, 0.12f, 0.92f) : new Color(1.0f, 0.72f, 0.2f),
                    3.0f);
                _statusLabel.Text = _playerInRange
                    ? isBotchedBatch ? "E: Collect BOTCHED batch" : "E: Collect drinkable batch"
                    : isBotchedBatch ? "BOTCHED batch ready" : "Drinkable batch ready";
                break;
        }
    }

    private void SetStationVisuals(Color kettleColor, Color lightColor, float lightEnergy)
    {
        _kettleMaterial!.AlbedoColor = kettleColor;
        _kettleLight!.LightColor = lightColor;
        _kettleLight.LightEnergy = lightEnergy;
    }
}
