namespace Aeroneer.Core.Assessments.Control;

/// <summary>Metrics and bounded score components for a Control attempt.</summary>
public sealed record ControlMetrics(
    double Score,
    double TimeOnTargetPercent,
    double HorizontalTimeOnTargetPercent,
    double VerticalTimeOnTargetPercent,
    double MeanAbsoluteDeviation,
    double HorizontalMeanAbsoluteDeviation,
    double VerticalMeanAbsoluteDeviation,
    double RmsError,
    int HorizontalCorrections,
    int VerticalCorrections,
    double SmoothnessPercent,
    double AxisBalancePercent);

/// <summary>Calculates documented Control performance metrics from downsampled trajectory data.</summary>
public static class ControlScoring
{
    private const double TargetTolerance = ControlSimulation.TargetTolerance;
    private const double CorrectionDebounceSeconds = 0.4d;
    private const double CorrectionDirectionThreshold = 0.035d;
    private const double ScoreTimeOnTargetWeight = 0.45d;
    private const double ScoreDeviationWeight = 0.25d;
    private const double ScoreSmoothnessWeight = 0.15d;
    private const double ScoreAxisBalanceWeight = 0.15d;

    /// <summary>Calculates time-on-target, deviation, RMS, corrections, smoothness, balance, and total score.</summary>
    public static ControlMetrics Calculate(IEnumerable<ControlTrajectorySample> trajectory)
    {
        ArgumentNullException.ThrowIfNull(trajectory);
        var samples = trajectory.OrderBy(sample => sample.ElapsedSeconds).ToArray();
        if (samples.Length == 0)
        {
            return new ControlMetrics(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        var horizontalErrors = samples.Select(sample => Math.Abs(sample.HorizontalDeviation)).ToArray();
        var verticalErrors = samples.Select(sample => Math.Abs(sample.VerticalDeviation)).ToArray();
        var horizontalMean = horizontalErrors.Average();
        var verticalMean = verticalErrors.Average();
        var meanDeviation = (horizontalMean + verticalMean) / 2d;
        var rms = Math.Sqrt(samples.Average(sample =>
            (sample.HorizontalDeviation * sample.HorizontalDeviation + sample.VerticalDeviation * sample.VerticalDeviation) / 2d));
        var horizontalOnTarget = horizontalErrors.Count(error => error <= TargetTolerance) * 100d / samples.Length;
        var verticalOnTarget = verticalErrors.Count(error => error <= TargetTolerance) * 100d / samples.Length;
        var onTarget = (horizontalOnTarget + verticalOnTarget) / 2d;
        var deviationScore = (1d - Math.Clamp(meanDeviation, 0d, 1d)) * 100d;
        var smoothness = CalculateSmoothness(samples);
        var axisBalance = CalculateAxisBalance(horizontalMean, verticalMean);
        var score = onTarget * ScoreTimeOnTargetWeight
            + deviationScore * ScoreDeviationWeight
            + smoothness * ScoreSmoothnessWeight
            + axisBalance * ScoreAxisBalanceWeight;
        var corrections = CountCorrections(samples);

        return new ControlMetrics(
            Clamp(score),
            Clamp(onTarget),
            Clamp(horizontalOnTarget),
            Clamp(verticalOnTarget),
            Clamp(meanDeviation),
            Clamp(horizontalMean),
            Clamp(verticalMean),
            Clamp(rms),
            corrections.Horizontal,
            corrections.Vertical,
            Clamp(smoothness),
            Clamp(axisBalance));
    }

    private static double CalculateSmoothness(IReadOnlyList<ControlTrajectorySample> samples)
    {
        if (samples.Count < 3)
        {
            return 100d;
        }

        var motionChangeTotal = 0d;
        var motionChangeCount = 0;
        for (var index = 2; index < samples.Count; index++)
        {
            var firstInterval = samples[index - 1].ElapsedSeconds - samples[index - 2].ElapsedSeconds;
            var secondInterval = samples[index].ElapsedSeconds - samples[index - 1].ElapsedSeconds;
            if (firstInterval <= 0 || secondInterval <= 0)
            {
                continue;
            }

            var horizontalMotionChange = Math.Abs(
                (samples[index].HorizontalDeviation - 2 * samples[index - 1].HorizontalDeviation + samples[index - 2].HorizontalDeviation)
                / Math.Max(firstInterval, secondInterval));
            var verticalMotionChange = Math.Abs(
                (samples[index].VerticalDeviation - 2 * samples[index - 1].VerticalDeviation + samples[index - 2].VerticalDeviation)
                / Math.Max(firstInterval, secondInterval));
            motionChangeTotal += (horizontalMotionChange + verticalMotionChange) / 2d;
            motionChangeCount++;
        }

        return motionChangeCount == 0
            ? 100d
            : 100d * (1d - Math.Clamp(motionChangeTotal / motionChangeCount, 0d, 1d));
    }

    private static double CalculateAxisBalance(double horizontalMean, double verticalMean)
    {
        var maximumError = Math.Max(horizontalMean, verticalMean);
        return maximumError <= double.Epsilon
            ? 100d
            : 100d * (1d - Math.Abs(horizontalMean - verticalMean) / maximumError);
    }

    private static (int Horizontal, int Vertical) CountCorrections(IReadOnlyList<ControlTrajectorySample> samples)
    {
        var horizontal = 0;
        var vertical = 0;
        var lastHorizontalDirection = 0;
        var lastVerticalDirection = 0;
        var lastHorizontalCorrection = double.NegativeInfinity;
        var lastVerticalCorrection = double.NegativeInfinity;

        for (var index = 1; index < samples.Count; index++)
        {
            var previous = samples[index - 1];
            var current = samples[index];
            var horizontalDirection = Direction(current.HorizontalDeviation - previous.HorizontalDeviation);
            var verticalDirection = Direction(current.VerticalDeviation - previous.VerticalDeviation);
            if (horizontalDirection != 0 && lastHorizontalDirection != 0 && horizontalDirection != lastHorizontalDirection
                && current.ElapsedSeconds - lastHorizontalCorrection >= CorrectionDebounceSeconds)
            {
                horizontal++;
                lastHorizontalCorrection = current.ElapsedSeconds;
            }

            if (verticalDirection != 0 && lastVerticalDirection != 0 && verticalDirection != lastVerticalDirection
                && current.ElapsedSeconds - lastVerticalCorrection >= CorrectionDebounceSeconds)
            {
                vertical++;
                lastVerticalCorrection = current.ElapsedSeconds;
            }

            if (horizontalDirection != 0)
            {
                lastHorizontalDirection = horizontalDirection;
            }

            if (verticalDirection != 0)
            {
                lastVerticalDirection = verticalDirection;
            }
        }

        return (horizontal, vertical);
    }

    private static int Direction(double delta) =>
        delta > CorrectionDirectionThreshold ? 1 : delta < -CorrectionDirectionThreshold ? -1 : 0;

    private static double Clamp(double value) => Math.Clamp(value, 0d, 100d);
}
