using System.Net;
using System.Security.Claims;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Services.Core.Security;
using Sitecore.Services.Infrastructure.Sitecore.Security;
using Sitecore.Services.Infrastructure.Web.Http.Security;

namespace Westco.Services.Infrastructure.Mvc
{
    public class ChapAuthenticationController : Controller
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IUserService _userService;

        public ChapAuthenticationController()
            : this(new UserService(), new ChapTokenProvider())
        {
        }

        public ChapAuthenticationController(IUserService userService, ITokenProvider tokenProvider)
        {
            _userService = userService;
            _tokenProvider = tokenProvider;
        }

        [HttpPost]
        //[RequireHttps]
        public ActionResult ChallengeToken()
        {
            _userService.Login("sitecore", "admin", "b");
            var token = _tokenProvider.GenerateToken(new Claim[1]
            {
                new Claim("User", Context.User.Name)
            });
            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            Response.StatusCode = 200;
            return new JsonResult
            {
                Data = new {token}
            };
        }
    }
}