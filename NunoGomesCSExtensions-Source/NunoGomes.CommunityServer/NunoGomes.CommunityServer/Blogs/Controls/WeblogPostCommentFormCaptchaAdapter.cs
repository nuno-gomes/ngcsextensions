using System.Collections.Generic;
using CommunityServer.Controls;
using NunoGomes.CommunityServer.Controls;

namespace NunoGomes.CommunityServer.Blogs.Controls
{
    public partial class WeblogPostCommentFormCaptchaAdapter : WrappedFormBaseCaptchaAdapter<WrappedFormBase>
    {
        private static RuntimeIntelligence _runtimeIntelligence = new RuntimeIntelligence();

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
