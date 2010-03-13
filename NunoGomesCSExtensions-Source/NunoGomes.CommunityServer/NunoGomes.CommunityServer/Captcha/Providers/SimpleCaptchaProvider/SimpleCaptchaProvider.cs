using System;
using System.Web.UI.WebControls;
using System.Configuration.Provider;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    public interface ICipherProvider
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public partial class SimpleCaptchaProvider : CaptchaProvider, ICipherProvider
    {
        private string _passPhrase = null;        // can be any string
        private string _saltValue = null;        // can be any string
        private string _hashAlgorithm = null;             // can be "MD5"
        private int _passwordIterations = 3;                  // can be any number
        private string _initVector = "@PT9Net5Pt0ComH8"; // must be 16 bytes
        private int _keySize = 256;                // can be 192 or 128

        private RijndaelSymmetricAlgorithm _manager = null;

        private string _imageUrl = null;
        private bool _enabled = true;


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
             * Initialize enabled
             */
            string enabledStr = config["enabled"];
            if (!String.IsNullOrEmpty(enabledStr))
            {
                bool.TryParse(enabledStr, out _enabled);
            }
            config.Remove("enabled");

            /*
             * ----------------------
             * Initialize passPhrase
             */
            _passPhrase = config["passPhrase"];
            if (String.IsNullOrEmpty(_passPhrase))
            {
                throw new ProviderException("Empty or missing passPhrase");
            }
            config.Remove("passPhrase");

            /*
             * ----------------------
             * Initialize saltValue
             */
            _saltValue = config["saltValue"];
            if (String.IsNullOrEmpty(_saltValue))
            {
                throw new ProviderException("Empty or missing saltValue");
            }
            config.Remove("saltValue");

            /*
             * ----------------------
             * Initialize hashAlgorithm
             */
            _hashAlgorithm = config["hashAlgorithm"];
            if (String.IsNullOrEmpty(_hashAlgorithm))
            {
                throw new ProviderException("Empty or missing hashAlgorithm");
            }
            config.Remove("hashAlgorithm");

            _manager = new RijndaelSymmetricAlgorithm(
                _passPhrase,
                _saltValue,
                _hashAlgorithm,
                _passwordIterations,
                _initVector,
                _keySize);

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
            if(!_enabled)
            {
                return;
            }
            SimpleCaptchaControl captchaControl = new SimpleCaptchaControl(this);
            captchaControl.ValidationGroup = validationGroup;
            captchaControl.ImageUrl = _imageUrl;

            captchaPanel.Controls.Add(captchaControl);
        }

        #region Implementation of ICipherProvider

        string ICipherProvider.Encrypt(string plainText)
        {
            return _manager.Encrypt(plainText);
        }

        string ICipherProvider.Decrypt(string cipherText)
        {
            return _manager.Decrypt(cipherText);
        }

        #endregion
    }
}
