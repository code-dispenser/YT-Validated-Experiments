using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Validation.Core.Common.Constants;
using Validation.Core.Common.Utils;
using Validation.Core.Factories;
using Validation.Core.Types;

namespace Validation.Core.Validator;

public static class GenericValidator
{
    public static Validated<T> Validate<T>(T dataObject, ImmutableList<ValidationRuleConfig> configurations, ValidationFactoryProvider validationFactoryProvider, string tenantID) where T : notnull
    {
        var invalidEntries = ValidateProperties(dataObject, configurations, validationFactoryProvider, tenantID);
        return invalidEntries.Count == 0 ? Validated<T>.Valid(dataObject) : Validated<T>.Invalid(invalidEntries);
    }
    /*
        * You could use reflection and recursion to dynamically validate properties of any object, below is just a basic example that does not cover all cases such as List<T> or using the Option<T> instead of nullable types etc 
    */
    private static List<ValidationEntry> ValidateProperties<T>(T dataObject, ImmutableList<ValidationRuleConfig> configurations, ValidationFactoryProvider validationFactoryProvider, string tenantID) where T : notnull
    {
        var invalidEntries = new List<ValidationEntry>();

        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var typeName        = typeof(T).FullName!;
        var configsForType  = configurations.Where(c => c.TypeFullName == typeName && (c.TenantID == tenantID || c.TenantID == GlobalValues.Default_TenantID)).ToList();

        foreach (var propertyInfo in properties)
        {
            Type    propertyType = propertyInfo.PropertyType;
            var     propertyName = propertyInfo.Name;
            var     ruleConfigs  = TenantValidationHelper.GetTenantConfigs(typeName, propertyName, tenantID, configurations);
            object? value        = propertyInfo.GetValue(dataObject);

            if (IsPropertyNullable(propertyInfo) && value is null) continue;

            if (propertyType.IsClass  && propertyType != typeof(string) && !typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                var recursiveMethod = typeof(GenericValidator).GetMethod(nameof(ValidateProperties), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(propertyType);

                var validations = (List<ValidationEntry>)recursiveMethod.Invoke(null, [value,configurations, validationFactoryProvider, tenantID])!;
                invalidEntries.AddRange(validations);
            }

            if (ruleConfigs.Count == 0) continue;

            var method          = typeof(TenantValidationHelper).GetMethod(nameof(TenantValidationHelper.BuildValidator), BindingFlags.Public | BindingFlags.Static);
            var buildValidator  = method!.MakeGenericMethod(propertyType);
            var validatorFunc   = buildValidator.Invoke(null, [ruleConfigs, validationFactoryProvider])!;

            var validated = (IValidated)validatorFunc.GetType().GetMethod("Invoke")!.Invoke(validatorFunc, [value])!;

            if (validated.IsInvalid) invalidEntries.AddRange(validated.Failures);
        }

        return invalidEntries.ToList();
    }

    public static bool IsPropertyNullable(PropertyInfo property)// Tries to determine if either Nullable<T> or T? was used
    {

        if (property.PropertyType.IsValueType) return Nullable.GetUnderlyingType(property.PropertyType) != null;

        var nullableAttribute = property.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == typeof(NullableAttribute).FullName);

        if (nullableAttribute == null) return false;

        var attributeArg = nullableAttribute.ConstructorArguments.FirstOrDefault();

        if (attributeArg.ArgumentType == typeof(byte)) return (byte)attributeArg.Value! == 2;

        if (attributeArg.ArgumentType == typeof(byte[]))// for stuff like List<string?>
        {
            var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArg.Value!;
            
            if (args.Count > 0) return (byte)args[0].Value! == 2;
        }

        return false;

    }
}
