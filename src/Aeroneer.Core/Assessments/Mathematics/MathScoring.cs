namespace Aeroneer.Core.Assessments.Mathematics;

/// <summary>Calculates Mathematics result metrics and the documented bounded score.</summary>
public static class MathScoring
{
    /// <summary>Scores question accuracy at 80% and response speed at 20%.</summary>
    /// <param name="results">Question outcomes; unanswered or timed-out questions must be supplied as incorrect and timed out.</param>
    public static MathAssessmentScore Calculate(IEnumerable<MathQuestionResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);
        var items = results.ToArray();
        if (items.Length == 0)
        {
            return new MathAssessmentScore(0, 0, 0, 0);
        }

        var correctCount = items.Count(result => result.IsCorrect);
        var accuracy = correctCount * 100d / items.Length;
        var speedTotal = items.Sum(result =>
        {
            if (!result.Attempted || result.TimedOut)
            {
                return 0d;
            }

            var elapsed = Math.Max(0d, result.ResponseTime.TotalMilliseconds);
            var timeoutMilliseconds = result.Question.ResponseTimeout.TotalMilliseconds;
            if (timeoutMilliseconds <= 0)
            {
                return 0d;
            }

            return 100d * (1d - Math.Clamp(elapsed / timeoutMilliseconds, 0d, 1d));
        });
        var speed = speedTotal / items.Length;
        var score = 0.8d * accuracy + 0.2d * speed;
        var meanResponseMilliseconds = items
            .Where(result => result.Attempted && !result.TimedOut && result.ResponseTime >= TimeSpan.Zero)
            .Select(result => result.ResponseTime.TotalMilliseconds)
            .DefaultIfEmpty(0d)
            .Average();

        return new MathAssessmentScore(Clamp(accuracy), Clamp(speed), Clamp(score), meanResponseMilliseconds);
    }

    /// <summary>Checks an answer against its generated question's explicit tolerance.</summary>
    public static bool IsCorrect(MathQuestion question, decimal candidateAnswer)
    {
        ArgumentNullException.ThrowIfNull(question);
        return Math.Abs(candidateAnswer - question.CorrectAnswer) <= question.Tolerance;
    }

    private static double Clamp(double value) => Math.Clamp(value, 0d, 100d);
}
