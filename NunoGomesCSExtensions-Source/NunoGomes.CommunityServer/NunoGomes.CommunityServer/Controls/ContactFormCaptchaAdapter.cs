using CommunityServer.Controls;

namespace NunoGomes.CommunityServer.Controls
{
    public class ContactFormCaptchaAdapter : WrappedFormBaseCaptchaAdapter<WrappedFormBase>
    {
        #region Overriden Methods

        protected override string DefaultValidationGroup
        {
            get { return null; }
        }

        #endregion Overriden Methods
    }
}
