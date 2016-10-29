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

namespace MetronomoUWP
{
    public sealed partial class MainPage : Page
    {
        private bool isTimerRunning = false;
        private int seconds = 0; //Se eliminará cuando funcione correctamente
        private CancellationTokenSource tokenSource;
        public int bpm { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.bpm = 60; //Cambiar en el futuro a un BPM usado previamente
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            seconds = 0;
            tokenSource = new CancellationTokenSource();
            if (isTimerRunning == true)
            {
                btnStart.Content = "Start";
                txtBpm.IsEnabled = true;
                btnLess.IsEnabled = true;
                btnPlus.IsEnabled = true;
                stopBackgroundTimer();
            }
            else
            {                
                startBackgroundTimer();
                txtBpm.IsEnabled = false;
                btnLess.IsEnabled = false;
                btnPlus.IsEnabled = false;
                btnStart.Content = "Stop";
            }
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

        private async void startBackgroundTimer()
        {
            ///El código debe ejecutarse por un periodo indeterminado de tiempo.
            ///Basado en https://social.msdn.microsoft.com/Forums/windowsapps/en-US/40f4a4a9-1235-4071-b62b-e4077df3e1dc/uwpdispatch-timer-and-windows-10?forum=wpdevelop
            isTimerRunning = true;
            Int32 ts = (60/bpm) * 1000;
            TimeSpan myTimeSpan = new TimeSpan(0, 0, 0, 0, ts);

            while (this.isTimerRunning)
            {
                /// Do something
                /// Temporal
                //txbText.Text = seconds.ToString();
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

        private void btnChangeBpm_Click(object sender, RoutedEventArgs e)
        {
            //Cambiar en el futuro para que sea asíncrono
            //Se debe poder cambiar mientras está activado el metrónomo

            if (((Button)sender).Name == btnLess.Name && this.bpm > 1)
                this.bpm--;
            else if(((Button)sender).Name == btnPlus.Name && this.bpm <= 200)
                this.bpm++;

            txtBpm.Text = this.bpm.ToString();
        }

        private void txtBpm_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Arreglar
            //if (isNumeric(txtBpm.Text) || txtBpm.Text == "")
            //{
            //    if (txtBpm.Text != "")
            //        this.bpm = int.Parse(txtBpm.Text);
            //}
            //else
            //    txtBpm.Text = this.bpm.ToString();
        }

        private bool isNumeric(string s)
        {
            int output;
            return int.TryParse(s, out output);
        }

        ///Acerca de timers
        ///t1: https://social.msdn.microsoft.com/Forums/windowsapps/en-US/40f4a4a9-1235-4071-b62b-e4077df3e1dc/uwpdispatch-timer-and-windows-10?forum=wpdevelop
        ///t2: http://stackoverflow.com/questions/34271100/timer-in-uwp-app-which-isnt-linked-to-the-ui
        ///t1vst2: http://social.technet.microsoft.com/wiki/contents/articles/21177.visual-c-thread-sleep-vs-task-delay.aspx
    }
}