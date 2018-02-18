using System.Net;
using System.Web.Mvc;

namespace Westco.Services.Infrastructure.Mvc
{
    public class ChapAuthenticationController : Controller
    {
        private readonly IChapTokenProvider _tokenProvider;

        public ChapAuthenticationController()
            : this(new ChapTokenProvider())
        {
        }

        public ChapAuthenticationController(IChapTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        [RequireHttps]
        public ActionResult Challenge()
        {
            var challenge = _tokenProvider.GenerateChallenge();
            if (challenge == null)
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            Response.StatusCode = 200;
            return new JsonResult
            {
                Data = new {challenge}, JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}