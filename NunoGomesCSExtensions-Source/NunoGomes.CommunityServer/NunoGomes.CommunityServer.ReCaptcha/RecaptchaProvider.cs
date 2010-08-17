using System;
using System.Configuration.Provider;
using NunoGomes.CommunityServer.Captcha.Providers;
using Recaptcha;

namespace NunoGomes.CommunityServer.Recaptcha
{
    public class RecaptchaProvider : CaptchaProvider
    {
        private string _privateKey;
        private string _publicKey;
        private string _theme;
        private string _errorMessage = "*";
        private string _customThemeWidget;
        private bool _allowMultipleInstances;
        private bool _overrideSecureMode;
        private bool _skipRecaptcha;

        [PreEmptive.Attributes.Feature("RecaptchaProvider")]
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign "name" a default value if it currently has no value or is an empty string
            if (String.IsNullOrEmpty(name))
                name = "RecaptchaProvider";

            // Add a default "description" attribute to config if the attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "CommunityServer Recaptcha provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            /*
             * ----------------------
             * Initialize privateKey
             */
            _privateKey = config["privateKey"];
            if (String.IsNullOrEmpty(_privateKey))
            {
                throw new ProviderException("Empty or missing privateKey");
            }
            config.Remove("privateKey");

            /*
             * ----------------------
             * Initialize publicKey
             */
            _publicKey = config["publicKey"];
            if (String.IsNullOrEmpty(_publicKey))
            {
                throw new ProviderException("Empty or missing publicKey");
            }
            config.Remove("publicKey");

            /*
             * ----------------------
             * Initialize theme
             */
            string theme = config["theme"];
            if (!String.IsNullOrEmpty(theme))
            {
                _theme = theme;
            }
            config.Remove("theme");

            /*
             * ----------------------
             * Initialize errorMessage
             */
            string errorMessage = config["errorMessage"];
            if (!String.IsNullOrEmpty(errorMessage))
            {
                _errorMessage = errorMessage;
            }
            config.Remove("errorMessage");

            /*
             * ----------------------
             * Initialize customThemeWidget
             */
            string customThemeWidget = config["customThemeWidget"];
            if (!String.IsNullOrEmpty(customThemeWidget))
            {
                _customThemeWidget = customThemeWidget;
            }
            config.Remove("customThemeWidget");

            /*
             * ----------------------
             * Initialize skipRecaptcha
             */
            string skipRecaptchaStr = config["skipRecaptcha"];
            if (!String.IsNullOrEmpty(skipRecaptchaStr))
            {
                if (!bool.TryParse(skipRecaptchaStr, out _skipRecaptcha))
                {
                    throw new ProviderException("Invalid skipRecaptcha value. Valid values are [true|false]");
                }
            }
            config.Remove("skipRecaptcha");

            /*
             * ----------------------
             * Initialize overrideSecureMode
             */
            string overrideSecureModeStr = config["overrideSecureMode"];
            if (!String.IsNullOrEmpty(overrideSecureModeStr))
            {
                if (!bool.TryParse(overrideSecureModeStr, out _overrideSecureMode))
                {
                    throw new ProviderException("Invalid overrideSecureMode value. Valid values are [true|false]");
                }
            }
            config.Remove("overrideSecureMode");

            /*
             * ----------------------
             * Initialize allowMultipleInstances
             */
            string allowMultipleInstancesStr = config["allowMultipleInstances"];
            if (!String.IsNullOrEmpty(allowMultipleInstancesStr))
            {
                if (!bool.TryParse(allowMultipleInstancesStr, out _allowMultipleInstances))
                {
                    throw new ProviderException("Invalid allowMultipleInstances value. Valid values are [true|false]");
                }
            }
            config.Remove("allowMultipleInstances");

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

        public override void AddCaptchaControls(System.Web.UI.WebControls.Panel captchaPanel, string validationGroup)
        {
            RecaptchaControl recaptchaControl = 
                new RecaptchaControl
                {
                    PrivateKey = this._privateKey,
                    PublicKey = this._publicKey,
                    Theme = this._theme,
                    CustomThemeWidget = this._customThemeWidget,
                    AllowMultipleInstances = this._allowMultipleInstances,
                    OverrideSecureMode = this._overrideSecureMode,
                    SkipRecaptcha = this._skipRecaptcha
                };
            if (RecaptchaManager.SupportsErrorMessage)
            {
                recaptchaControl.ErrorMessage = this._errorMessage;
            }

            captchaPanel.Controls.Add(recaptchaControl);
        }
    }
}
