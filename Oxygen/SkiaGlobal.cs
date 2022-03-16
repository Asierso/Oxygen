using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oxygen
{
    class SkiaGlobal
    {
        public static SKSurface GetSurface(Panel panel) => SKSurface.Create(new SKImageInfo(panel.Width, panel.Height));
        public static Bitmap GetBitmap(SKSurface surface)
        {
            var image = surface.Snapshot();
            var imageEncodedData = image.Encode(SKEncodedImageFormat.Png, 50);
            var stream = new MemoryStream(imageEncodedData.ToArray());
            return new Bitmap(stream);
        }
    }
}
