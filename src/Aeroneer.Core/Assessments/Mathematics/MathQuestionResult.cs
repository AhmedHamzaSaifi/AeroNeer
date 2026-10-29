namespace Aeroneer.Core.Assessments.Mathematics;

/// <summary>Records the candidate response and elapsed time for one question.</summary>
public sealed record MathQuestionResult(
    MathQuestion Question,
    decimal? CandidateAnswer,
    bool IsCorrect,
    bool Attempted,
    bool TimedOut,
    TimeSpan ResponseTime);

/// <summary>Contains bounded Mathematics score components and response-time metrics.</summary>
public sealed record MathAssessmentScore(
    double AccuracyPercent,
    double SpeedPercent,
    double Score,
    double MeanResponseMilliseconds);
