/*
    Copyright (c) Microsoft Corporation All rights reserved.  
 
    MIT License: 
 
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
    documentation files (the  "Software"), to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
    and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
 
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
 
    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using BandSlider.Tile;
using Basel;
using Basel.Detection;
using Basel.Detection.Detectors;
using Basel.Detection.Recognizer.Dollar;
using Basel.Detection.Recognizer.UWave;
using Basel.Recorder;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using Recognizer.Dollar;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TileEvents;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace BandSlider
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    partial class MainPage
    {
        private App _viewModel;
        private bool _handlingClick;
        IBandClient _bandClient;

        private async void ListenToTileEventsButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(async () =>
            {
                if (_bandClient == null)
                {
                    // Get the list of Microsoft Bands paired to the phone.
                    IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                    if (pairedBands.Length < 1)
                    {
                        App.Current.StatusMessage = "This sample app requires a Microsoft Band paired to your device. Also make sure that you have the latest firmware installed on your Band, as provided by the latest Microsoft Health app.";
                        return;
                    }
                    // Connect to Microsoft Band.
                    _bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

                    var id = TileConstants.TileGuid;
                    var cts = new CancellationTokenSource();
                    if (!_bandClient.TileManager.TileInstalledAndOwned(ref id, cts.Token))
                    {
                        // Create a Tile with a TextButton and WrappedTextBlock on it.
                        BandTile myTile = new BandTile(TileConstants.TileGuid)
                        {
                            Name = "BandSlider",
                            TileIcon = await LoadIcon("ms-appx:///Assets/SampleTileIconLarge.png"),
                            SmallIcon = await LoadIcon("ms-appx:///Assets/SampleTileIconSmall.png")
                        };
                        var designe = new BandSliderTileLayout();
                        myTile.PageLayouts.Add(designe.Layout);

                        // Remove the Tile from the Band, if present. An application won't need to do this everytime it runs. 
                        // But in case you modify this sample code and run it again, let's make sure to start fresh.
                        await _bandClient.TileManager.RemoveTileAsync(TileConstants.TileGuid);
                        await designe.LoadIconsAsync(myTile);

                        // Create the Tile on the Band.
                        await _bandClient.TileManager.AddTileAsync(myTile);
                        await _bandClient.TileManager.SetPagesAsync(TileConstants.TileGuid, new PageData(TileConstants.Page1Guid, 0, designe.Data.All));
                        App.Current.StatusMessage = "Installed Tile";
                    }

                    _bandClient.TileManager.TileButtonPressed += TileManager_TileButtonPressed;
                    await _bandClient.TileManager.StartReadingsAsync();
                    _viewModel.StatusMessage = "Check the Tile on your Band (it's the last Tile). Waiting for events ...";
                }
                else
                {
                    await _bandClient.TileManager.StopReadingsAsync();
                    _bandClient.Dispose();
                    _bandClient = null;
                    _viewModel.StatusMessage = "Done.";
                }

            });
        }

        private void TileManager_TileButtonPressed(object sender, BandTileEventArgs<IBandTileButtonPressedEvent> e)
        {
            Dispatcher.RunAsync(
                 CoreDispatcherPriority.Normal,
                 () =>
                 {
                     _viewModel.StatusMessage = $"TileButtonPressed = {e.TileEvent.ElementId}";
                 }
             );
        }

        #region Record and play
        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(() =>
            {
                _viewModel.StatusMessage = "Starting ...";
                if (_viewModel.Playing)
                {
                    if (_viewModel.CurrentRecord != null)
                    {
                        _viewModel.Player = new DataPlayer(_viewModel.Config)
                        {
                            Record = _viewModel.CurrentRecord
                        };
                        _viewModel.Producer = _viewModel.Player as ISensorDataProducer;
                        _viewModel.StatusMessage = "Playing...";
                    }
                    else
                        _viewModel.StatusMessage = "No Record to Play loaded.";
                }
                else
                {
                    _viewModel.Producer = new BandManager(BandClientManager.Instance, _viewModel.Config);
                    _viewModel.Recorder = new DataRecorder(_viewModel.Producer, _viewModel.Config);
                    _viewModel.StatusMessage = "Recording...";
                }

                _viewModel.Root.Navigate(typeof(DataPage));
            });
        }


        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(async () =>
            {
                _viewModel.StatusMessage = "Stopping ...";
                if (_viewModel.Playing)
                    await _viewModel.Player.StopAsync();
                else
                    await _viewModel.Recorder.StopAsync();

                _viewModel.StatusMessage = "Stopped";
            });
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(async () =>
            {
                _viewModel.StatusMessage = "Pausing ...";
                if (_viewModel.Player != null)
                    await _viewModel.Player.PauseAsync();
                if (_viewModel.Recorder != null)
                    await _viewModel.Recorder.PauseAsync();

                _viewModel.StatusMessage = "Paused";
            });
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(async () =>
            {
                if (_viewModel.Playing)
                {
                    _viewModel.StatusMessage = "";
                    FileOpenPicker picker = new FileOpenPicker();
                    picker.ViewMode = PickerViewMode.Thumbnail;
                    picker.FileTypeFilter.Add(".bsd");
                    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                    StorageFile file = await picker.PickSingleFileAsync();
                    if (file != null)
                    {
                        string jsonText = await FileIO.ReadTextAsync(file);
                        _viewModel.CurrentRecord = JsonRecordPersistor.Deserialize(jsonText);
                        _viewModel.FilePath = file.Name;
                        _viewModel.StatusMessage = "File loaded";
                    }
                    else
                        _viewModel.StatusMessage = "No file selected";
                }
                else
                    _viewModel.StatusMessage = "Invalid State";
            });
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(async () =>
            {
                _viewModel.StatusMessage = "";
                FileSavePicker picker = new FileSavePicker();
                picker.FileTypeChoices.Add("file style", new string[] { ".bsd" });
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.SuggestedFileName = _viewModel.FilePath;
                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, JsonRecordPersistor.Serialize(_viewModel.Recorder?.Record));
                    _viewModel.StatusMessage = "File saved";
                }
                else
                    _viewModel.StatusMessage = "No file selected";
            });
        }
        #endregion

        #region Detection And Gestures
        private void ButtonAddGesture_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(() =>
            {
                _viewModel.StatusMessage = "";
                if (_viewModel.Recognizer == null)
                    _viewModel.Recognizer = new UWaveRecognizer();
                var name = Path.GetFileNameWithoutExtension(_viewModel.FilePath);
                var gesture = new UWaveGesture(name, _viewModel.CurrentRecord.Accelerometer.ToList());
                _viewModel.Gestures.Add(gesture);
                _viewModel.Recognizer.AddGesture(name, gesture);
            });
        }

        private void ButtonRecognize_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(async () =>
            {
                if (_viewModel.Recognizer == null)
                    return;
                _viewModel.StatusMessage = "";
                IGesture result = null;
                var data = _viewModel.Playing ? _viewModel.CurrentRecord.Accelerometer.ToList() : _viewModel.Recorder?.Record.Accelerometer.ToList();
                if (data.Any())
                {
                    await Task.Run(() => { result = _viewModel.Recognizer.Recognize(data); });
                }

                if (result != null)
                    _viewModel.StatusMessage = $"Detected gesture: {result.Name}";
                else
                    _viewModel.StatusMessage = $"No gesture detected!";
            });
        }

        private void ButtonDetect_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick(async () =>
           {
               _viewModel.StatusMessage = "";
               if (_viewModel.PPDetector == null)
               {

                   if (_viewModel.Producer == null)
                       _viewModel.Producer = new BandManager(BandClientManager.Instance, _viewModel.Config);

                   _viewModel.PPDetector = new AccelerometerGestureDetector(_viewModel.Producer, new BaselConfiguration());
                   await ConfigureDetector();
                   await _viewModel.PPDetector.StartDetectionAsync();
               }
               else
                   await _viewModel.PPDetector.StopDetectionAsync();
           });

        }


        private async Task ConfigureDetector()
        {
            var folder = KnownFolders.PicturesLibrary;
            foreach (var file in (await folder.GetFilesAsync()).Where(x => x.Name.EndsWith(".bsd")))
            {
                string jsonText = await FileIO.ReadTextAsync(file);
                var record = JsonRecordPersistor.Deserialize(jsonText);
                var name = file.Name.Substring(0, file.Name.IndexOf("."));
                _viewModel.PPDetector.AddRecordAsGesture(name, record, () =>
                {
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => _viewModel.StatusMessage = $"{DateTime.Now}:{name}");
                    if (_viewModel.SliderCtrl != null)
                    {
                        switch (name)
                        {
                            case "next":
                                _viewModel.SliderCtrl.NextAsync();
                                break;
                            case "prev":
                                _viewModel.SliderCtrl.PrevAsync();
                                break;
                            case "start":
                                _viewModel.SliderCtrl.StartAsync();
                                break;
                            case "stop":
                                _viewModel.SliderCtrl.StopAsync();
                                break;
                        }
                    }
                });

            }

            _viewModel.StatusMessage = "Detecting...";
        }
        #endregion

        #region Helper

        private void ButtonClick(Action action)
        {
            if (_handlingClick)
                return;
            _handlingClick = true;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _viewModel.StatusMessage = ex.ToString();
            }
            finally
            {
                _handlingClick = false;
            }
        }


        private async Task<BandIcon> LoadIcon(string uri)
        {
            StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                WriteableBitmap bitmap = new WriteableBitmap(1, 1);
                await bitmap.SetSourceAsync(fileStream);
                return bitmap.ToBandIcon();
            }
        }


        #endregion
    }
}
