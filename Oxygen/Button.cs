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
    class Button : IComponent
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

        //For transparency
        public SKColor WindowContainerColor { get; set; }

        //Events
        public event EventHandler Click, MouseEnter, MouseLeave;
        public event MouseEventHandler MouseMove,MouseUp,MouseDown,MouseDoubleClick;

        //Button subclases declaration
        private SkiaRendererCollection SkiaRenderer = new SkiaRendererCollection();

        public Button()
        {
            Position = new Point(0, 0);
            Dimension = new Size(75, 25);
            Text = "Button";
            Font = new Font("Arial", 11, FontStyle.Regular);
            ForeColor = SKColor.FromHsl(0, 0, 20);

            ComponentBase.MouseEnter += ((sender, e) => SkiaRenderer.ButtonHoverRender(ComponentBase, Text, Font, ForeColor, WindowContainerColor));
            ComponentBase.MouseLeave += ((sender, e) => SkiaRenderer.ButtonNormalRender(ComponentBase, Text, Font, ForeColor, WindowContainerColor));

            ComponentBase.Click += ((sender, e) => Click?.Invoke(sender, e));
            ComponentBase.MouseEnter += ((sender, e) => MouseEnter?.Invoke(sender, e));
            ComponentBase.MouseLeave += ((sender, e) => MouseLeave?.Invoke(sender, e));
            ComponentBase.MouseMove += ((sender, e) => MouseMove?.Invoke(sender, e));
            ComponentBase.MouseUp += ((sender, e) => MouseUp?.Invoke(sender, e));
            ComponentBase.MouseDown += ((sender, e) => MouseDown?.Invoke(sender, e));
            ComponentBase.MouseDoubleClick += ((sender, e) => MouseDoubleClick?.Invoke(sender, e));

        }

        public void Render() => SkiaRenderer.ButtonNormalRender(ComponentBase, Text, Font, ForeColor, WindowContainerColor);

        internal class SkiaRendererCollection
        {
            public void ButtonNormalRender(Panel componentBase, string text, Font font, SKColor foreColor, SKColor windowContainerColor)
            {
                var surface = SkiaGlobal.GetSurface(componentBase);
                var canvas = surface.Canvas;
                var center = new SKPoint(componentBase.Width / 2, componentBase.Height / 2);
                float rounded = 50f;
                float margin = 2.5f;

                using (var buttonPanelBackground = new SKPaint())
                {
                    var drawRectUp = new SKRect(0, center.Y, componentBase.Width, componentBase.Height);
                    buttonPanelBackground.Color = windowContainerColor;
                    canvas.DrawRect(new SKRect(0, 0, componentBase.Width, componentBase.Height), buttonPanelBackground);
                }

                using (var roundedButton = new SKPaint())
                {
                    roundedButton.Color = SKColor.FromHsl(0, 0, 90);
                    roundedButton.Style = SKPaintStyle.Fill;
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0 + margin, 0 + margin, componentBase.Width - margin, componentBase.Height - margin), rounded), roundedButton);
                }

                using (var buttonText = new SKPaint())
                {
                    buttonText.Color = foreColor;
                    buttonText.Style = SKPaintStyle.Fill;
                    buttonText.TextSize = font.Size;
                    buttonText.TextAlign = SKTextAlign.Center;
                    buttonText.Typeface = SKTypeface.FromFamilyName(font.Name);

                    canvas.DrawText(text, new SKPoint(center.X, center.Y + (buttonText.TextSize / 2)-1), buttonText);
                }

                canvas.Save();
                componentBase.BackgroundImage = SkiaGlobal.GetBitmap(surface);
            }
            public void ButtonHoverRender(Panel componentBase, string text, Font font, SKColor foreColor, SKColor windowContainerColor)
            {
                var surface = SkiaGlobal.GetSurface(componentBase);
                var canvas = surface.Canvas;
                var center = new SKPoint(componentBase.Width / 2, componentBase.Height / 2);
                float rounded = 50f;
                float margin = 2.5f;

                using (var buttonPanelBackground = new SKPaint())
                {
                    var drawRectUp = new SKRect(0, center.Y, componentBase.Width, componentBase.Height);
                    buttonPanelBackground.Color = windowContainerColor;
                    canvas.DrawRect(new SKRect(0, 0, componentBase.Width, componentBase.Height), buttonPanelBackground);
                }

                using (var roundedButton = new SKPaint())
                {
                    roundedButton.Color = SKColor.FromHsl(0, 0, 90);
                    roundedButton.Style = SKPaintStyle.Fill;
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0 + margin, 0 + margin, componentBase.Width - margin, componentBase.Height - margin), rounded), roundedButton);
                }

                using (var roundedButtonHover = new SKPaint())
                {
                    roundedButtonHover.Color = SKColor.FromHsl(213, 84, 67);
                    roundedButtonHover.Style = SKPaintStyle.Stroke;
                    roundedButtonHover.StrokeWidth = 3;
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0 + margin, 0 + margin, componentBase.Width - margin, componentBase.Height - margin), rounded), roundedButtonHover);
                }

                using (var buttonText = new SKPaint())
                {
                    buttonText.Color = foreColor;
                    buttonText.Style = SKPaintStyle.Fill;
                    buttonText.TextSize = font.Size;
                    buttonText.TextAlign = SKTextAlign.Center;
                    buttonText.Typeface = SKTypeface.FromFamilyName(font.Name);

                    canvas.DrawText(text, new SKPoint(center.X, center.Y + (buttonText.TextSize / 2)-1), buttonText);
                }

                canvas.Save();
                componentBase.BackgroundImage = SkiaGlobal.GetBitmap(surface);
            }
        }

    }
}
