using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartRate
{
    class Program
    {
        const float fh_x = 0.5f;
        const float fh_y = 0.18f;
        const float fh_w = 0.25f;
        const float fh_h = 0.15f;

        static void Main(string[] args)
        {
            var capture = new Capture();

            //TODO: Switch to alt face detector haarcascade_frontalface_alt.xml
            var haar = new CascadeClassifier(@"C:\Emgu\emgucv-windows-universal 2.4.10.1940\opencv\data\haarcascades\haarcascade_frontalface_default.xml");

            var imageObservable = Observable.Interval(TimeSpan.FromSeconds(0.1f)).Select(x => capture.QueryFrame());

            var foreheadsObservable = imageObservable.Select(x => 
                                                     {
                                                         var gray = x.Convert<Gray, byte>();
                                                         var faces = haar.DetectMultiScale(gray, 1.1, 3, Size.Empty, Size.Empty).Select(y => 
                                                         {
                                                             var rect = new Rectangle();
                                                             var r_x = rect.X;
                                                             var r_y = rect.Y;
                                                             var r_w = (float)rect.Width;
                                                             var r_h = (float)rect.Height;

                                                             r_x += (int) (r_w * fh_x);
                                                             r_y += (int) (r_h * fh_y);
                                                             r_w *= fh_w;
                                                             r_h *= fh_h;
                                                             r_x -= (int)(r_w / 2.0f);
                                                             r_y -= (int)(r_h / 2.0f);
                                                             return new Rectangle(r_x, r_y, (int)r_w, (int)r_h);
                                                         });
                                                         return Tuple.Create(x, faces);
                                                     });

            var detectedForeheads = foreheadsObservable.Select(x =>
            {
                var foreheadRegions = x.Item2.Select(forehead =>
                {
                    return x.Item1.GetSubRect(forehead);
                });
                return foreheadRegions;
            });


            Console.ReadLine();
        }
    }
}