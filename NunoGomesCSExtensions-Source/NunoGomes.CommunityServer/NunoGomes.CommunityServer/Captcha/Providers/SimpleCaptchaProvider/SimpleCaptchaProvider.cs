using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using NunoGomes.CommunityServer.Util;
using System.Configuration.Provider;
using System.Web;
using System.Web.UI;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    public class SimpleCaptchaProvider : CaptchaProvider
    {
        private string _imageUrl = null;
        private string _cookieName = "{26F3DFAB-2E24-4363-907C-6D5880F9AFC5}";
        ADSSAntiBot _captcha = new ADSSAntiBot();

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign "name" a default value if it currently has no value or is an empty string
            if (String.IsNullOrEmpty(name))
                name = "DefaultCaptchaProvider";

            // Add a default "description" attribute to config if the attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Default Captcha provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            /*
             * ----------------------
             * Initialize imageUrl
             */
            _imageUrl = config["imageUrl"];
            if (String.IsNullOrEmpty(_imageUrl))
            {
                throw new ProviderException("Empty or missing imageUrl");
            }
            config.Remove("imageUrl");

            /*
             * ----------------------
             * Initialize cookieName
             */
            string cookieName = config["cookieName"];
            if (!String.IsNullOrEmpty(cookieName))
            {
                _cookieName = cookieName;
            }
            config.Remove("cookieName");

            /*
             * ----------------------
             * Throw an exception if unrecognized attributes remain
             */
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " + attr);
            }
        }

        public override void AddCaptchaControls(Panel captchaPanel, string validationGroup)
        {
            string code = _captcha.GenerateText(5);
            string id = CipherManager.Encrypt(code);

            global::System.Web.UI.WebControls.Image imgCaptcha =
                new global::System.Web.UI.WebControls.Image
                {
                    ImageUrl = string.Format("{0}?id={1}", _imageUrl, HttpContext.Current.Server.UrlEncode(id)),
                    AlternateText = NunoGomes.CommunityServer.Resources.CaptchaImage_AlternateText
                };

            HttpContext.Current.Response.Cookies[_cookieName].Value = CipherManager.Encrypt(code);
            HttpContext.Current.Response.Cookies[_cookieName].HttpOnly = true;

            TextBox txtCaptcha = new TextBox { ID = "txtCaptcha", EnableViewState = false };

            CustomValidator captchaPostValidator =
                new CustomValidator
                {
                    ID = "CaptchaCustVal",
                    ValidationGroup = validationGroup,
                    ControlToValidate = txtCaptcha.ID,
                    ErrorMessage = "*",
                    ValidateEmptyText = true,
                    EnableClientScript = true,
                    ClientValidationFunction = "CaptchaValidate"
                };
            captchaPostValidator.ServerValidate += new ServerValidateEventHandler(captchaPostValidator_ServerValidate);

            captchaPanel.Page.ClientScript.RegisterClientScriptBlock(captchaPanel.Page.GetType(), "CaptchaValidate", "function CaptchaValidate( src, args ){{args.IsValid = args.Value.length > 0;}}", true);

            captchaPanel.Controls.Add(imgCaptcha);
            captchaPanel.Controls.Add(new LiteralControl("<br />"));
            captchaPanel.Controls.Add(
                new Label
                {
                    Text = NunoGomes.CommunityServer.Resources.CaptchaLabel_Text
                });
            captchaPanel.Controls.Add(txtCaptcha);
            captchaPanel.Controls.Add(captchaPostValidator);
        }

        #region Event Handling

        void captchaPostValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[_cookieName];
            if (cookie == null)
            {
                args.IsValid = false;
                return;
            }
            string txtCaptcha = args.Value;

            if (cookie.Value == CipherManager.Encrypt(txtCaptcha))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        #endregion Event Handling

    }
}
