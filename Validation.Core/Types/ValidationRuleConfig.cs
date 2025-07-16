using Validation.Core.Common.Constants;

namespace Validation.Core.Types;

public record class ValidationRuleConfig(string TypeFullName, string PropertyName, string DisplayName, RuleType RuleType, MinMaxValueType MinMaxValueType, string Pattern, string FailureMessage,
                                        int MinLength, int MaxLength, string MinValue = "", string MaxValue = "", string TenantID = GlobalValues.Default_TenantID);