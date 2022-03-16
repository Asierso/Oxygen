using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using SkiaSharp;
using System.IO;

namespace Oxygen
{
    class Window : IComponent
    {
        //Default window modification contezt
        public string Title { get; set; }
        public Point Position { get => this.GetControl().Location; set => this.GetControl().Location = value; } //Points to Locate property
        public Size Dimension { get => this.GetControl().Size; set => this.GetControl().Size = value; } //Points to Size property
        public SKColor WindowContainerColor { get => SkiaRenderer.WindowBackground; set => SkiaRenderer.WindowBackground = value; }
        public bool CanMaximize = true;
        public bool CanMinimize = true;
        public Panel Container { get => windowContainer; }

        //Windows enum collection
        public enum WindowsStatusKind { Normal, Minimized, Maximized }
        protected WindowsStatusKind WindowsStatus { get; set; }

        //Window instance component
        private Panel window = new Panel();
        private Panel windowContainer = new Panel();
        private Panel windowTopBar = new Panel();
        private Panel windowCloseButton = new Panel();
        private Panel windowResizeButton = new Panel();
        private Panel windowMinimizeButton = new Panel();

        //Window GDI Control getter
        public Control GetControl() => window;

        //Events of window states
        public delegate void WindowEvent(Window window);
        public event WindowEvent OnShow, OnReload, OnClose, OnMaximized, OnDrag;

        //Other
        private Point WindowPositionBeforeResize { get; set; }
        private Size WindowDimensionBeforeResize { get; set; }

        //Window subclass declaration
        private MouseEventsCollection MouseEvents = new MouseEventsCollection();
        private SkiaRendererCollection SkiaRenderer = new SkiaRendererCollection();

        //Components list
        private List<IComponent> Components = new List<IComponent>();

        //Window constructor
        public Window()
        {
            //Default coords
            Position = new Point(0, 0);
            Dimension = new Size(0, 0);

            //Default pre-configs
            Title = "Window";
            CanMaximize = true;
            CanMinimize = true;
            WindowsStatus = WindowsStatusKind.Normal;

            //Set window reference to delegate coll. class
            MouseEvents.WindowObject = this;
        }

        //Window instancier method (Used to create it and reload it)
        public void Show(bool isReload = false)
        {
            #region Window
            window.Location = Position;
            window.Size = Dimension;
            //window.BackColor = Color.Yellow;
            SkiaRenderer.WindowRender(window);
            #endregion

            #region Container
            windowContainer.Size = new Size(window.Width, window.Height - 30);
            windowContainer.Dock = DockStyle.Fill;

            if(!isReload) windowContainer.Click += ((sender, e) => GetControl().BringToFront());

            SkiaRenderer.WindowContainerRender(windowContainer);
            window.Controls.Add(windowContainer);
            #endregion

            #region TopBar
            windowTopBar.Size = new Size(Dimension.Width, 30);
            windowTopBar.Dock = DockStyle.Top;

            if (!isReload)
            {
                windowTopBar.MouseDown += MouseEvents.TopBarMouseDown;
                windowTopBar.MouseUp += MouseEvents.TopBarMouseUp;
                windowTopBar.MouseMove += MouseEvents.TopBarMouseMove;
            }

            SkiaRenderer.TopBarRender(windowTopBar, Title);

            window.Controls.Add(windowTopBar);
            #endregion

            #region MinimizeButton
            windowMinimizeButton.Dock = DockStyle.Right;
            windowMinimizeButton.Size = new Size(30, Dimension.Height);

            if (!isReload)
            {
                windowMinimizeButton.MouseClick += MouseEvents.ResizeButtonMouseClick;
                windowMinimizeButton.MouseMove += MouseEvents.ResizeButtonMouseMove;
            }

            SkiaRenderer.MinimizeButtonRender(windowMinimizeButton);

            if (CanMinimize) windowTopBar.Controls.Add(windowMinimizeButton);

            #endregion

            #region ResizeButton
            windowResizeButton.Dock = DockStyle.Right;
            windowResizeButton.Size = new Size(30, Dimension.Height);

            if (!isReload)
            {
                windowResizeButton.MouseClick += MouseEvents.ResizeButtonMouseClick;
                windowResizeButton.MouseMove += MouseEvents.ResizeButtonMouseMove;
            }

            SkiaRenderer.ResizeButtonRender(windowResizeButton);

            if(CanMaximize) windowTopBar.Controls.Add(windowResizeButton);

            #endregion

            #region CloseButton
            windowCloseButton.Dock = DockStyle.Right;
            windowCloseButton.Size = new Size(30, Dimension.Height);

            if (!isReload)
            {
                windowCloseButton.MouseClick += MouseEvents.CloseButtonMouseClick;
            }

            SkiaRenderer.CloseButtonRender(windowCloseButton);

            windowTopBar.Controls.Add(windowCloseButton);
            #endregion



            Desktop.AddWindow(this);

            if (isReload) OnShow?.Invoke(this);
            else OnReload?.Invoke(this);
        }

        //Window destroyer method (Used to destroy it and reload it)
        public void Close(bool isReload = false)
        {
            Desktop.RemoveWindow(this);
            OnClose?.Invoke(this);
        }

        //Window reload method (Used by control events)
        public void Reload()
        {
            Close(true);
            if (WindowsStatus == Window.WindowsStatusKind.Normal)
            {
                Position = WindowPositionBeforeResize;
                Dimension = WindowDimensionBeforeResize;
            }
            else
            {
                WindowPositionBeforeResize = Position;
                WindowDimensionBeforeResize = Dimension;
                Position = new Point(0, 0);
                Dimension = new Size(Desktop.GetControl().Width, Desktop.GetControl().Height);
                OnMaximized?.Invoke(this);
            }
            Show(true);
        }

        public void AddComponent<T>(IComponent component)
        {
            T componentType = (T)component;
            componentType.GetType().GetProperty("WindowContainerColor").SetValue(componentType, SkiaRenderer.WindowBackground);
            var panel = component.GetControl();
            component.Render();

            Components.Add(component);
            windowContainer.Controls.Add(panel);
        }

        public void RemoveComponent(IComponent component)
        {
            var panel = component.GetControl();
            Components.Remove(component);
            windowContainer.Controls.Remove(panel);
        }



        public void ReloadComponents() => Components.ForEach((compnent) => compnent.Render());

        public void Render() => Show();

        //Collection of events and actions of any parts of a window
        internal class MouseEventsCollection
        {
            //Internal vars for control interactions with program
            private bool MouseClick = false;
            private bool IsResizeEnabled = true;
            private Point PositionBeforeDrag { get; set; }
            public Window WindowObject { get; set; }

            //Drag windows methods
            public void TopBarMouseDown(object sender,MouseEventArgs e)
            {
                if (WindowObject.WindowsStatus != Window.WindowsStatusKind.Maximized)
                {
                    MouseClick = true;
                    PositionBeforeDrag = new Point(e.X, e.Y);
                }
                WindowObject.GetControl().BringToFront();
            }

            public void TopBarMouseUp(object sender,MouseEventArgs e)
            {
                MouseClick = false;
            }

            public void TopBarMouseMove(object sender, MouseEventArgs e)
            {
                if (MouseClick && WindowObject.WindowsStatus != Window.WindowsStatusKind.Maximized)
                {
                    WindowObject.GetControl().Location = new Point(WindowObject.Position.X + e.X - PositionBeforeDrag.X, WindowObject.Position.Y + e.Y - PositionBeforeDrag.Y);
                    WindowObject.OnDrag?.Invoke(WindowObject);
                }
            }

            public void CloseButtonMouseClick(object sender,MouseEventArgs e)
            {
                WindowObject.Close();
            }

            public void ResizeButtonMouseClick(object sender, MouseEventArgs e)
            {
                if (IsResizeEnabled)
                {
                    if (WindowObject.WindowsStatus == WindowsStatusKind.Normal) WindowObject.WindowsStatus = WindowsStatusKind.Maximized;
                    else WindowObject.WindowsStatus = WindowsStatusKind.Normal;
                    WindowObject.Reload();
                    IsResizeEnabled = false;
                }
            }

            public void ResizeButtonMouseMove(object sender,MouseEventArgs e)
            {
                IsResizeEnabled = true;
            }
        }

        //Collection of skia instructions to render all the window panel parts
        internal class SkiaRendererCollection
        {
            private SKColor TopBarBackground = SKColor.FromHsv(0, 0, 20, 255);
            public SKColor WindowBackground = SKColor.FromHsv(0, 0, 80, 255);
            
            public void CloseButtonRender(Panel buttonPanel)
            {
                var surface = SkiaGlobal.GetSurface(buttonPanel);
                var canvas = surface.Canvas;
                int center = buttonPanel.Width / 2;
                using (var buttonPanelBackground = new SKPaint())
                {
                    buttonPanelBackground.Color = TopBarBackground;
                    //canvas.DrawRect(new SKRect(0, 0, buttonPanel.Width, buttonPanel.Height), buttonPanelBackground);
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0, 0, buttonPanel.Width, buttonPanel.Height), 12,12), buttonPanelBackground);
                    canvas.DrawRect(new SKRect(0, 0, center, buttonPanel.Height), buttonPanelBackground);
                    canvas.DrawRect(new SKRect(0, buttonPanel.Height / 2, buttonPanel.Width, buttonPanel.Height), buttonPanelBackground);
                }
                using (var grayCirclePaint = new SKPaint())
                {
                    grayCirclePaint.Color = SKColor.FromHsv(0,75,100,255);
                    grayCirclePaint.StrokeWidth = 0;
                    grayCirclePaint.Style = SKPaintStyle.Fill;
                    canvas.DrawCircle(new SKPoint(center,center), 8, grayCirclePaint);
                }
                using(var whiteCrossPaint = new SKPaint())
                {
                    whiteCrossPaint.Color = SKColors.White;
                    whiteCrossPaint.StrokeWidth = 1.5f;
                    whiteCrossPaint.Style = SKPaintStyle.StrokeAndFill;

                    var path = new SKPath();
                    
                    path.AddPoly(new SKPoint[] {
                        new SKPoint(center - 3,center - 3),
                        new SKPoint(center - 2,center - 3),
                        new SKPoint(center + 2,center + 3),
                        new SKPoint(center + 3,center + 3),
                    },true);
                    path.AddPoly(new SKPoint[] {
                        new SKPoint(center + 3,center - 3),
                        new SKPoint(center + 2,center - 3),
                        new SKPoint(center - 2,center + 3),
                        new SKPoint(center - 3,center + 3),
                    }, true);

                    canvas.DrawPath(path, whiteCrossPaint);
                }
                canvas.Save();
                buttonPanel.BackgroundImage = SkiaGlobal.GetBitmap(surface);
            }

            public void ResizeButtonRender(Panel buttonPanel)
            {
                var surface = SkiaGlobal.GetSurface(buttonPanel);
                var canvas = surface.Canvas;
                int center = buttonPanel.Width / 2;
                using (var buttonPanelBackground = new SKPaint())
                {
                    buttonPanelBackground.Color = TopBarBackground;
                    canvas.DrawRect(new SKRect(0, 0, buttonPanel.Width, buttonPanel.Height), buttonPanelBackground);
                }
                using (var redCirclePaint = new SKPaint())
                {
                    redCirclePaint.Color = SKColor.FromHsv(0, 0, 50, 255);
                    redCirclePaint.StrokeWidth = 0;
                    redCirclePaint.Style = SKPaintStyle.Fill;
                    canvas.DrawCircle(new SKPoint(center, center), 8, redCirclePaint);
                }
                using (var whiteRectanglePaint = new SKPaint())
                {
                    whiteRectanglePaint.Color = SKColors.White;
                    whiteRectanglePaint.StrokeWidth = 1.5f;
                    whiteRectanglePaint.Style = SKPaintStyle.Stroke;

                    var path = new SKPath();
                    path.AddRect(new SKRect(center - 3, center - 3, center + 3, center + 3), SKPathDirection.Clockwise);

                    canvas.DrawPath(path, whiteRectanglePaint);
                }
                canvas.Save();
                buttonPanel.BackgroundImage = SkiaGlobal.GetBitmap(surface);
            }

            public void MinimizeButtonRender(Panel buttonPanel)
            {
                var surface = SkiaGlobal.GetSurface(buttonPanel);
                var canvas = surface.Canvas;
                int center = buttonPanel.Width / 2;
                using (var buttonPanelBackground = new SKPaint())
                {
                    buttonPanelBackground.Color = TopBarBackground;
                    canvas.DrawRect(new SKRect(0, 0, buttonPanel.Width, buttonPanel.Height), buttonPanelBackground);
                }
                using (var redCirclePaint = new SKPaint())
                {
                    redCirclePaint.Color = SKColor.FromHsv(0, 0, 50, 255);
                    redCirclePaint.StrokeWidth = 0;
                    redCirclePaint.Style = SKPaintStyle.Fill;
                    canvas.DrawCircle(new SKPoint(center, center), 8, redCirclePaint);
                }
                using (var whiteLinePaint = new SKPaint())
                {
                    whiteLinePaint.Color = SKColors.White;
                    whiteLinePaint.StrokeWidth = 1.75f;
                    whiteLinePaint.Style = SKPaintStyle.Stroke;

                    var path = new SKPath();
                    path.AddRect(new SKRect(center-3, center + 0.75f, center + 3, center - 0.25f), SKPathDirection.Clockwise);

                    canvas.DrawPath(path, whiteLinePaint);
                }
                canvas.Save();
                buttonPanel.BackgroundImage = SkiaGlobal.GetBitmap(surface);
            }

            public void TopBarRender(Panel topBarPanel,string title)
            {
                var surface = SkiaGlobal.GetSurface(topBarPanel);
                var canvas = surface.Canvas;
                int center = topBarPanel.Height / 2;
                using (var topBarBackground = new SKPaint())
                {
                    topBarBackground.Color = TopBarBackground;
                    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0, 0, topBarPanel.Width, topBarPanel.Height), 12), topBarBackground);
                    canvas.DrawRect(new SKRect(0, center, 12, center * 2), topBarBackground);
                    //canvas.DrawRect(new SKRect(0, 0, topBarPanel.Width, topBarPanel.Height),topBarBackground);
                }
                using (var titleFontPaint = new SKPaint())
                {
                    titleFontPaint.Color = SKColors.White;
                    titleFontPaint.Style = SKPaintStyle.Fill;
                    titleFontPaint.TextSize = 13;
                    titleFontPaint.Typeface = SKTypeface.FromFamilyName("Arial");

                    canvas.DrawText(title, new SKPoint(6, center + 5), titleFontPaint);
                }
                canvas.Save();
                topBarPanel.BackgroundImage = SkiaGlobal.GetBitmap(surface);
            }

            public void WindowRender(Panel windowPanel)
            {
                var surface = SkiaGlobal.GetSurface(windowPanel);
                var canvas = surface.Canvas;
                using (var windowBackground = new SKPaint())
                {
                    windowBackground.Color = SKColors.Transparent;
                    canvas.DrawRect(new SKRect(0, 0, windowPanel.Width, windowPanel.Height), windowBackground);
                }
                canvas.Save();
                windowPanel.BackgroundImage = SkiaGlobal.GetBitmap(surface);
            }

            public void WindowContainerRender(Panel windowPanel)
            {
                var surface = SkiaGlobal.GetSurface(windowPanel);
                var canvas = surface.Canvas;
                using (var windowBackground = new SKPaint())
                {
                    windowBackground.Color = WindowBackground;
                    canvas.DrawRect(new SKRect(0, 0, windowPanel.Width, windowPanel.Height), windowBackground);
                }
                canvas.Save();
                windowPanel.BackgroundImage = SkiaGlobal.GetBitmap(surface);
            }
        }
    }
}
