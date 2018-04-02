using System;
using Lib.WebApi.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lib.WebApi
{
    public abstract class BaseController : Controller
    {
        public readonly JwtIssuerOptions JwtOptions;

        public BaseController(IOptions<JwtIssuerOptions> jwtOptions)
        {
            JwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions();
        }

        void ThrowIfInvalidOptions()
        {
            if (JwtOptions == null)
                throw new ArgumentNullException(nameof(JwtOptions));

            if (JwtOptions.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (JwtOptions.JtiGenerator == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
    }
}
