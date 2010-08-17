using System.Collections.Generic;
using CommunityServer.Controls;

namespace NunoGomes.CommunityServer.Controls
{
    public partial class OpenIdCreateUserSubFormCaptchaAdapter : WrappedFormBaseCaptchaAdapter<WrappedFormBase>
    {
        private static RuntimeIntelligence _runtimeIntelligence = new RuntimeIntelligence();

        #region Overriden Methods

        protected override List<string> ValidAnchorIds
        {
            get
            {
                List<string> validAnchorNames = base.ValidAnchorIds;
                validAnchorNames.Insert(0, "verifyOpenId");
                return validAnchorNames;
            }
        }

        protected override string DefaultValidationGroup
        {
            get { return "OpenIdGroup"; }
        }

        #endregion Overriden Methods
    }
}
