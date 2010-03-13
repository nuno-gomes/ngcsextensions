using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    public class SimpleCaptchaControl : PlaceHolder
    {
        private static ADSSAntiBot _captchaGenerator = new ADSSAntiBot();

        private ICipherProvider _cipherProvider;
        private string _cipherCode;
        private global::System.Web.UI.WebControls.Image _imgCaptcha;

        public SimpleCaptchaControl(ICipherProvider cipherProvider)
        {
            _cipherProvider = cipherProvider;
        }

        public string ValidationGroup { get; set; }
        public string ImageUrl { get; set; }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _imgCaptcha =
                new global::System.Web.UI.WebControls.Image
                {
                    AlternateText = NunoGomes.CommunityServer.Resources.CaptchaImage_AlternateText
                };

            TextBox txtCaptcha = new TextBox { ID = "txtCaptcha", EnableViewState = false };

            CustomValidator captchaPostValidator =
                new CustomValidator
                {
                    ID = "CaptchaCustVal",
                    ValidationGroup = this.ValidationGroup,
                    ControlToValidate = txtCaptcha.ID,
                    ErrorMessage = "*",
                    ValidateEmptyText = true,
                    EnableClientScript = true,
                    ClientValidationFunction = "CaptchaValidate"
                };
            captchaPostValidator.ServerValidate += new ServerValidateEventHandler(captchaPostValidator_ServerValidate);

            this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "CaptchaValidate", "function CaptchaValidate( src, args ){{args.IsValid = args.Value.length > 0;}}", true);

            this.Controls.Add(_imgCaptcha);
            this.Controls.Add(new LiteralControl("<br />"));
            this.Controls.Add(
                new Label
                {
                    Text = NunoGomes.CommunityServer.Resources.CaptchaLabel_Text
                });
            this.Controls.Add(txtCaptcha);
            this.Controls.Add(captchaPostValidator);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Page.RegisterRequiresControlState(this);
        }

        protected override void LoadControlState(object savedState)
        {
            if (savedState is string)
            {
                _cipherCode = (string)savedState;
            }
        }

        protected override object SaveControlState()
        {
            return _cipherCode;
        }

        protected override void OnPreRender(EventArgs e)
        {
            _cipherCode = _cipherProvider.Encrypt(_captchaGenerator.GenerateText(5));

            _imgCaptcha.ImageUrl = 
                string.Format("{0}?id={1}", 
                    this.ImageUrl,
                    HttpContext.Current.Server.UrlEncode(_cipherCode));

            base.OnPreRender(e);
        }

        #region Event Handling

        void captchaPostValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (_cipherCode == null)
            {
                args.IsValid = false;
                return;
            }
            string txtCaptcha = args.Value;

            if (_cipherCode == _cipherProvider.Encrypt(txtCaptcha))
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
