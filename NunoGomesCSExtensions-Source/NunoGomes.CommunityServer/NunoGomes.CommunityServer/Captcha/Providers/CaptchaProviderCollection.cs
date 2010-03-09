using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    public sealed class CaptchaProviderCollection : ProviderCollection
    {

        public new CaptchaProvider this[string name]
        {
            get { return (CaptchaProvider)base[name]; }
        }

        public override void Add(System.Configuration.Provider.ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is CaptchaProvider))
            {
                throw new ArgumentException("Invalid provider type", "provider");
            }

            base.Add(provider);
        }
    
    }
}
