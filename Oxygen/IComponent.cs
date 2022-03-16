using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oxygen
{
    interface IComponent
    {
        Control GetControl();
        void Render();
        SKColor WindowContainerColor { get; set; }
    }
}
