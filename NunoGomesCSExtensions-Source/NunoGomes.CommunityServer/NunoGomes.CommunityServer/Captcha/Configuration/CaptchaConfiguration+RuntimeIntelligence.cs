using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NunoGomes.CommunityServer.Captcha.Configuration
{
    public static partial class CaptchaConfiguration
    {
        private class RuntimeIntelligence
        {
            public RuntimeIntelligence()
            {
                Setup();
            }

            ~RuntimeIntelligence()
            {
                Teardown();
            }

            //[PreEmptive.Attributes.Setup(CustomEndpoint = "so-s.info/PreEmptive.Web.Services.Messaging/MessagingServiceV2.asmx")]
            private void Setup()
            { }

            //[PreEmptive.Attributes.Teardown()]
            private void Teardown()
            { }
        }
    }
}
