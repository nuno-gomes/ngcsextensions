using System.Collections.Generic;
using CommunityServer.Controls;
using NunoGomes.CommunityServer.Controls;

namespace NunoGomes.CommunityServer.Blogs.Controls
{
    public class WeblogPostCommentFormCaptchaAdapter : WrappedFormBaseCaptchaAdapter<WrappedFormBase>
    {
        #region Overriden Methods

        protected override List<string> ValidAnchorIds
        {
            get
            {
                List<string> validAnchorNames = base.ValidAnchorIds;
                validAnchorNames.Add("CommentSubmit");
                return validAnchorNames;
            }
        }

        protected override string DefaultValidationGroup
        {
            get { return "CreateCommentForm"; }
        }

        #endregion Overriden Methods
    }
}
