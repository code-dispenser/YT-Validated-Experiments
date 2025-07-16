namespace Validation.Contracts.DTOs;

/*
    * Could also just be in your Application project if not shared with say a Blazor/.net client 
*/
public record class ContactDto
{
    public string   Title        { get; set; } = default!;
    public string   GivenName    { get; set; } = default!;
    public string   FamilyName   { get; set; } = default!;
    public DateOnly DOB          { get; set; } = default!;
    public string   Email        { get; set; } = default!;
    public string?  Mobile       { get; set; }
    
    public AddressDto? Address { get; set; } 

}
