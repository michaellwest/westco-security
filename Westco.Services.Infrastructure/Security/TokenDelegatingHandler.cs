using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Services.Core.Security;
using Sitecore.Services.Infrastructure.Security;

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
            AttemptLogin();
            return await base.SendAsync(request, cancellationToken);
        }

        private void AttemptLogin()
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            var requestBase = context.Request;
            var validatedResult = _tokenProvider.ValidateRequest(requestBase);
            if (!validatedResult.IsValid) return;

            var username = validatedResult.ApiUser;
            _userService.SwitchToUser(username, true);
        }
    }
}