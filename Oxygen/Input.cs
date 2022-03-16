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
    class Input : IComponent
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

        //Button subclases declaration
        private MouseEventsCollection MouseEvents = new MouseEventsCollection();
        private SkiaRendererCollection SkiaRenderer = new SkiaRendererCollection();

        public Input()
        {
            Position = new Point(0, 0);
            Dimension = new Size(100, 25);
            Text = "Text";
            Font = new Font("Arial", 11, FontStyle.Regular);
            ForeColor = SKColor.FromHsl(0, 0, 20);
            TextAlign = SKTextAlign.Left;

            ComponentBase.MouseEnter += MouseEvents.InputMouseEnter;
            ComponentBase.MouseLeave += MouseEvents.InputMouseLeave;
            ComponentBase.Click += MouseEvents.InputMouseClick;
            MouseEvents.InputObject = this;
        }

        public void Render() => SkiaRenderer.InputRenderNormal(ComponentBase, Text, Font, ForeColor, WindowContainerColor);

        //Collection of events and actions of any parts of a window
        internal class MouseEventsCollection
        {
            //Internal vars for control interactions with program
            public Input InputObject { get; set; }
            private bool selected { get; set; }
            private TextBox inputDetect = new TextBox()
            {
                Location = new Point(0, 0),
                Size = new Size(0, 0)
            };

            public void InputMouseEnter(object sender,EventArgs e)
            {
                InputObject.GetControl().Cursor = Cursors.IBeam;
            }

            public void InputMouseLeave(object sender, EventArgs e)
            {
                InputObject.GetControl().Cursor = Cursors.Default;
                if(selected)
                {
                    selected = false;
                    InputObject.SkiaRenderer.InputRenderNormal(InputObject.ComponentBase, InputObject.Text, InputObject.Font, InputObject.ForeColor, InputObject.WindowContainerColor);
                    InputObject.GetControl().Controls.Clear();
                }
            }

            public void InputMouseClick(object sender, EventArgs e)
            {
                if (!selected)
                {
                    selected = true;
                    InputObject.SkiaRenderer.InputRenderSelected(InputObject.ComponentBase, InputObject.Text, InputObject.Font, InputObject.ForeColor, InputObject.WindowContainerColor);

                    
                    inputDetect.Text = InputObject.Text;
                    InputObject.GetControl().Controls.Add(inputDetect);
                    inputDetect.Focus();
                    inputDetect.TextChanged += ((sender2, e2) => {
                        InputObject.Text = inputDetect.Text;
                        InputObject.Render();
                    });

                }
            }
        }

        //Collection of skia instructions to render all the window panel parts
        internal class SkiaRendererCollection
        {
            public void InputRenderNormal(Panel componentBase, string text, Font font, SKColor foreColor, SKColor windowContainerColor)
            {
                var surface = SkiaGlobal.GetSurface(componentBase);
                var canvas = surface.Canvas;
                var center = new SKPoint(componentBase.Width / 2, componentBase.Height / 2);
                float rounded = 50f;
                float margin = 2.5f;

                using (var labelPanelBackground = new SKPaint())
                {
                    var drawRectUp = new SKRect(0, center.Y, componentBase.Width, componentBase.Height);
                    labelPanelBackground.Color = windowContainerColor;
                    canvas.DrawRect(new SKRect(0, 0, componentBase.Width, componentBase.Height), labelPanelBackground);
                }

                using(var labelRounded = new SKPaint())
                {
                    labelRounded.Color = SKColor.FromHsl(0, 0, 70);
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0 + margin, 0 + margin, componentBase.Width - margin, componentBase.Height - margin), rounded), labelRounded);
                }

                using (var labelText = new SKPaint())
                {
                    labelText.Color = foreColor;
                    labelText.Style = SKPaintStyle.Fill;
                    labelText.TextSize = font.Size;
                    labelText.TextAlign = SKTextAlign.Left;
                    labelText.Typeface = SKTypeface.FromFamilyName(font.Name);
                    canvas.DrawText(text, new SKPoint(margin * 3, center.Y + (labelText.TextSize / 2)), labelText);
                    canvas.Save();
                    componentBase.BackgroundImage = SkiaGlobal.GetBitmap(surface);
                }
            }
            public void InputRenderSelected(Panel componentBase, string text, Font font, SKColor foreColor, SKColor windowContainerColor)
            {
                var surface = SkiaGlobal.GetSurface(componentBase);
                var canvas = surface.Canvas;
                var center = new SKPoint(componentBase.Width / 2, componentBase.Height / 2);
                float rounded = 50f;
                float margin = 2.5f;

                using (var inputPanelBackground = new SKPaint())
                {
                    var drawRectUp = new SKRect(0, center.Y, componentBase.Width, componentBase.Height);
                    inputPanelBackground.Color = windowContainerColor;
                    canvas.DrawRect(new SKRect(0, 0, componentBase.Width, componentBase.Height), inputPanelBackground);
                }

                using (var inputRounded = new SKPaint())
                {
                    inputRounded.Color = SKColor.FromHsl(0, 0, 70);
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0 + margin, 0 + margin, componentBase.Width - margin, componentBase.Height - margin), rounded), inputRounded);
                }

                using (var roundedButtonHover = new SKPaint())
                {
                    roundedButtonHover.Color = SKColor.FromHsl(213, 84, 67);
                    roundedButtonHover.Style = SKPaintStyle.Stroke;
                    roundedButtonHover.StrokeWidth = 3;
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0 + margin, 0 + margin, componentBase.Width - margin, componentBase.Height - margin), rounded), roundedButtonHover);
                }

                using (var inputText = new SKPaint())
                {
                    inputText.Color = foreColor;
                    inputText.Style = SKPaintStyle.Fill;
                    inputText.TextSize = font.Size;
                    inputText.TextAlign = SKTextAlign.Left;
                    inputText.Typeface = SKTypeface.FromFamilyName(font.Name);
                    canvas.DrawText(text, new SKPoint(margin * 3, center.Y + (inputText.TextSize / 2)), inputText);
                    canvas.Save();
                    componentBase.BackgroundImage = SkiaGlobal.GetBitmap(surface);
                }
            }
        }  
    }
}
