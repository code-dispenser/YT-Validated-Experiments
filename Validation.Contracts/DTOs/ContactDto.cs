namespace Validation.Contracts.DTOs;

/*
    * Could also just be in your Application project if not shared with say a Blazor/.net client 
*/
public class ClientDto
{
    public string   Title       { get; set; } = default!;
    public string   GivenName   { get; set; } = default!;
    public string   FamilyName  { get; set; } = default!;
    public DateTime DOB         { get; set; } = default!;
    public string   Email       { get; set; } = default!;
    public string   PhoneNumber { get; set; } = default!;

}
