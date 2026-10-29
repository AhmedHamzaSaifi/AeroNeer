namespace Aeroneer.Core.Input;

/// <summary>Provides default keyboard presets and rejects ambiguous bindings.</summary>
public sealed class InputBindingService
{
    private static readonly InputAction[][] SimultaneousActionGroups =
    [
        [InputAction.ControlLeft, InputAction.ControlRight, InputAction.ControlUp, InputAction.ControlDown],
        [InputAction.SlalomLeft, InputAction.SlalomRight],
        [InputAction.OrientationAnswer1, InputAction.OrientationAnswer2, InputAction.OrientationAnswer3, InputAction.OrientationAnswer4]
    ];

    /// <summary>Creates the suggested arrow-key and number-key defaults.</summary>
    public InputBindingSet CreateDefaultBindings() => CreateBindings(
        (InputAction.ControlLeft, "Left"),
        (InputAction.ControlRight, "Right"),
        (InputAction.ControlUp, "Up"),
        (InputAction.ControlDown, "Down"),
        (InputAction.SlalomLeft, "Left"),
        (InputAction.SlalomRight, "Right"),
        (InputAction.OrientationAnswer1, "D1"),
        (InputAction.OrientationAnswer2, "D2"),
        (InputAction.OrientationAnswer3, "D3"),
        (InputAction.OrientationAnswer4, "D4"),
        (InputAction.Submit, "Enter"),
        (InputAction.PauseOrExit, "Escape"),
        (InputAction.AcknowledgeAlert, "Space"));

    /// <summary>Creates the alternative WASD movement preset while preserving common actions.</summary>
    public InputBindingSet CreateWasdPreset() => CreateBindings(
        (InputAction.ControlLeft, "A"),
        (InputAction.ControlRight, "D"),
        (InputAction.ControlUp, "W"),
        (InputAction.ControlDown, "S"),
        (InputAction.SlalomLeft, "A"),
        (InputAction.SlalomRight, "D"),
        (InputAction.OrientationAnswer1, "D1"),
        (InputAction.OrientationAnswer2, "D2"),
        (InputAction.OrientationAnswer3, "D3"),
        (InputAction.OrientationAnswer4, "D4"),
        (InputAction.Submit, "Enter"),
        (InputAction.PauseOrExit, "Escape"),
        (InputAction.AcknowledgeAlert, "Space"));

    /// <summary>Returns actionable conflicts when bindings overlap within a simultaneous action group.</summary>
    public IReadOnlyList<string> FindConflicts(InputBindingSet bindings)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        var conflicts = new List<string>();
        foreach (var group in SimultaneousActionGroups)
        {
            var duplicateKeys = group
                .Select(action => (Action: action, Key: bindings.GetKey(action)))
                .Where(binding => binding.Key is not null)
                .GroupBy(binding => binding.Key!, StringComparer.OrdinalIgnoreCase)
                .Where(keys => keys.Count() > 1);

            foreach (var duplicate in duplicateKeys)
            {
                var actions = string.Join(", ", duplicate.Select(binding => binding.Action));
                conflicts.Add($"Key '{duplicate.Key}' is assigned to multiple simultaneous actions: {actions}.");
            }
        }

        return conflicts;
    }

    private static InputBindingSet CreateBindings(params (InputAction Action, string Key)[] bindings) =>
        new(bindings.Select(binding => new KeyValuePair<InputAction, string>(binding.Action, binding.Key)));
}
