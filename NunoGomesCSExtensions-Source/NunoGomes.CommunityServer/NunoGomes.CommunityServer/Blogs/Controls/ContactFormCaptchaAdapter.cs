using CommunityServer.Controls;
using NunoGomes.CommunityServer.Controls;

namespace NunoGomes.CommunityServer.Blogs.Controls
{
    public partial class ContactFormCaptchaAdapter : WrappedFormBaseCaptchaAdapter<WrappedFormBase>
    {
        private static RuntimeIntelligence _runtimeIntelligence = new RuntimeIntelligence();

        #region Overriden Methods

        protected override string DefaultValidationGroup
        {
            get { return null; }
        }

        #endregion Overriden Methods
    }
}
