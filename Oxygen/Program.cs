using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oxygen
{
    static class Program
    {
        static void Main()
        {
            var win = new Window();
            win.Dimension = new System.Drawing.Size(320, 280);
            win.Position = new System.Drawing.Point(10, 12);
            win.Title = "Program";
            win.CanMaximize = true;
            win.Show();

            Desktop.OnStart += new Desktop.DesktopEvent(() =>
            {
                

                
            });

            var label = new Label();
            label.Text = "This is a test label";
            win.AddComponent<Label>(label);

            var btn1 = new Button();
            btn1.Position = new System.Drawing.Point(0, 25);
            btn1.Text = "Add window";
            btn1.GetControl().Click += new EventHandler((sender, e) =>
            {
                var win2 = new Window();
                win2.Title = "Test";
                win2.Dimension = new System.Drawing.Size(320, 280);
                win2.Position = new System.Drawing.Point(10, 12);
                win2.CanMinimize = false;   
                win2.Show();
            });

            var btn2 = new Button();
            btn2.Position = new System.Drawing.Point(0, 50);
            btn2.Text = "Show msg";
            btn2.GetControl().Click += new EventHandler((sender, e) =>
            {
                var notif = new Notify();
                notif.Text = "Text msgbox component";
                notif.Position = new System.Drawing.Point(Desktop.GetControl().Width / 2, Desktop.GetControl().Height / 2);
                notif.Show();
            });

            var inp1 = new Input();
            inp1.Position = new System.Drawing.Point(0, 75);

            win.AddComponent<Button>(btn1);
            win.AddComponent<Button>(btn2);
            win.AddComponent<Input>(inp1);

            Application.EnableVisualStyles();
            Desktop.Initialize();
            Application.Run((Form)Desktop.GetControl());
        }
    }
}
