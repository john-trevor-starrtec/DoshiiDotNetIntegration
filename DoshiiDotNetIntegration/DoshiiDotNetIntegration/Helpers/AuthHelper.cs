using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Helpers
{
    internal static class AuthHelper
    {
        internal static string CreateToken(string locationToken, string secretKey)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //var now = Math.Round((DateTime.Now - unixEpoch).TotalSeconds);
            var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);

            var payload = new Dictionary<string, object>()
            {
                //{"locationId", LocationId}, //locationId of the location connected to Doshii
                {"locationToken", locationToken},
                {"timestamp", now}
            };

            return string.Format("Bearer {0}", JWT.JsonWebToken.Encode(payload, secretKey, JWT.JwtHashAlgorithm.HS256));
        }
    }
}
