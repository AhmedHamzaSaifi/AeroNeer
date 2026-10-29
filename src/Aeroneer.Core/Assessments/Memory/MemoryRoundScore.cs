namespace Aeroneer.Core.Assessments.Memory;

/// <summary>Contains bounded component and aggregate scores for a memory round.</summary>
/// <param name="AccuracyPercent">Correct requested fields as a percentage.</param>
/// <param name="SpeedPercent">Speed component relative to the recall timeout.</param>
/// <param name="Score">Weighted round score from 0 through 100.</param>
/// <param name="DifficultyWeight">Weight used when aggregating rounds.</param>
public sealed record MemoryRoundScore(
    double AccuracyPercent,
    double SpeedPercent,
    double Score,
    int DifficultyWeight);
