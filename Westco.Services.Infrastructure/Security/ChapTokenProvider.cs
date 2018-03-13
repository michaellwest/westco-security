using System;
using System.Globalization;
using System.Web;
using MicroCHAP;
using MicroCHAP.Server;
using Sitecore.Configuration;
using Sitecore.Exceptions;

namespace Westco.Services.Infrastructure.Security
{
    internal class ChapTokenProvider : IChapTokenProvider
    {
        private static IChapServer _server;
        private static ISignatureService _signatureService;

        private readonly string _challengeDatabase =
            Settings.GetSetting("Westco.Services.Infrastructure.Security.ChapTokenProvider.ChallengeDatabase");

        private string _sharedSecret =
            Settings.GetSetting("Westco.Services.Infrastructure.Security.ChapTokenProvider.SharedSecret");

        private readonly string _apiUser =
            Settings.GetSetting("Westco.Services.Infrastructure.Security.ChapTokenProvider.ApiUser");

        protected virtual IChapServer Server => _server ?? (_server = new ChapServer(SignatureService, ChallengeStore));

        protected virtual ISignatureService SignatureService
        {
            get
            {
                // if no shared secret is set, we set a random double. This essentially renders it unusable.
                // we verify that the shared secret is not numeric before actually using it for tool authentication.
                if (string.IsNullOrWhiteSpace(_sharedSecret))
                    _sharedSecret = new Random().NextDouble().ToString(CultureInfo.InvariantCulture);

                return _signatureService ?? (_signatureService = new SignatureService(_sharedSecret));
            }
        }

        protected virtual IChallengeStore ChallengeStore => new SitecoreDatabaseChallengeStore(_challengeDatabase);

        protected virtual int RequestTimeoutInMs => 1000 * 60 * 120; // 2h in msec

        public IChapValidationResult ValidateRequest(HttpRequestBase request)
        {
            return new ChapValidatedResult
            {
                ApiUser = _apiUser,
                IsValid = Server.ValidateRequest(request)
            };
        }

        public string GenerateChallenge()
        {
            ValidateSharedSecret();
            return Server.GetChallengeToken();
        }

        public virtual void ValidateSharedSecret()
        {
            if (string.IsNullOrWhiteSpace(_sharedSecret))
                throw new SecurityException(
                    "The shared secret is not set.");

            if (double.TryParse(_sharedSecret, out _))
                throw new SecurityException(
                    "The shared secret is not set, or was set to a numeric value.");

            if (_sharedSecret.Length < 30)
                throw new SecurityException(
                    "Your shared secret is not long enough. Please make it more than 30 characters for maximum security.");
        }
    }
}