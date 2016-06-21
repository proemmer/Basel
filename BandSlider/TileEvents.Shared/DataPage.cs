using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BandSlider
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class DataPage
    {
        private App _viewModel;
        private int _numOfEvents = 0;

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _numOfEvents++;
            _viewModel.Producer.OnAccelerometerSensorUpdate += Producer_OnAccelerometerSensorUpdate;

            if(_viewModel.Player != null)
                await _viewModel.Player.StartAsync();
            else if(_viewModel.Recorder != null)
                await _viewModel.Recorder.StartAsync();

        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Producer.OnAccelerometerSensorUpdate -= Producer_OnAccelerometerSensorUpdate;

            if (_viewModel.Playing)
                await _viewModel.Player.StopAsync();
            else
                await _viewModel.Recorder.StopAsync();
        }


        private async void Producer_OnAccelerometerSensorUpdate(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateUI(e.SensorReading));
        }

        private void UpdateUI(IBandAccelerometerReading accelerometerReading)
        {
            _numOfEvents++;
            statusTextBlock.Text = "getting data";
            // Show the numeric values.
            xTextBlock.Text = _numOfEvents + " - X: " + accelerometerReading.AccelerationX.ToString("0.00");
            yTextBlock.Text = _numOfEvents + " - Y: " + accelerometerReading.AccelerationY.ToString("0.00");
            zTextBlock.Text = _numOfEvents + " - Z: " + accelerometerReading.AccelerationZ.ToString("0.00");

            // Show the values graphically.
            xLine.X2 = xLine.X1 + accelerometerReading.AccelerationX * 200;
            yLine.Y2 = yLine.Y1 - accelerometerReading.AccelerationY * 200;
            zLine.X2 = zLine.X1 - accelerometerReading.AccelerationZ * 100;
            zLine.Y2 = zLine.Y1 + accelerometerReading.AccelerationZ * 100;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }
    }
}
