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
using MetronomoUWP.Model;

namespace MetronomoUWP
{
    public sealed partial class MainPage : Page
    {
        private bool isTimerRunning = false;
        private int beat = 1; //Se eliminará cuando funcione correctamente
        private CancellationTokenSource tokenSource;
        public int bpm { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.bpm = 60; //Cambiar en el futuro a un BPM usado previamente
            
            mediaElementSoundAccent.Source = new Uri("ms-appx:///Assets/tic.mp3", UriKind.RelativeOrAbsolute);
            mediaElementSoundNormal.Source = new Uri("ms-appx:///Assets/toc.mp3", UriKind.RelativeOrAbsolute);
        }

        private void page_loaded(object sender, RoutedEventArgs e)
        {
            cbCompas.SelectedIndex = 2;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            beat = 1;
            tokenSource = new CancellationTokenSource();
            if (isTimerRunning == true)
            {
                btnStart.Content = "Start";
                txtBpm.IsEnabled = true;
                btnLess.IsEnabled = true;
                btnPlus.IsEnabled = true;
                cbCompas.IsEnabled = true;
                stopBackgroundTimer();
            }
            else
            {                
                startBackgroundTimer();
                txtBpm.IsEnabled = false;
                btnLess.IsEnabled = false;
                btnPlus.IsEnabled = false;
                cbCompas.IsEnabled = false;
                btnStart.Content = "Stop";
                beat = 1;
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
            TimeSpan myTimeSpan;
            int intValue = 0;
            double doubleValue = 0.0;

            if (bpm <= 60)
            {
                intValue = (60 / bpm) * 1000;
                myTimeSpan = new TimeSpan(0, 0, 0, 0, intValue);
            }
            else
            {
                doubleValue = 60.0 / bpm;
                myTimeSpan = new TimeSpan(0, 0, 0, 0, ((Int32)(doubleValue * 1000)));
                doubleValue = 0.0;
            }

            mediaElementSoundAccent.PlaybackRate = ((60 / bpm) * 1000) * 0.5;
            mediaElementSoundNormal.PlaybackRate = ((60 / bpm) * 1000) * 0.5;

            int compas = getCompas();

            while (this.isTimerRunning)
            {
                System.Diagnostics.Debug.WriteLine(DateTime.Now);
                await playSounds(beat, compas);
                try
                {
                    await Task.Delay(myTimeSpan, tokenSource.Token);
                    beat += 1;
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
            else if(((Button)sender).Name == btnPlus.Name && this.bpm < 240)
                this.bpm++;

            txtBpm.Text = this.bpm.ToString();
        }

        private void txtBpm_TextChanged(object sender, TextChangedEventArgs e)
        {
            /// Validar
            /// Actualmente si no se valida, la asignación no funcionará.

            /// Cambiar!
            try
            {
                bpm = int.Parse(((TextBox)sender).Text.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Error conversión BPM:\n" + 
                    ex.Message.ToString());
            }
        }        

        private async Task playSounds(int pulsoActual, int compas = 4, int accentNote = 1)
        {            
            // Corregir
            if (pulsoActual > compas)
            {
                beat = 1;
                pulsoActual = beat;
            }

            if (pulsoActual == accentNote)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    mediaElementSoundAccent.Play();
                });
            }
            else
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    mediaElementSoundNormal.Play();
                });
            }

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                beatStoryboard1.Begin();
            });
            //beatStoryboard1.Begin();

            System.Diagnostics.Debug.WriteLine(beat);
            txbText.Text = beat.ToString();
        }

        private int getCompas()
        {
            ComboBoxItem c = (ComboBoxItem)cbCompas.SelectedItem;
            string[] tempArray = c.Content.ToString().Split('|');
            return Int32.Parse(tempArray[0].ToString().Trim());
        }

        ///Acerca de timers
        ///t1: https://social.msdn.microsoft.com/Forums/windowsapps/en-US/40f4a4a9-1235-4071-b62b-e4077df3e1dc/uwpdispatch-timer-and-windows-10?forum=wpdevelop
        ///t2: http://stackoverflow.com/questions/34271100/timer-in-uwp-app-which-isnt-linked-to-the-ui
        ///t1vst2: http://social.technet.microsoft.com/wiki/contents/articles/21177.visual-c-thread-sleep-vs-task-delay.aspx
    }
}