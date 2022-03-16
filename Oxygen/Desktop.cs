using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Oxygen
{
    class Desktop
    {
        private static Form desktop = new Form();

        private static List<Window> windowsHeap = new List<Window>();

        public delegate void DesktopEvent();
        public static event DesktopEvent OnStart;

        //Give acess to desktop 
        public static Control GetControl() => desktop;
        public static void Initialize()
        {
            desktop.WindowState = FormWindowState.Maximized;
            desktop.TopMost = true;
            desktop.FormBorderStyle = FormBorderStyle.None;
            desktop.Show();
            OnStart?.Invoke();
        }

        //Windows heap controls
        public static void AddWindow(Window window)
        {
            windowsHeap.Add(window);
            desktop.Controls.Add(window.GetControl());
        }
        public static Window GetWindow(int index)
        {
            return windowsHeap[index];
        }
        public static int GetWindowIndex(Window window)
        {
            return windowsHeap.IndexOf(window);
        }
        public static void RemoveWindow(Window window)
        {
            windowsHeap.Remove(window);
            desktop.Controls.Remove(window.GetControl());
        }
        public static bool ContainsWindow(Window window)
        {
            return windowsHeap.Contains(window);
        } 
    }
}
