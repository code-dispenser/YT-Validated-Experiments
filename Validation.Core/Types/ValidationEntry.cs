namespace Validation.Core.Types;

public readonly record struct ValidationEntry(string PropertyName, string DisplayName, string Message);

