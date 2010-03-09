using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NunoGomes.CommunityServer.Captcha.Configuration
{
    public class CaptchaSection : ConfigurationSection
    {

        #region Private Constants

        private const string DEFAULTPROVIDER_PROPERTY_NAME = "defaultProvider";
        private const ConfigurationPropertyOptions DEFAULTPROVIDER_PROPERTY_OPTIONS = ConfigurationPropertyOptions.IsRequired;
        private const string PROVIDERS_PROPERTY_NAME = "providers";
        private const ConfigurationPropertyOptions PROVIDERS_PROPERTY_OPTIONS = ConfigurationPropertyOptions.IsDefaultCollection;

        #endregion Private Constants

        #region Private Class Fields

        private static readonly ConfigurationProperty m_defaultProviderProperty =
                new ConfigurationProperty(
                    CaptchaSection.DEFAULTPROVIDER_PROPERTY_NAME,
                    typeof(string),
                    "default",
                    null,
                    new StringValidator(1),
                    CaptchaSection.DEFAULTPROVIDER_PROPERTY_OPTIONS);

        private static readonly ConfigurationProperty m_providersProperty =
                new ConfigurationProperty(
                    CaptchaSection.PROVIDERS_PROPERTY_NAME,
                    typeof(ProviderSettingsCollection),
                    null,
                    null,
                    null,
                    CaptchaSection.PROVIDERS_PROPERTY_OPTIONS);

        #endregion Private Class Fields

        #region Class Lifetime

        /// <summary>
        /// Initializes the <see cref="CaptchaSection"/> class.
        /// </summary>
        protected internal CaptchaSection()
        {
            this.Properties.Add(CaptchaSection.m_defaultProviderProperty);
            this.Properties.Add(CaptchaSection.m_providersProperty);
        }
        #endregion Class Lifetime

        #region Specific Public Properties

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base[CaptchaSection.PROVIDERS_PROPERTY_NAME]; }
        }

        /// <summary>
        /// Gets or sets the default provider.
        /// </summary>
        /// <value>The default provider.</value>
        public string DefaultProvider
        {
            get { return (string)base[CaptchaSection.DEFAULTPROVIDER_PROPERTY_NAME]; }
            set { base[CaptchaSection.DEFAULTPROVIDER_PROPERTY_NAME] = value; }
        }

        #endregion Specific Public Properties


    }
}
