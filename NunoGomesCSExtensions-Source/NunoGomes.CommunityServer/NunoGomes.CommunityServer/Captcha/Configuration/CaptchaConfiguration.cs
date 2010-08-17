using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NunoGomes.CommunityServer.Captcha.Providers;
using System.Web.Configuration;
using System.Configuration.Provider;

namespace NunoGomes.CommunityServer.Captcha.Configuration
{
    public static partial class CaptchaConfiguration
    {
        #region Fields

        private static CaptchaProvider _defaultProvider;
        private static CaptchaProviderCollection _providers;
        private static object _syncProvider = new RuntimeIntelligence();

        #endregion Fields


        #region Constructors

        static CaptchaConfiguration()
        {
            if (_defaultProvider == null)
            {
                lock (_syncProvider)
                {
                    if (_defaultProvider == null)
                    {
                        try
                        {
                            CaptchaSection settings = (CaptchaSection)ConfigurationManager.GetSection("communityServer.Captcha");

                            if (settings != null && settings.ElementInformation.IsPresent)
                            {
                                // Load registered providers and point _provider to the default provider
                                _providers = new CaptchaProviderCollection();
                                ProvidersHelper.InstantiateProviders(settings.Providers, _providers, typeof(CaptchaProvider));

                                _defaultProvider = _providers[settings.DefaultProvider];

                                if (_defaultProvider == null)
                                    throw new ProviderException("Unable to load default CaptchaProvider");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
            }
        }

        #endregion Constructors


        #region Properties

        public static CaptchaProvider DefaultProvider
        {
            get { return _defaultProvider; }
        }

        public static CaptchaProviderCollection Providers
        {
            get { return _providers; }
        }

        #endregion Properties
    }
}
