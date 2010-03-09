using System;

namespace NunoGomes.CommunityServer.Blogs.Controls
{
    public class WeblogPostRating : global::CommunityServer.Blogs.Controls.WeblogPostRating
    {
        protected override void OnPreRender(EventArgs e)
        {
            if (((this.IsReadOnly || !this.GetCanRate()) && (this.GetRatingCount() == 0)) || !this.GetEnableRating())
            {
                this.AutomatedVisible = false;
            }

            this.UpdateVisible();
            if(this.Visible)
            {
                base.OnPreRender(e);
            }
        }
    }
}
