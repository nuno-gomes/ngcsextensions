using System.Configuration.Provider;
using System.Web.UI.WebControls;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    public abstract class CaptchaProvider : ProviderBase
    {
        public abstract void AddCaptchaControls(Panel captchaPanel, string validationGroup);
    }
}
