using System.Web;
using System.Drawing;
using NunoGomes.CommunityServer.Captcha.Configuration;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    public class SimpleCaptchaProviderImageHandler : IHttpHandler
    {
        private ADSSAntiBot _captchaGenerator = new ADSSAntiBot();

        ~SimpleCaptchaProviderImageHandler()
        {
            if (this._captchaGenerator != null)
            {
                this._captchaGenerator.Dispose();
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (CaptchaConfiguration.DefaultProvider is ICipherProvider)
            {
                context.Response.Clear();
                context.Response.ClearHeaders();
                context.Response.ClearContent();
                context.Response.ContentType = "image/jpeg";

                string id = context.Request["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    string code = ((ICipherProvider)CaptchaConfiguration.DefaultProvider).Decrypt(id);
                    if (!string.IsNullOrEmpty(code))
                    {
                        this._captchaGenerator.DrawText(code);

                        Bitmap bmp = this._captchaGenerator.Result;
                        bmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }

                context.Response.Flush();
                context.Response.Close();
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
