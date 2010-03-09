using System;

namespace NunoGomes.CommunityServer.Controls
{
    public class RateableContentRating : global::CommunityServer.Controls.RateableContentRating
    {
        protected override void OnPreRender(EventArgs e)
        {
            if (((this.IsReadOnly || !this.GetCanRate()) && (this.GetRatingCount() == 0)) || !this.GetEnableRating())
            {
                this.AutomatedVisible = false;
            }

            this.UpdateVisible();
            if (this.Visible)
            {
                base.OnPreRender(e);
            }
        }
    }
}
