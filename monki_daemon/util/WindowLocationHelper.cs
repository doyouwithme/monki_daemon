using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace MonkiDaemon.util
{
    public class WindowLocationHelper
    {
        public static double MainWindowLeft => WindowLocationHelper.MainWindowLocation.Left;

        public static double MainWindowTop => WindowLocationHelper.MainWindowLocation.Top;

        public static double MainWindowWidth => WindowLocationHelper.MainWindowLocation.Width;

        public static double MainWindowHeight => WindowLocationHelper.MainWindowLocation.Height;

        private static WindowLocationHelper.WindowLocation MainWindowLocation => new WindowLocationHelper.WindowLocation(Screen.FromHandle(new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle));

        public static double WindowLeft(double width, Window window = null)
        {
            if (window == null)
                return WindowLocationHelper.MainWindowLeft + WindowLocationHelper.MainWindowWidth / 2.0 - width / 2.0;
            WindowLocationHelper.WindowLocation windowLocation = new WindowLocationHelper.WindowLocation(Screen.FromHandle(new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle), window);
            return windowLocation.Left + windowLocation.Width / 2.0 - width / 2.0;
        }

        public static double WindowTop(double height, Window window = null)
        {
            if (window == null)
                return WindowLocationHelper.MainWindowTop + WindowLocationHelper.MainWindowHeight / 2.0 - height / 2.0;
            WindowLocationHelper.WindowLocation windowLocation = new WindowLocationHelper.WindowLocation(Screen.FromHandle(new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle), window);
            return windowLocation.Top + windowLocation.Height / 2.0 - height / 2.0;
        }

        public class WindowLocation
        {
            public double Left { get; private set; }

            public double Top { get; private set; }

            public double Width { get; private set; }

            public double Height { get; private set; }

            public WindowLocation(Screen screen, Window window = null)
            {
                if (window == null)
                    window = System.Windows.Application.Current.MainWindow;
                if (window.WindowState == WindowState.Minimized || window.WindowState == WindowState.Maximized)
                {
                    this.Left = (double)screen.WorkingArea.Left;
                    this.Top = (double)screen.WorkingArea.Top;
                    Rectangle workingArea = screen.WorkingArea;
                    this.Width = (double)workingArea.Width;
                    workingArea = screen.WorkingArea;
                    this.Height = (double)workingArea.Height;
                }
                else
                {
                    this.Left = window.PointToScreen(new System.Windows.Point(0.0, 0.0)).X;
                    this.Top = window.PointToScreen(new System.Windows.Point(0.0, 0.0)).Y;
                    this.Width = window.ActualWidth;
                    this.Height = window.ActualHeight;
                }
            }
        }
    }
}
