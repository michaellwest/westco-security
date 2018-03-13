namespace Westco.Services.Infrastructure.Security
{
    public interface IChapValidationResult
    {
        string ApiUser { get; set; }
        bool IsValid { get; set; }
    }
}