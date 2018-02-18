namespace Westco.Services.Infrastructure.Security
{
    public class ChapValidatedResult : IChapValidationResult
    {
        public string ApiUser { get; set; }
        public bool IsValid { get; set; }
    }
}