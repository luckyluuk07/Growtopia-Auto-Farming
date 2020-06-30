using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace Auto_Farming_Growtopia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        InputSimulator inputSimulator = new InputSimulator();
        private VirtualKeyCode movement = VirtualKeyCode.VK_D;
        private CancellationTokenSource _canceller;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool punchs = true;
            btnStop.IsEnabled = true;
            btnStart.IsEnabled = false;
            _canceller = new CancellationTokenSource();

            if (left.IsChecked == true)
            {
                movement = VirtualKeyCode.VK_A;
            } else if (right.IsChecked == true)
            {
                movement = VirtualKeyCode.VK_D;
            }
            if (punch.IsChecked == false)
            {
                punchs = false;
            } else if (punch.IsChecked == true)
            {
                punchs = true;
            }

            BringMainWindowToFront("Growtopia");
            farm(punchs);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            stop();
        }

        public void stop()
        {
            BringMainWindowToFront("Growtopia");
            _canceller.Cancel();
            inputSimulator.Keyboard.KeyUp(movement);
            inputSimulator.Keyboard.KeyUp(VirtualKeyCode.SPACE);
            btnStop.IsEnabled = false;
            btnStart.IsEnabled = true;
            Thread.Sleep(500);
            BringMainWindowToFront("Auto Farming Growtopia");
        }

        public async void farm(bool punchs)
        {
            await Task.Run(() =>
            {
                do
                {
                    if (punchs == true)
                    {
                        inputSimulator.Keyboard.KeyDown(movement);
                        inputSimulator.Keyboard.KeyDown(VirtualKeyCode.SPACE);
                        Thread.Sleep(250);
                        inputSimulator.Keyboard.KeyUp(movement);
                        inputSimulator.Keyboard.KeyUp(VirtualKeyCode.SPACE);
                    } else if (punchs == false)
                    {
                        inputSimulator.Keyboard.KeyDown(movement);
                        Thread.Sleep(250);
                        inputSimulator.Keyboard.KeyUp(movement);
                    }

                    if (_canceller.Token.IsCancellationRequested)
                    {
                        _canceller.Dispose();
                        break;
                    }

                } while (true);
            });
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        public void BringMainWindowToFront(string processName)
        {
            // get the process
            Process bProcess = Process.GetProcessesByName(processName).FirstOrDefault();

            // check if the process is running
            if (bProcess != null)
            {
                // check if the window is hidden / minimized
                if (bProcess.MainWindowHandle == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(bProcess.Handle, ShowWindowEnum.Restore);
                }

                // set user the focus to the window
                SetForegroundWindow(bProcess.MainWindowHandle);
            }
            else
            {
                // the process is not running, so start it
                Process.Start(processName);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ((bool)Properties.Settings.Default["FirstRun"] == true)
            {
                Properties.Settings.Default["FirstRun"] = false;
                Properties.Settings.Default.Save();
                MessageBox.Show("You must allow Spacebar for punch in Growtopia first", "Growtopia Auto Farming Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            } else
            {
                return;
            }
        }
    }
}
