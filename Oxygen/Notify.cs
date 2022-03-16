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
    class Notify : IComponent
    {
        //Layout base
        private Panel ComponentBase = new Panel();
        public Control GetControl() => ComponentBase;

        //Properties
        public Point Position { get => this.GetControl().Location; set => this.GetControl().Location = value; }
        public string Text { get => this.GetControl().Text; set => this.GetControl().Text = value; }
        public string Caption { get; set; }
        public Font Font { get => this.GetControl().Font; set => this.GetControl().Font = value; }

        //For transparency
        public SKColor WindowContainerColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Show() => Render();

        public Notify()
        {
            Position = new Point(0, 0);
            Text = "Text";
            Caption = "Notify";
            Font = new Font("Arial", 11, FontStyle.Regular);
        }

        public void Render()
        {
            var window = new Window();
            window.Position = Position;
            window.Dimension = new Size(200, 45);
            window.Title = Caption;
            window.CanMaximize = false;
            window.CanMinimize = false;
            window.Show();

            string someText = "";
            int chars = 0,lines = 0;

            var generateTextMethod = new Action(() =>
            {
                var label = new Label();
                label.Position = new Point(0,5 + lines);
                label.TextAlign = SKTextAlign.Center;
                label.Font = new Font("Arial", 12, FontStyle.Regular);
                label.Dimension = new Size(window.Dimension.Width, 18);
                label.Text = someText;
                window.Dimension = new Size(window.Dimension.Width, window.Dimension.Height + 18);
                window.AddComponent<Label>(label);
                lines = lines + 18;
            });

            for (int i = 0; i < Text.Length; i++)
            {
                someText += Text[i];
                chars++;
                if (chars >= 20 && Text[i] == ' ')
                {
                    generateTextMethod.Invoke();
                    someText = "";
                    chars = 0;
                }
                else if(chars == 27)
                {
                    someText += "-";
                    generateTextMethod.Invoke();
                    someText = "";
                    chars = 0;
                }
            }

            generateTextMethod.Invoke();

        }
    }
}
