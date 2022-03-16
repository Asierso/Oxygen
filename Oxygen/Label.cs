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
    class Label : IComponent
    {
        //Layout base
        private Panel ComponentBase = new Panel();
        public Control GetControl() => ComponentBase;

        //Properties
        public Point Position { get => this.GetControl().Location; set => this.GetControl().Location = value; }
        public Size Dimension { get => this.GetControl().Size; set => this.GetControl().Size = value; }
        public string Text { get => this.GetControl().Text; set => this.GetControl().Text = value; }
        public Font Font { get => this.GetControl().Font; set => this.GetControl().Font = value; }
        public SKColor ForeColor { get; set; }
        public SKTextAlign TextAlign { get; set; }

        //For transparency
        public SKColor WindowContainerColor { get; set; }

        //Events
        public event EventHandler Click, MouseEnter, MouseLeave;
        public event MouseEventHandler MouseMove, MouseUp, MouseDown, MouseDoubleClick;

        //Button subclases declaration
        private SkiaRendererCollection SkiaRenderer = new SkiaRendererCollection();

        public Label()
        {
            Position = new Point(0, 0);
            Dimension = new Size(100, 15);
            Text = "Label";
            Font = new Font("Arial", 11, FontStyle.Regular);
            ForeColor = SKColor.FromHsl(0, 0, 20);
            TextAlign = SKTextAlign.Left;

            ComponentBase.Click += ((sender, e) => Click?.Invoke(sender, e));
            ComponentBase.MouseEnter += ((sender, e) => MouseEnter?.Invoke(sender, e));
            ComponentBase.MouseLeave += ((sender, e) => MouseLeave?.Invoke(sender, e));
            ComponentBase.MouseMove += ((sender, e) => MouseMove?.Invoke(sender, e));
            ComponentBase.MouseUp += ((sender, e) => MouseUp?.Invoke(sender, e));
            ComponentBase.MouseDown += ((sender, e) => MouseDown?.Invoke(sender, e));
            ComponentBase.MouseDoubleClick += ((sender, e) => MouseDoubleClick?.Invoke(sender, e));

        }

        public void Render() => SkiaRenderer.LabelRender(ComponentBase, Text, Font, TextAlign, ForeColor, WindowContainerColor);

        internal class SkiaRendererCollection
        {
            public void LabelRender(Panel componentBase, string text, Font font, SKTextAlign textAlign, SKColor foreColor, SKColor windowContainerColor)
            {
                var surface = SkiaGlobal.GetSurface(componentBase);
                var canvas = surface.Canvas;
                var center = new SKPoint(componentBase.Width / 2, componentBase.Height / 2);

                using (var labelPanelBackground = new SKPaint())
                {
                    var drawRectUp = new SKRect(0, center.Y, componentBase.Width, componentBase.Height);
                    labelPanelBackground.Color = windowContainerColor;
                    canvas.DrawRect(new SKRect(0, 0, componentBase.Width, componentBase.Height), labelPanelBackground);
                }

                using (var labelText = new SKPaint())
                {
                    labelText.Color = foreColor;
                    labelText.Style = SKPaintStyle.Fill;
                    labelText.TextSize = font.Size;
                    labelText.TextAlign = textAlign;
                    labelText.Typeface = SKTypeface.FromFamilyName(font.Name);
                    switch (textAlign)
                    {
                        case SKTextAlign.Left:
                            canvas.DrawText(text, new SKPoint(0, center.Y + (labelText.TextSize / 2)), labelText);
                            break;
                        case SKTextAlign.Center:
                            canvas.DrawText(text, new SKPoint(center.X, center.Y + (labelText.TextSize / 2)), labelText);
                            break;
                        case SKTextAlign.Right:
                            canvas.DrawText(text, new SKPoint(componentBase.Width, center.Y + (labelText.TextSize / 2)), labelText);
                            break;
                    }

                    canvas.Save();
                    componentBase.BackgroundImage = SkiaGlobal.GetBitmap(surface);
                }
            }
        }
    }
}
