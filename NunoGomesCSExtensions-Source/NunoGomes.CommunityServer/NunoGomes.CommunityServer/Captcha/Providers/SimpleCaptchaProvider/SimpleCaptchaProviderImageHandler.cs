using System.Web;
using System.Drawing;
using NunoGomes.CommunityServer.Util;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    public class SimpleCaptchaProviderImageHandler : IHttpHandler
    {
        private ADSSAntiBot _captcha = new ADSSAntiBot();

        ~SimpleCaptchaProviderImageHandler()
        {
            if (this._captcha != null)
            {
                this._captcha.Dispose();
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.ClearContent();
            context.Response.ContentType = "image/jpeg";

            string id = context.Request["id"];
            if (!string.IsNullOrEmpty(id))
            {
                string code = CipherManager.Decrypt(id);
                if (!string.IsNullOrEmpty(code))
                {
                    this._captcha.DrawText(code);

                    Bitmap bmp = this._captcha.Result;
                    bmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }

            context.Response.Flush();
            context.Response.Close();
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
