namespace Aeroneer.Core.Input;

/// <summary>Maps stable actions to platform-neutral key identifiers.</summary>
public sealed class InputBindingSet
{
    private readonly IReadOnlyDictionary<InputAction, string> _keys;

    /// <summary>Creates an immutable binding set from key enum names or other stable key identifiers.</summary>
    public InputBindingSet(IEnumerable<KeyValuePair<InputAction, string>> bindings)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        _keys = bindings.ToDictionary(
            pair => pair.Key,
            pair => string.IsNullOrWhiteSpace(pair.Value)
                ? throw new ArgumentException("Key identifiers cannot be empty.", nameof(bindings))
                : pair.Value.Trim());
    }

    /// <summary>Gets the bound key identifier for an action, if configured.</summary>
    public string? GetKey(InputAction action) => _keys.GetValueOrDefault(action);

    /// <summary>Gets configured action/key pairs.</summary>
    public IReadOnlyDictionary<InputAction, string> Bindings => _keys;
}
