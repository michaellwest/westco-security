using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MicroCHAP;
using MicroCHAP.Server;
using Sitecore.Services.Core.Security;
using Sitecore.Services.Infrastructure.Sitecore.Security;
using Sitecore.Services.Infrastructure.Web.Http.Security;

namespace Westco.Services.Infrastructure.Security
{
    public class TokenDelegatingHandler : DelegatingHandler
    {
        private static IChapServer _server;
        private static ISignatureService _signatureService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUserService _userService;


        public TokenDelegatingHandler()
            : this(new ConfiguredOrNullTokenProvider(new ChapTokenProvider()), new UserService())
        {
        }

        protected TokenDelegatingHandler(ITokenProvider tokenProvider, IUserService userService)
        {
            _tokenProvider = tokenProvider;
            _userService = userService;
        }

        protected TokenDelegatingHandler(HttpMessageHandler innerHandler)
            : this(innerHandler, new ConfiguredOrNullTokenProvider(new ChapTokenProvider()), new UserService())
        {
        }

        protected TokenDelegatingHandler(HttpMessageHandler innerHandler, ITokenProvider tokenProvider,
            IUserService userService)
            : base(innerHandler)
        {
            _tokenProvider = tokenProvider;
            _userService = userService;
        }

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

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            AttemptLoginWithToken(request);
            return await base.SendAsync(request, cancellationToken);
        }

        private void AttemptLoginWithToken(HttpRequestMessage request)
        {
            if (!request.Headers.Contains("token")) return;

            var context = new HttpContextWrapper(HttpContext.Current);
            var requestBase = context.Request;

            if (!Server.ValidateRequest(requestBase)) return;
            
            var validationResult = _tokenProvider.ValidateToken(request.Headers.GetValues("token").FirstOrDefault());
            if (!validationResult.IsValid) return;

            var claims = validationResult.Claims;
            if (claims.Count(c => c.Type == "User") != 1) return;

            var username = validationResult.Claims.First(c => c.Type == "User").Value;
            if (_userService is IUserServiceEx userService)
                userService.SwitchToUser(username, true);
            else
                _userService.SwitchToUser(username);
        }
    }
}