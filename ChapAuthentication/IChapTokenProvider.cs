using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Westco.Services.Infrastructure
{
    public interface IChapTokenProvider
    {
        string GenerateChallenge();
        void ValidateSharedSecret();
        bool ValidateRequest(HttpRequestBase request);
    }
}
