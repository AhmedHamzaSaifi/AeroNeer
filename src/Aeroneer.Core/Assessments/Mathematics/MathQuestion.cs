namespace Aeroneer.Core.Assessments.Mathematics;

/// <summary>A generated question with a unique numeric answer and explicit tolerance.</summary>
public sealed record MathQuestion(
    Guid Id,
    MathQuestionType Type,
    string Text,
    decimal CorrectAnswer,
    decimal Tolerance,
    TimeSpan ResponseTimeout);
