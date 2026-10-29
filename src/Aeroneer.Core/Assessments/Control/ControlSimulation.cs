namespace Aeroneer.Core.Assessments.Control;

/// <summary>Runs a seedable fixed-step keyboard tracking simulation independent of WPF.</summary>
public sealed class ControlSimulation
{
    public const double FixedStepSeconds = 1d / 60d;
    public const double TrajectoryIntervalSeconds = 0.1d;
    public const double TargetTolerance = 0.08d;
    private const double InputAcceleration = 1.4d;
    private const double VelocityDecay = 1.8d;
    private const double MaximumVelocity = 1.5d;
    private const double MaximumDeviation = 1d;
    private const double BaseTurbulence = 0.16d;
    private const double MaximumTurbulenceMultiplier = 3d;
    private const double DifficultyStageSeconds = 20d;

    private readonly Random _random;
    private readonly List<ControlTrajectorySample> _pendingSamples = [];
    private double _accumulator;
    private double _nextSampleSeconds = TrajectoryIntervalSeconds;
    private double _elapsedSeconds;
    private double _horizontalPosition;
    private double _verticalPosition;
    private double _horizontalVelocity;
    private double _verticalVelocity;
    private ControlInputState _input;

    /// <summary>Creates a deterministic simulation for the supplied seed.</summary>
    public ControlSimulation(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>Gets the current simulation snapshot.</summary>
    public ControlSimulationState State => new(
        _elapsedSeconds,
        _horizontalPosition,
        _verticalPosition,
        _horizontalVelocity,
        _verticalVelocity);

    /// <summary>Replaces all held-key state; pass the default value when paused or focus is lost.</summary>
    public void SetInput(ControlInputState input)
    {
        _input = input;
    }

    /// <summary>Advances the simulation using fixed 60 Hz steps and returns every 100 ms sample.</summary>
    public IReadOnlyList<ControlTrajectorySample> Advance(TimeSpan elapsed)
    {
        if (elapsed < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(elapsed));
        }

        _accumulator += elapsed.TotalSeconds;
        while (_accumulator >= FixedStepSeconds)
        {
            Step(FixedStepSeconds);
            _accumulator -= FixedStepSeconds;
            if (_elapsedSeconds + 1e-9 >= _nextSampleSeconds)
            {
                _pendingSamples.Add(new ControlTrajectorySample(
                    _nextSampleSeconds,
                    _horizontalPosition,
                    _verticalPosition));
                _nextSampleSeconds += TrajectoryIntervalSeconds;
            }
        }

        var samples = _pendingSamples.ToArray();
        _pendingSamples.Clear();
        return samples;
    }

    /// <summary>Clears held input without resetting trajectory or elapsed time.</summary>
    public void ClearInput() => _input = default;

    private void Step(double deltaSeconds)
    {
        _elapsedSeconds += deltaSeconds;
        var stage = (int)(_elapsedSeconds / DifficultyStageSeconds);
        var turbulenceMultiplier = Math.Min(1 + stage, MaximumTurbulenceMultiplier);
        var turbulenceLimit = BaseTurbulence * turbulenceMultiplier;
        var horizontalTurbulence = (_random.NextDouble() * 2 - 1) * turbulenceLimit;
        var verticalTurbulence = (_random.NextDouble() * 2 - 1) * turbulenceLimit;
        var horizontalDirection = (_input.Right ? 1 : 0) - (_input.Left ? 1 : 0);
        var verticalDirection = (_input.Down ? 1 : 0) - (_input.Up ? 1 : 0);

        _horizontalVelocity = Math.Clamp(
            _horizontalVelocity + (horizontalDirection * InputAcceleration + horizontalTurbulence - _horizontalVelocity * VelocityDecay) * deltaSeconds,
            -MaximumVelocity,
            MaximumVelocity);
        _verticalVelocity = Math.Clamp(
            _verticalVelocity + (verticalDirection * InputAcceleration + verticalTurbulence - _verticalVelocity * VelocityDecay) * deltaSeconds,
            -MaximumVelocity,
            MaximumVelocity);
        _horizontalPosition = IntegrateAxis(_horizontalPosition, ref _horizontalVelocity, deltaSeconds);
        _verticalPosition = IntegrateAxis(_verticalPosition, ref _verticalVelocity, deltaSeconds);
    }

    private static double IntegrateAxis(double position, ref double velocity, double deltaSeconds)
    {
        var nextPosition = position + velocity * deltaSeconds;
        if (nextPosition > MaximumDeviation)
        {
            velocity = Math.Min(0, velocity);
            return MaximumDeviation;
        }

        if (nextPosition < -MaximumDeviation)
        {
            velocity = Math.Max(0, velocity);
            return -MaximumDeviation;
        }

        return nextPosition;
    }
}
