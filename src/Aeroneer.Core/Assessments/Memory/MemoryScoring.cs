namespace Aeroneer.Core.Assessments.Memory;

/// <summary>Calculates bounded Memory round and assessment scores.</summary>
public static class MemoryScoring
{
    /// <summary>Maximum recall interval used for the speed component.</summary>
    public static readonly TimeSpan RecallTimeout = TimeSpan.FromSeconds(15);

    /// <summary>Calculates a round score using 80% accuracy and 20% response speed.</summary>
    public static MemoryRoundScore ScoreRound(
        int correctFields,
        int requestedFields,
        TimeSpan recallTime,
        bool timedOut,
        int difficultyWeight)
    {
        if (requestedFields < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedFields));
        }

        if (difficultyWeight < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(difficultyWeight));
        }

        if (requestedFields == 0)
        {
            return new MemoryRoundScore(0, 0, 0, difficultyWeight);
        }

        var boundedCorrectFields = Math.Clamp(correctFields, 0, requestedFields);
        var accuracy = boundedCorrectFields * 100d / requestedFields;
        var elapsedMilliseconds = Math.Max(0, recallTime.TotalMilliseconds);
        var speed = timedOut
            ? 0
            : 100d * (1d - Math.Clamp(elapsedMilliseconds / RecallTimeout.TotalMilliseconds, 0d, 1d));
        var score = 0.8d * accuracy + 0.2d * speed;

        return new MemoryRoundScore(ClampScore(accuracy), ClampScore(speed), ClampScore(score), difficultyWeight);
    }

    /// <summary>Calculates the difficulty-weighted mean of round scores.</summary>
    public static double ScoreAssessment(IEnumerable<MemoryRoundScore> roundScores)
    {
        ArgumentNullException.ThrowIfNull(roundScores);
        var scores = roundScores.ToArray();
        if (scores.Length == 0)
        {
            return 0;
        }

        var totalWeight = scores.Sum(result => result.DifficultyWeight);
        if (totalWeight <= 0)
        {
            return 0;
        }

        var weightedScore = scores.Sum(result => ClampScore(result.Score) * result.DifficultyWeight);
        return ClampScore(weightedScore / totalWeight);
    }

    private static double ClampScore(double value) => Math.Clamp(value, 0d, 100d);
}
