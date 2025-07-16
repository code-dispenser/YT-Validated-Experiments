using Validation.Core.Common.Constants;

namespace Validation.Core.Factories;

public class ValidationFactoryProvider
{
    private readonly Dictionary<RuleType, IValidatorFactory> _validationFactories;

    public ValidationFactoryProvider()
    {
        _validationFactories = new()
        {
            [RuleType.Regex]        = new RegexValidatorFactory(),
            [RuleType.Between]      = new BetweenValidatorFactory(),
            [RuleType.Length]       = new LengthValidatorFactory(),
            [RuleType.RollingDate]  = new RollingDateOnlyValidatorFactory(() => DateOnly.FromDateTime(DateTime.UtcNow)),
        };
    }

    public IValidatorFactory GetValidatorFactory(RuleType ruleType)

        => _validationFactories[ruleType];

}