namespace Validation.Core.Types;

public sealed record class Validated<T> where T : notnull
{
    private readonly IReadOnlyList<ValidationEntry> _failures;
    private readonly T? _value;
    public IEnumerable<ValidationEntry> Failures => _failures;
    public bool IsValid { get; }
    public bool IsInvalid => !IsValid;

    private Validated(T value)

        => (_value, _failures, IsValid) = value is null
                ? (default, [new(nameof(value), nameof(value), "Value cannot be null.")], false)
                    : (value, new List<ValidationEntry>(), true);

    private Validated(IEnumerable<ValidationEntry> failures)
    {
        var filteredFailures = failures?.Where(failure => !string.IsNullOrEmpty(failure.PropertyName) && !string.IsNullOrEmpty(failure.Message)).ToList() ?? [];
        _failures            = filteredFailures.Count > 0 ? filteredFailures.AsReadOnly() : [new ValidationEntry("Unknown", "Unknown", "No validation failures provided.")];
        _value               = default!;
        IsValid              = false;
    }

    public static Validated<T> Valid(T value)

        => new Validated<T>(value);

    public static Validated<T> Invalid(IEnumerable<ValidationEntry> failures)

        => new Validated<T>(failures);

    public static Validated<T> Invalid(ValidationEntry failure)

        => new Validated<T>([failure]);

    public T GetValueOr(T fallback)

        => IsValid ? _value! : fallback;

    public TOut Match<TOut>(Func<IEnumerable<ValidationEntry>, TOut> onInvalid, Func<T, TOut> onValid)

        => IsValid ? onValid(_value!) : onInvalid(Failures);

    public void Match(Action<IEnumerable<ValidationEntry>> onInvalid, Action<T> onValid)
    {
        if (true == IsValid) onValid(_value!); else onInvalid(Failures);
    }

    public Validated<TOut> Map<TOut>(Func<T, TOut> onValid) where TOut : notnull

        => IsValid ? Validated<TOut>.Valid(onValid(_value!)) : Validated<TOut>.Invalid(Failures);

}
