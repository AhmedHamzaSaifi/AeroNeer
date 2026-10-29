namespace Aeroneer.Core.Assessments.TaskManager;

/// <summary>One event scheduled during a Task Manager attempt.</summary>
public abstract record TaskManagerEvent(Guid Id, TimeSpan ArrivesAt, TimeSpan ExpiresAt);

/// <summary>New aircraft values the candidate must transfer to the autopilot.</summary>
public sealed record PrimaryUpdateEvent(
    Guid Id,
    TimeSpan ArrivesAt,
    TimeSpan ExpiresAt,
    int AltitudeFeet,
    int HeadingDegrees,
    int SpeedKnots) : TaskManagerEvent(Id, ArrivesAt, ExpiresAt);

/// <summary>An alert requiring keyboard acknowledgment before it expires.</summary>
public sealed record AlertEvent(
    Guid Id,
    TimeSpan ArrivesAt,
    TimeSpan ExpiresAt,
    string AlertCode) : TaskManagerEvent(Id, ArrivesAt, ExpiresAt);

/// <summary>Candidate response to a scheduled primary-task update.</summary>
public sealed record PrimaryUpdateResponse(
    Guid EventId,
    int? AltitudeFeet,
    int? HeadingDegrees,
    int? SpeedKnots,
    TimeSpan? RespondedAt,
    bool Expired);

/// <summary>Candidate response to a scheduled secondary-task alert.</summary>
public sealed record AlertResponse(
    Guid EventId,
    TimeSpan? AcknowledgedAt,
    bool Expired);

/// <summary>Final accuracy, response-time, and score metrics for Task Manager.</summary>
public sealed record TaskManagerScore(
    double Score,
    double PrimaryAccuracyPercent,
    double PrimarySpeedPercent,
    double AlertAccuracyPercent,
    double AlertSpeedPercent,
    double MeanPrimaryResponseMilliseconds,
    double MeanAlertResponseMilliseconds,
    int ExpiredUpdates,
    int MissedAlerts);
