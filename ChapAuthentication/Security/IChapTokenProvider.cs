using System.Web;

namespace Westco.Services.Infrastructure.Security
{
    public interface IChapTokenProvider
    {
        string GenerateChallenge();
        void ValidateSharedSecret();
        IChapValidationResult ValidateRequest(HttpRequestBase request);
    }
}