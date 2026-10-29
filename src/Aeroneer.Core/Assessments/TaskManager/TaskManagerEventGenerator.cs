namespace Aeroneer.Core.Assessments.TaskManager;

/// <summary>Generates deterministic, independently scheduled Task Manager updates and alerts.</summary>
public sealed class TaskManagerEventGenerator
{
    public static readonly TimeSpan PracticeDuration = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan AssessmentDuration = TimeSpan.FromSeconds(180);
    public static readonly TimeSpan PrimaryUpdateExpiry = TimeSpan.FromSeconds(20);
    public static readonly TimeSpan AlertExpiry = TimeSpan.FromSeconds(10);

    private readonly Random _random;
    private int _eventSequence;

    /// <summary>Creates a scheduler with a deterministic seed.</summary>
    public TaskManagerEventGenerator(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>Creates overlapping primary and alert events, ordered by arrival time.</summary>
    public IReadOnlyList<TaskManagerEvent> Generate(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        var events = new List<TaskManagerEvent>();
        var primaryArrival = TimeSpan.FromSeconds(_random.Next(8, 16));
        while (primaryArrival < duration)
        {
            var altitude = _random.Next(10, 391) * 100;
            var heading = _random.Next(0, 360);
            var speed = _random.Next(100, 451);
            events.Add(new PrimaryUpdateEvent(
                NextId(),
                primaryArrival,
                primaryArrival + PrimaryUpdateExpiry,
                altitude,
                heading,
                speed));
            primaryArrival += TimeSpan.FromSeconds(_random.Next(24, 36));
        }

        var alertArrival = TimeSpan.FromSeconds(_random.Next(5, 13));
        while (alertArrival < duration)
        {
            events.Add(new AlertEvent(
                NextId(),
                alertArrival,
                alertArrival + AlertExpiry,
                $"ALT-{_random.Next(100, 1000)}"));
            alertArrival += TimeSpan.FromSeconds(_random.Next(12, 29));
        }

        return events.OrderBy(item => item.ArrivesAt).ThenBy(item => item.Id).ToArray();
    }

    private Guid NextId()
    {
        Span<byte> idBytes = stackalloc byte[16];
        BitConverter.TryWriteBytes(idBytes, ++_eventSequence);
        _random.NextBytes(idBytes[4..]);
        return new Guid(idBytes);
    }
}
