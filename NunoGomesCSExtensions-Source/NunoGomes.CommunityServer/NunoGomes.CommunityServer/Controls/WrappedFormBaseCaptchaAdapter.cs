using System;
using System.Collections.Generic;
using System.Web.UI.Adapters;
using System.Web.UI.WebControls;
using System.Web;
using CommunityServer.Controls;
using System.Web.UI;
using System.Linq;
using NunoGomes.CommunityServer.Captcha.Configuration;

namespace NunoGomes.CommunityServer.Controls
{
    public abstract class WrappedFormBaseCaptchaAdapter<T> : ControlAdapter where T : WrappedFormBase
    {
        #region Overriden Methods

        protected sealed override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.Page.IsPostBack && this.IsCaptchaRequired)
            {
                this.Page.Validate();
            }
        }

        protected sealed override void CreateChildControls()
        {
            base.CreateChildControls();

            if (this.IsCaptchaRequired)
            {
                T wrapper = base.Control as T;
                if (wrapper != null)
                {
                    Control anchorControl = GetAnchorControl(wrapper);
                    if (anchorControl != null)
                    {
                        Panel phCaptcha = new Panel {CssClass = "CommonFormField", ID = "Captcha"};

                        int index = anchorControl.Parent.Controls.IndexOf(anchorControl);
                        anchorControl.Parent.Controls.AddAt(index, phCaptcha);

                        CaptchaConfiguration.DefaultProvider.AddCaptchaControls(
                            phCaptcha,
                            GetValidationGroup(wrapper, anchorControl));
                    }
                }
            }
        }

        #endregion Overriden Methods

        #region Private Members

        private bool IsCaptchaRequired
        {
            get
            {
                return (!HttpContext.Current.User.Identity.IsAuthenticated);
            }
        }

        private Control GetAnchorControl(T wrapper)
        {
            if (this.ValidAnchorIds == null || this.ValidAnchorIds.Count == 0)
            {
                throw new ArgumentException(NunoGomes.CommunityServer.Resources.CaptchaAnchor_Invalid, "ValidAnchorNames");
            }

            var q = from anchorId in this.ValidAnchorIds
                    let anchorControl = CSControlUtility.Instance().FindControl(wrapper, anchorId)
                    where anchorControl != null
                    select anchorControl;

            return q.FirstOrDefault();
        }

        private string GetValidationGroup(T wrapper, Control anchorControl)
        {
            string validationGroup = null;
            if (anchorControl is Button)
            {
                validationGroup = ((Button)anchorControl).ValidationGroup;
            }
            if (string.IsNullOrEmpty(validationGroup))
            {
                validationGroup = string.IsNullOrEmpty(wrapper.ValidationGroup) ? this.DefaultValidationGroup : wrapper.ValidationGroup;
            }
            return validationGroup;
        }

        #endregion Private Members

        #region Protected Members

        private List<string> _validAnchorIds = null;
        protected virtual List<string> ValidAnchorIds
        {
            get
            {
                if (this._validAnchorIds == null)
                {
                    this._validAnchorIds = new List<string>();
                    this._validAnchorIds.Add("btnSubmit");
                }
                return this._validAnchorIds;
            }
        }

        protected abstract string DefaultValidationGroup { get; }

        #endregion Protected Members
    }
}
