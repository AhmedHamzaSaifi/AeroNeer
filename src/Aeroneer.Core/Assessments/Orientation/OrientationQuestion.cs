namespace Aeroneer.Core.Assessments.Orientation;

/// <summary>One offline-generated instrument interpretation question and its four symbols.</summary>
public sealed record OrientationQuestion(
    Guid Id,
    int CompassHeadingDegrees,
    int RelativeBearingDegrees,
    int HorizonPitchDegrees,
    int HorizonBankDegrees,
    int AbsoluteBeaconBearingDegrees,
    IReadOnlyList<OrientationSymbol> Options,
    int CorrectOption,
    string Explanation);

/// <summary>Describes the attitude and beacon direction represented by one answer symbol.</summary>
public sealed record OrientationSymbol(
    int BeaconBearingDegrees,
    int PitchDegrees,
    int BankDegrees);

/// <summary>Records a selected answer; a null answer represents an unanswered question.</summary>
public sealed record OrientationAnswerResult(
    Guid QuestionId,
    int? SelectedOption,
    int CorrectOption,
    bool IsCorrect,
    TimeSpan ResponseTime);

/// <summary>Summary of points and answer accuracy for an Orientation assessment.</summary>
public sealed record OrientationAssessmentScore(
    int Points,
    int AnsweredCount,
    int CorrectCount,
    int IncorrectCount,
    int UnansweredCount,
    double AccuracyPercent);
