using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    public class ADSSAntiBot : IDisposable
    {
        protected Bitmap result;
        Graphics g;
        Random rnd;

        public int Width {get; private set;}
        public int Height { get; private set; }

        public ADSSAntiBot():this(135,35)
        {
        }

        public ADSSAntiBot(int width, int height)
        {
            Width = width;
            Height = height;
            rnd = new Random();
            result = new Bitmap(width, height);
            g = Graphics.FromImage(result);
        }

        ~ADSSAntiBot()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (this.result != null)
        {
                this.result.Dispose();
                this.result = null;
        }

            if (this.g != null)
        {
                this.g.Dispose();
                this.g = null;
            }
        }


        public PointF Noise(PointF p, double eps)
        {
            p.X = Convert.ToSingle(rnd.NextDouble() * eps * 2 - eps) + p.X;
            p.Y = Convert.ToSingle(rnd.NextDouble() * eps * 2 - eps) + p.Y;
            return p;
        }

        public PointF Wave(PointF p, double amp, double size)
        {
            p.Y = Convert.ToSingle(Math.Sin(p.X / size) * amp) + p.Y;
            p.X = Convert.ToSingle(Math.Sin(p.X / size) * amp) + p.X;
            return p;
        }



        public GraphicsPath RandomWarp(GraphicsPath path)
        {
            // Add line //
            int PsCount = 10;
            PointF[] curvePs = new PointF[PsCount * 2];
            for (int u = 0; u < PsCount; u++)
            {
                curvePs[u].X = u * (Width / PsCount);
                curvePs[u].Y = Height / 2;
            }
            for (int u = PsCount; u < (PsCount * 2); u++)
            {
                curvePs[u].X = (u - PsCount) * (Width / PsCount);
                curvePs[u].Y = Height / 2 + 2;
            }

            path.AddLines(curvePs);

            //
            double eps = Height * 0.05;

            double amp = rnd.NextDouble() * (double)(Height / 3);
            double size = rnd.NextDouble() * (double)(Width / 4) + Width / 8;

            double offset = (double)(Height / 3);


            PointF[] pn = new PointF[path.PointCount];
            byte[] pt = new byte[path.PointCount];

            using (GraphicsPath np2 = new GraphicsPath())
            {
                using (GraphicsPathIterator iter = new GraphicsPathIterator(path))
                {
            for (int i = 0; i < iter.SubpathCount; i++)
            {
                        using (GraphicsPath sp = new GraphicsPath())
                        {
                bool closed;
                iter.NextSubpath(sp, out closed);

                            using (Matrix m = new Matrix())
                            {
                m.RotateAt(Convert.ToSingle(rnd.NextDouble() * 30 - 15), sp.PathPoints[0]);

                m.Translate(-1 * i, 0);

                sp.Transform(m);
                            }

                np2.AddPath(sp, true);
            }
                    }
                }

            for (int i = 0; i < np2.PointCount; i++)
            {
                //pn[i] = Noise( path.PathPoints[i] , eps);
                pn[i] = Wave(np2.PathPoints[i], amp, size);
                pt[i] = np2.PathTypes[i];
            }

            }

            return new GraphicsPath(pn, pt);
        }


        public string GenerateText(int len)
        {
            string str = "";
            for (int i = 0; i < len; i++)
            {
                int n = rnd.Next() % 10;
                str += n.ToString();
            }

            return str;
        }

        public void DrawNumbers(int len)
        {
            string str = GenerateText(len);
            DrawText(str);
        }

        private void MeasureText(string text, Graphics g, FontFamily fontFamily, out SizeF textSize, out float fontSize)
        {
            fontSize = Height;

            while (fontSize > 0)
            {
                using (Font font = new Font(fontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
                {
                    textSize = g.MeasureString(text, font);

                    if ((textSize.Width < Width) && (textSize.Height < Height))
                    {
                        return;
                    }
                    else
                    {
                        fontSize *= 0.9F;
                    }
                }
            }

            textSize = new SizeF();
        }

        public void DrawText(string text)
        {

            //using (Graphics g = Graphics.FromImage(result))
            {
                using (FontFamily fontFamily = new FontFamily("Verdana"))
            {
                    SizeF textSize;
                    float fontSize;
                    MeasureText(text, g, fontFamily, out textSize, out fontSize);
                    int width = Convert.ToInt32(textSize.Width);
                    int height = Convert.ToInt32(textSize.Height);

            int x = Convert.ToInt32(Math.Abs((double)width - (double)Width) * rnd.NextDouble());
            int y = Convert.ToInt32(Math.Abs((double)height - (double)Height) * rnd.NextDouble());

                    using (GraphicsPath path = new GraphicsPath(FillMode.Alternate))
                    {
            int fontStyle = (int)(FontStyle.Regular);
                        float emSize = fontSize;
            Point origin = new Point(x, y);
            StringFormat format = StringFormat.GenericDefault;

                        path.AddString(text, fontFamily, fontStyle, emSize, origin, format);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            g.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.White, Color.LightGray, 0f), rect);
            //g.DrawString(aText, f, new SolidBrush(Color.Black), x, y);
            g.SmoothingMode = SmoothingMode.AntiAlias;

                        using (GraphicsPath radomizedPath = RandomWarp(path))
                        {
                            g.FillPath(new SolidBrush(Color.Black), radomizedPath);
                        }
                    }
                }
            }
        }

        public Bitmap Result
        {
            get
            {
                return result;
            }
        }
    }
}