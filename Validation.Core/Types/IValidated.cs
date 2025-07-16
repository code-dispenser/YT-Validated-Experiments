namespace Validation.Core.Types
{
    public interface IValidated
    {
        IEnumerable<ValidationEntry> Failures { get; }
        public bool IsInvalid                 { get; } 
    }
}
