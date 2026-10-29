namespace Aeroneer.Core.Assessments.Slalom;

/// <summary>Defines one bounded gate in normalized horizontal coordinates.</summary>
public sealed record SlalomGate(
    int Index,
    double Center,
    double GapWidth,
    double VerticalPosition);

/// <summary>Records the candidate's result for a crossed gate.</summary>
public sealed record SlalomGateResult(
    int GateIndex,
    double? AircraftPosition,
    double? CenterDeviation,
    bool Passed,
    bool Collided);
