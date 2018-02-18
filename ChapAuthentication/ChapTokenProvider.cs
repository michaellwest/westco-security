using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using MicroCHAP;
using MicroCHAP.Server;
using Sitecore.Exceptions;
using Sitecore.Services.Infrastructure.Sitecore.Security;
using Sitecore.Services.Infrastructure.Web.Http.Security;

namespace Westco.Services.Infrastructure
{
    internal class ChapTokenProvider : IChapTokenProvider
    {
        private static IChapServer _server;
        private static ISignatureService _signatureService;

        private readonly string _challengeDatabase = Sitecore.Configuration.Settings.GetSetting("Westco.Services.Infrastructure.ChapTokenProvider.ChallengeDatabase");

        private string _sharedSecret = Sitecore.Configuration.Settings.GetSetting("Westco.Services.Infrastructure.ChapTokenProvider.SharedSecret");

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

        public bool ValidateRequest(HttpRequestBase request)
        {
            return Server.ValidateRequest(request);
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
            {
                // if no shared secret is set we make it a random double, but we reject that once you actually try to authenticate with a tool
                throw new SecurityException(
                    "The shared secret is not set, or was set to a numeric value.");
            }

            if (_sharedSecret.Length < 30)
                throw new SecurityException(
                    "Your shared secret is not long enough. Please make it more than 30 characters for maximum security.");
        }
    }
}