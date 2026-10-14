using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;
using System.Diagnostics;

namespace Taxi.API.Server.Services
{
    public class CaptchaService
    {
        private IMemoryCache _cache;
        public CaptchaService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenImageBase64(string capchaText)
        {
            int width = 150;
            int height = 50;

            using var bimap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bimap);

            canvas.Clear(SKColors.WhiteSmoke);

            var typeFace = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic);
            using var font = new SKFont(typeFace, 20);
            var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = SKColors.DarkSlateGray;
            
            var random = new Random();
            using var linePaint = new SKPaint
            {
                Color = SKColors.LightGray,
                StrokeWidth = 2
            };
            for (int i = 0; i < 5; i++)
            { 
                canvas.DrawLine(
                    random.Next(0, width), random.Next(0, height),
                    random.Next(0, width), random.Next(0, height),
                    linePaint
                    );
            }

            float x = 5;
            foreach(char ch in capchaText)
            {
                float y = 35 + random.Next(-5, 5);
                canvas.DrawText(ch.ToString(), x, y, SKTextAlign.Left, font, paint);
                x += 15;
            }

            using var image = SKImage.FromBitmap(bimap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var memoryStream = new MemoryStream();

            data.SaveTo(memoryStream); 
            byte[] imageBytes = memoryStream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        public string CaptchaGen(string userLogin)
        {
            string allowchar = "";
            allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            char a = ',';
            string[] ar = allowchar.Split(a);
            string pwd = "";
            string temp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                temp = ar[r.Next(0, ar.Length)];
                pwd += temp;
            }
            //var guid = Guid.NewGuid();
            Debug.WriteLine(pwd);
            return pwd;
        }

        public bool CapthcaCheck(Guid capthcaId, string captchaUser)
        {
            if (_cache.TryGetValue(capthcaId.ToString(), out string correctCaptcha))
            {
                if (string.Equals(captchaUser, correctCaptcha, StringComparison.Ordinal))
                {
                    _cache.Remove(capthcaId.ToString());
                    return true;
                }
                _cache.Remove(capthcaId.ToString());
            }
            return false;
        }
    }
}
