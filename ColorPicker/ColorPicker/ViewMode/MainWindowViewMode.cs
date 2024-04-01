using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ColorPicker.ViewMode
{
    public class MainWindowViewMode : ObservableObject
    {
        private DispatcherTimer timer = new DispatcherTimer();

        private Color MyColor;

        private byte red = 0;

        public byte Red
        {
            get { return red; }
            set
            {
                red = value;
                OnPropertyChanged();
            }
        }

        private byte green = 0;

        public byte Green
        {
            get { return green; }
            set
            {
                green = value;
                OnPropertyChanged();
            }
        }

        private byte blue = 0;

        public byte Blue
        {
            get { return blue; }
            set
            {
                blue = value;
                OnPropertyChanged();
            }
        }

        private byte alpha = 250;

        public byte Alpha
        {
            get { return alpha; }
            set
            {
                alpha = value;
                OnPropertyChanged();
            }
        }


        public struct POINT
        {
            public int X;

            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }


        public MainWindowViewMode()
        {
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Tick += timer_Tick;

            MouseDownCommand = new RelayCommand<Button>(MouseDown);
            MouseUpCommand = new RelayCommand<Button>(MouseUp);
        }


        [DllImport("gdi32")]
        private static extern int GetPixel(int hdc, int nXPos, int nYPos);

        [DllImport("user32")]
        private static extern int GetWindowDC(int hwnd);

        [DllImport("user32")]
        private static extern int ReleaseDC(int hWnd, int hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);


        public RelayCommand<Button> MouseDownCommand { get; set; }
        public RelayCommand<Button> MouseUpCommand { get; set; }

        private void timer_Tick(object sender, EventArgs e)
        {
            GetCursorPos(out var pt);
            Point point = new Point(pt.X, pt.Y);
            MyColor = GetPixelColor(point);

            Red = MyColor.R;
            Green = MyColor.G;
            Blue = MyColor.B;
            Alpha = MyColor.A;
        }

        private Color GetPixelColor(Point point)
        {
            int windowDC = GetWindowDC(0);
            int pixel = GetPixel(windowDC, (int)point.X, (int)point.Y);
            ReleaseDC(0, windowDC);
            byte b = (byte)((ulong)(pixel >> 16) & 0xFFuL);
            byte g = (byte)((ulong)(pixel >> 8) & 0xFFuL);
            byte r = (byte)((ulong)pixel & 0xFFuL);
            return Color.FromRgb(r, g, b);
        }

        public void MouseDown(Button button)
        {
            button.CaptureMouse();
            button.Content = "松开完成";
            timer.Start();
        }
        public void MouseUp(Button button)
        {
            button.ReleaseMouseCapture();
            button.Content = "按住取色";
            timer.Stop();
        }
    }
}
