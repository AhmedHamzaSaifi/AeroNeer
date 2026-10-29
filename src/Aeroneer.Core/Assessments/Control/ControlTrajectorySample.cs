namespace Aeroneer.Core.Assessments.Control;

/// <summary>One downsampled point in the Control simulation trajectory.</summary>
public sealed record ControlTrajectorySample(
    double ElapsedSeconds,
    double HorizontalDeviation,
    double VerticalDeviation);

/// <summary>Current normalized position and velocity of the simulated instruments.</summary>
public sealed record ControlSimulationState(
    double ElapsedSeconds,
    double HorizontalDeviation,
    double VerticalDeviation,
    double HorizontalVelocity,
    double VerticalVelocity);

/// <summary>Keyboard input state consumed by the deterministic Control simulation.</summary>
public readonly record struct ControlInputState(bool Left, bool Right, bool Up, bool Down);
