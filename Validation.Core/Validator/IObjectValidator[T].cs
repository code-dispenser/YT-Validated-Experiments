using System.Data;
using Validation.Core.Types;

namespace Validation.Core.Validator;

public interface IObjectValidator<T> where T : notnull, new()
{
    Validated<T> Validate(T dataObject);
}