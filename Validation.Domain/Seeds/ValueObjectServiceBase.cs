using Validation.Core.Types;
using Validation.Domain.ValueObjects;

namespace Validation.Domain.Seeds;

public abstract class ValueObjectServiceBase
{
    protected Validated<FullName> CreateFullName(Validated<string> validatedTitle, Validated<string> validatedGivenName, Validated<string> validatedFamilyName)

        => FullName.InternalCreate(validatedTitle, validatedGivenName, validatedFamilyName);
}
