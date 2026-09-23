namespace BotchedBatchBrewery.Game.Brewing;

public enum BrewPhase
{
    Empty,
    GrainAdded,
    HopsAdded,
    Brewing,
    Ready,
}

public enum BrewOutcome
{
    Drinkable,
    Botched,
}

/// <summary>
/// Engine-independent state for the first brewing loop.
/// </summary>
public sealed class BrewBatch
{
    public BrewPhase Phase
    {
        get; private set;
    } = BrewPhase.Empty;

    public BrewOutcome? Outcome
    {
        get; private set;
    }

    public float RemainingSeconds
    {
        get; private set;
    }

    public bool AddStandardIngredient(float brewDurationSeconds)
    {
        switch (Phase)
        {
            case BrewPhase.Empty:
                Phase = BrewPhase.GrainAdded;
                return true;
            case BrewPhase.GrainAdded:
                Phase = BrewPhase.HopsAdded;
                return true;
            case BrewPhase.HopsAdded:
                Start(BrewOutcome.Drinkable, brewDurationSeconds);
                return true;
            default:
                return false;
        }
    }

    public bool AddStrangeAdditive(float brewDurationSeconds)
    {
        if (Phase is not (BrewPhase.GrainAdded or BrewPhase.HopsAdded))
        {
            return false;
        }

        Start(BrewOutcome.Botched, brewDurationSeconds);
        return true;
    }

    public bool Advance(float delta)
    {
        if (Phase != BrewPhase.Brewing)
        {
            return false;
        }

        RemainingSeconds = MathF.Max(0.0f, RemainingSeconds - delta);
        if (RemainingSeconds > 0.0f)
        {
            return false;
        }

        Phase = BrewPhase.Ready;
        return true;
    }

    public bool Collect()
    {
        if (Phase != BrewPhase.Ready)
        {
            return false;
        }

        Phase = BrewPhase.Empty;
        Outcome = null;
        RemainingSeconds = 0.0f;
        return true;
    }

    private void Start(BrewOutcome outcome, float brewDurationSeconds)
    {
        Outcome = outcome;
        RemainingSeconds = MathF.Max(0.1f, brewDurationSeconds);
        Phase = BrewPhase.Brewing;
    }
}
