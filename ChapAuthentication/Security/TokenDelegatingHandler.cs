using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Services.Core.Security;
using Sitecore.Services.Infrastructure.Sitecore.Security;

namespace Westco.Services.Infrastructure.Security
{
    public class TokenDelegatingHandler : DelegatingHandler
    {
        private readonly IChapTokenProvider _tokenProvider;
        private readonly IUserService _userService;


        public TokenDelegatingHandler()
            : this(new ChapTokenProvider(), new UserService())
        {
        }

        protected TokenDelegatingHandler(IChapTokenProvider tokenProvider, IUserService userService)
        {
            _tokenProvider = tokenProvider;
            _userService = userService;
        }

        protected TokenDelegatingHandler(HttpMessageHandler innerHandler)
            : this(innerHandler, new ChapTokenProvider(), new UserService())
        {
        }

        protected TokenDelegatingHandler(HttpMessageHandler innerHandler, IChapTokenProvider tokenProvider,
            IUserService userService)
            : base(innerHandler)
        {
            _tokenProvider = tokenProvider;
            _userService = userService;
        }

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
            if (!_tokenProvider.ValidateRequest(requestBase)) return;

            var username = "sitecore\admin";
            if (_userService is IUserServiceEx userService)
                userService.SwitchToUser(username, true);
            else
                _userService.SwitchToUser(username);
        }
    }
}