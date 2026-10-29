namespace Aeroneer.Core.Assessments.TaskManager;

/// <summary>Scores primary-task transfer and secondary alert handling under dual load.</summary>
public static class TaskManagerScoring
{
    private const double PrimaryWeight = 0.60d;
    private const double AlertWeight = 0.40d;
    private const double PrimaryAccuracyWeight = 0.70d;
    private const double PrimarySpeedWeight = 0.30d;
    private const double AlertAccuracyWeight = 0.70d;
    private const double AlertSpeedWeight = 0.30d;

    /// <summary>Scores each update and alert, including missed events as zero-credit outcomes.</summary>
    public static TaskManagerScore Calculate(
        IEnumerable<PrimaryUpdateEvent> updates,
        IEnumerable<PrimaryUpdateResponse> updateResponses,
        IEnumerable<AlertEvent> alerts,
        IEnumerable<AlertResponse> alertResponses)
    {
        ArgumentNullException.ThrowIfNull(updates);
        ArgumentNullException.ThrowIfNull(updateResponses);
        ArgumentNullException.ThrowIfNull(alerts);
        ArgumentNullException.ThrowIfNull(alertResponses);
        var updateList = updates.ToArray();
        var updateResponseMap = updateResponses.ToDictionary(response => response.EventId);
        var alertList = alerts.ToArray();
        var alertResponseMap = alertResponses.ToDictionary(response => response.EventId);

        var primaryAccuracy = updateList.Length == 0 ? 0d : updateList.Count(update =>
            updateResponseMap.TryGetValue(update.Id, out var response)
            && !response.Expired
            && response.RespondedAt.HasValue
            && response.RespondedAt.Value >= update.ArrivesAt
            && response.RespondedAt.Value <= update.ExpiresAt
            && response.AltitudeFeet == update.AltitudeFeet
            && response.HeadingDegrees == update.HeadingDegrees
            && response.SpeedKnots == update.SpeedKnots) * 100d / updateList.Length;
        var primarySpeed = AverageSpeed(updateList.Select<PrimaryUpdateEvent, TimeSpan?>(update =>
            updateResponseMap.TryGetValue(update.Id, out var response)
                && response.RespondedAt.HasValue
                && !response.Expired
                && response.RespondedAt.Value >= update.ArrivesAt
                && response.RespondedAt.Value <= update.ExpiresAt
                ? response.RespondedAt.Value - update.ArrivesAt
                : null), TaskManagerEventGenerator.PrimaryUpdateExpiry);

        var alertAccuracy = alertList.Length == 0 ? 0d : alertList.Count(alert =>
            alertResponseMap.TryGetValue(alert.Id, out var response)
            && !response.Expired
            && response.AcknowledgedAt.HasValue
            && response.AcknowledgedAt.Value >= alert.ArrivesAt
            && response.AcknowledgedAt.Value <= alert.ExpiresAt) * 100d / alertList.Length;
        var alertSpeed = AverageSpeed(alertList.Select<AlertEvent, TimeSpan?>(alert =>
            alertResponseMap.TryGetValue(alert.Id, out var response)
                && response.AcknowledgedAt.HasValue
                && !response.Expired
                && response.AcknowledgedAt.Value >= alert.ArrivesAt
                && response.AcknowledgedAt.Value <= alert.ExpiresAt
                ? response.AcknowledgedAt.Value - alert.ArrivesAt
                : null), TaskManagerEventGenerator.AlertExpiry);

        var primaryScore = primaryAccuracy * PrimaryAccuracyWeight + primarySpeed * PrimarySpeedWeight;
        var alertScore = alertAccuracy * AlertAccuracyWeight + alertSpeed * AlertSpeedWeight;
        var overall = updateList.Length == 0 && alertList.Length == 0
            ? 0d
            : primaryScore * PrimaryWeight + alertScore * AlertWeight;
        var primaryResponseTimes = updateList.Select(update =>
                updateResponseMap.TryGetValue(update.Id, out var response)
                    && response.RespondedAt.HasValue
                    && !response.Expired
                    && response.RespondedAt.Value >= update.ArrivesAt
                    && response.RespondedAt.Value <= update.ExpiresAt
                    ? (double?)(response.RespondedAt.Value - update.ArrivesAt).TotalMilliseconds
                    : null)
            .Where(value => value.HasValue && value.Value >= 0)
            .Select(value => value!.Value)
            .ToArray();
        var alertResponseTimes = alertList.Select(alert =>
                alertResponseMap.TryGetValue(alert.Id, out var response)
                    && response.AcknowledgedAt.HasValue
                    && !response.Expired
                    && response.AcknowledgedAt.Value >= alert.ArrivesAt
                    && response.AcknowledgedAt.Value <= alert.ExpiresAt
                    ? (double?)(response.AcknowledgedAt.Value - alert.ArrivesAt).TotalMilliseconds
                    : null)
            .Where(value => value.HasValue && value.Value >= 0)
            .Select(value => value!.Value)
            .ToArray();

        return new TaskManagerScore(
            Clamp(overall),
            Clamp(primaryAccuracy),
            Clamp(primarySpeed),
            Clamp(alertAccuracy),
            Clamp(alertSpeed),
            primaryResponseTimes.Length == 0 ? 0 : primaryResponseTimes.Average(),
            alertResponseTimes.Length == 0 ? 0 : alertResponseTimes.Average(),
            updateList.Count(update => !updateResponseMap.TryGetValue(update.Id, out var response) || response.Expired),
            alertList.Count(alert => !alertResponseMap.TryGetValue(alert.Id, out var response) || response.Expired));
    }

    private static double AverageSpeed(IEnumerable<TimeSpan?> responseTimes, TimeSpan timeout)
    {
        var durations = responseTimes.ToArray();
        if (durations.Length == 0 || timeout <= TimeSpan.Zero)
        {
            return 0;
        }

        var speed = durations.Sum(duration => duration.HasValue && duration.Value >= TimeSpan.Zero
            ? 100d * (1d - Math.Clamp(duration.Value.TotalMilliseconds / timeout.TotalMilliseconds, 0d, 1d))
            : 0d);
        return Math.Clamp(speed / durations.Length, 0d, 100d);
    }

    private static double Clamp(double value) => Math.Clamp(value, 0d, 100d);
}
