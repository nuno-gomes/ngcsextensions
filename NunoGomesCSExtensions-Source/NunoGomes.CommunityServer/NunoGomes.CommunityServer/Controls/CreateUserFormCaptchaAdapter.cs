using System.Collections.Generic;
using CommunityServer.Controls;

namespace NunoGomes.CommunityServer.Controls
{
    public class CreateUserFormCaptchaAdapter : WrappedFormBaseCaptchaAdapter<WrappedFormBase>
    {
        #region Overriden Methods

        protected override List<string> ValidAnchorIds
        {
            get
            {
                List<string> validAnchorNames = base.ValidAnchorIds;
                validAnchorNames.Insert(0, "CreateAccount");
                return validAnchorNames;
            }
        }

        protected override string DefaultValidationGroup
        {
            get { return ""; }
        }

        #endregion Overriden Methods
    }
}
