namespace Aeroneer.Core.Assessments.Orientation;

/// <summary>Applies the fixed Orientation timer and one-point, no-penalty scoring rules.</summary>
public static class OrientationScoring
{
    /// <summary>Maximum time for a scored Orientation attempt.</summary>
    public static readonly TimeSpan AssessmentDuration = TimeSpan.FromMinutes(10);

    /// <summary>Calculates points and accuracy; unanswered questions earn neither points nor penalty.</summary>
    public static OrientationAssessmentScore Calculate(IEnumerable<OrientationAnswerResult> answers, int questionsPresented)
    {
        ArgumentNullException.ThrowIfNull(answers);
        if (questionsPresented < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(questionsPresented));
        }

        var results = answers.ToArray();
        if (results.Length > questionsPresented)
        {
            throw new ArgumentException("Recorded answers cannot exceed questions presented.", nameof(answers));
        }

        var correct = results.Count(answer =>
            answer.SelectedOption.HasValue
            && answer.SelectedOption == answer.CorrectOption);
        var answered = results.Count(answer => answer.SelectedOption.HasValue);
        var incorrect = answered - correct;
        var unanswered = questionsPresented - answered;
        var accuracy = questionsPresented == 0 ? 0d : correct * 100d / questionsPresented;

        return new OrientationAssessmentScore(correct, answered, correct, incorrect, unanswered, Math.Clamp(accuracy, 0d, 100d));
    }
}
