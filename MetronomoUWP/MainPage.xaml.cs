using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace MetronomoUWP
{
    public sealed partial class MainPage : Page
    {
        private bool isTimerRunning = false;
        private int seconds = 0;
        private CancellationTokenSource tokenSource;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            seconds = 0;
            tokenSource = new CancellationTokenSource();
            if(isTimerRunning == true)
                stopBackgroundTimer();
            else
                startBackgroundTimer();
        }

        private void stopBackgroundTimer()
        {
            try
            {
                tokenSource.Cancel();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("No se pudo cancelar\n" + ex.ToString());
            }
        }

        private async void startBackgroundTimer(int bpm = 60)
        {
            ///El código debe ejecutarse por un periodo indeterminado de tiempo.
            ///Basado en https://social.msdn.microsoft.com/Forums/windowsapps/en-US/40f4a4a9-1235-4071-b62b-e4077df3e1dc/uwpdispatch-timer-and-windows-10?forum=wpdevelop
            isTimerRunning = true;
            int ts = (60/bpm) * 1000;
            TimeSpan myTimeSpan = new TimeSpan(0, 0, 0, 0, ts);

            while (this.isTimerRunning)
            {
                /// Do something
                /// Temporal
                txbText.Text = seconds.ToString();
                System.Diagnostics.Debug.WriteLine(DateTime.Now);
                /// End

                try
                {
                    await Task.Delay(myTimeSpan, tokenSource.Token);
                    seconds += 1;
                }
                catch(TaskCanceledException tex)
                {
                    System.Diagnostics.Debug.WriteLine("Token... " + tex);
                    break;
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception... " + ex);
                    break;
                }
            }
            this.isTimerRunning = false;
        }

        ///Acerca de timers
        ///t1: https://social.msdn.microsoft.com/Forums/windowsapps/en-US/40f4a4a9-1235-4071-b62b-e4077df3e1dc/uwpdispatch-timer-and-windows-10?forum=wpdevelop
        ///t2: http://stackoverflow.com/questions/34271100/timer-in-uwp-app-which-isnt-linked-to-the-ui
        ///t1vst2: http://social.technet.microsoft.com/wiki/contents/articles/21177.visual-c-thread-sleep-vs-task-delay.aspx
    }
}
