using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recaptcha;

namespace NunoGomes.CommunityServer.Recaptcha
{
    internal static class RecaptchaManager
    {
        private static Version _reCaptchaAPIVersion = typeof(RecaptchaControl).Assembly.GetName().Version;

        public static bool SupportsErrorMessage
        {
            get
            {
                if (_reCaptchaAPIVersion.Major == 1 && _reCaptchaAPIVersion.Build < 4)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
