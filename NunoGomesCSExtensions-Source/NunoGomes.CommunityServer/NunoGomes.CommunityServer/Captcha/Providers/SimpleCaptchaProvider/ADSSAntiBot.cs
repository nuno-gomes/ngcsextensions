using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NunoGomes.CommunityServer.Captcha.Providers
{
    /// <summary>
    /// This is a modified version of Dmitiriy Salko ADSSAntiBot class.
    /// The original post and code can be found at http://www.c-sharpcorner.com/UploadFile/dsalko/adssantibot10282007030736AM/adssantibot.aspx
    /// </summary>
    public class ADSSAntiBot : IDisposable
    {
        protected Bitmap _result;
        private Graphics _graphics;
        private Random _rnd;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public ADSSAntiBot()
            : this(135, 35)
        {
        }

        public ADSSAntiBot(int width, int height)
        {
            Width = width;
            Height = height;
            _rnd = new Random();
            _result = new Bitmap(width, height);
            _graphics = Graphics.FromImage(_result);
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
            if (this._result != null)
            {
                this._result.Dispose();
                this._result = null;
            }

            if (this._graphics != null)
            {
                this._graphics.Dispose();
                this._graphics = null;
            }
        }


        public PointF Noise(PointF p, double eps)
        {
            p.X = Convert.ToSingle(_rnd.NextDouble() * eps * 2 - eps) + p.X;
            p.Y = Convert.ToSingle(_rnd.NextDouble() * eps * 2 - eps) + p.Y;
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

            double amp = _rnd.NextDouble() * (double)(Height / 3);
            double size = _rnd.NextDouble() * (double)(Width / 4) + Width / 8;

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
                                m.RotateAt(Convert.ToSingle(_rnd.NextDouble() * 30 - 15), sp.PathPoints[0]);

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
                int n = _rnd.Next() % 10;
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
                    MeasureText(text, _graphics, fontFamily, out textSize, out fontSize);
                    int width = Convert.ToInt32(textSize.Width);
                    int height = Convert.ToInt32(textSize.Height);

                    int x = Convert.ToInt32(Math.Abs((double)width - (double)Width) * _rnd.NextDouble());
                    int y = Convert.ToInt32(Math.Abs((double)height - (double)Height) * _rnd.NextDouble());

                    using (GraphicsPath path = new GraphicsPath(FillMode.Alternate))
                    {
                        int fontStyle = (int)(FontStyle.Regular);
                        float emSize = fontSize;
                        Point origin = new Point(x, y);
                        StringFormat format = StringFormat.GenericDefault;

                        path.AddString(text, fontFamily, fontStyle, emSize, origin, format);

                        _graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        Rectangle rect = new Rectangle(0, 0, Width, Height);
                        _graphics.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.White, Color.LightGray, 0f), rect);
                        //g.DrawString(aText, f, new SolidBrush(Color.Black), x, y);
                        _graphics.SmoothingMode = SmoothingMode.AntiAlias;

                        using (GraphicsPath radomizedPath = RandomWarp(path))
                        {
                            _graphics.FillPath(new SolidBrush(Color.Black), radomizedPath);
                        }
                    }
                }
            }
        }

        public Bitmap Result
        {
            get
            {
                return _result;
            }
        }
    }
}