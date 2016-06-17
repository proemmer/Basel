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

using Basel;
using Basel.Detection;
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
using System.Threading.Tasks;
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
        private ButtonKind _buttonKind;
        private bool _handlingClick;

        private async void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            if (_handlingClick)
            {
                return;
            }

            this._viewModel.StatusMessage = "Starting ...";

            _handlingClick = true;
            try
            {
                _buttonKind = ButtonKindFromModel();

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
            }
            catch (Exception ex)
            {
                this._viewModel.StatusMessage = ex.ToString();
            }
            finally
            {
                _handlingClick = false;
            }
        }

        private async void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (_handlingClick)
            {
                return;
            }

            _viewModel.StatusMessage = "Stopping ...";

            _handlingClick = true;
            try
            {
                if (_viewModel.Playing)
                    await _viewModel.Player.StopAsync();
                else
                    await _viewModel.Recorder.StopAsync();

                this._viewModel.StatusMessage = "Stopped";
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

        private async void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (_handlingClick)
            {
                return;
            }

            this._viewModel.StatusMessage = "Pausing ...";

            _handlingClick = true;
            try
            {
                if (_viewModel.Player != null)
                    await _viewModel.Player.PauseAsync();
                if (_viewModel.Recorder != null)
                    await _viewModel.Recorder.PauseAsync();

                this._viewModel.StatusMessage = "Paused";
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

        private async void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            if (_handlingClick)
            {
                return;
            }

            _handlingClick = true;
            try
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
            }
            catch (Exception ex)
            {
                this._viewModel.StatusMessage = ex.ToString();
            }
            finally
            {
                _handlingClick = false;
            }


        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (_handlingClick)
            {
                return;
            }

            _handlingClick = true;
            try
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
                    this._viewModel.StatusMessage = "File saved";
                }
                else
                    this._viewModel.StatusMessage = "No file selected";
            }
            catch (Exception ex)
            {
                this._viewModel.StatusMessage = ex.ToString();
            }
            finally
            {
                _handlingClick = false;
            }


        }


        private void ButtonAddGesture_Click(object sender, RoutedEventArgs e)
        {
            if (_handlingClick)
            {
                return;
            }

            _handlingClick = true;
            try
            {
                this._viewModel.StatusMessage = "";
                if(_viewModel.Recognizer == null)
                    _viewModel.Recognizer = new UWaveRecognizer();
                var name = Path.GetFileNameWithoutExtension(_viewModel.FilePath);
                var gesture = new UWaveGesture(name, _viewModel.CurrentRecord.Accelerometer.ToList());
                _viewModel.Gestures.Add(gesture);
                _viewModel.Recognizer.AddGesture(name, gesture);



            }
            catch (Exception ex)
            {
                this._viewModel.StatusMessage = ex.ToString();
            }
            finally
            {
                _handlingClick = false;
            }
        }

        private async void ButtonRecognize_Click(object sender, RoutedEventArgs e)
        {
            if (_handlingClick || _viewModel.Recognizer == null)
            {
                return;
            }

            _handlingClick = true;
            try
            {
                _viewModel.StatusMessage = "";
                IGesture result = null;
                var data = _viewModel.Playing ? _viewModel.CurrentRecord.Accelerometer.ToList() : _viewModel.Recorder?.Record.Accelerometer.ToList();
                if (data.Any())
                {
                    await Task.Run(() =>
                   {
                       result = _viewModel.Recognizer.Recognize(data, false);
                   });
                }

                //if(result != null)
                //    _viewModel.StatusMessage = $"{result.Name}: {Math.Round(result.Score, 2)} ({Math.Round(result.Distance, 2)}px, {Math.Round(result.Angle, 2)}{(char)176})";
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


        private async Task BuildLayout(BandTile myTile)
        {
            FilledPanel panel = new FilledPanel() { Rect = new PageRect(0, 0, 220, 150) };

            PageElement buttonElement = null;

            switch (_buttonKind)
            {
                case ButtonKind.Text:
                    buttonElement = new TextButton();
                    break;

                case ButtonKind.Filled:
                    buttonElement = new FilledButton() { BackgroundColor = new BandColor(0, 128, 0) };
                    break;

                case ButtonKind.Icon:
                    buttonElement = new IconButton();
                    myTile.AdditionalIcons.Add(await LoadIcon("ms-appx:///Assets/Smile.png"));
                    myTile.AdditionalIcons.Add(await LoadIcon("ms-appx:///Assets/SmileUpsideDown.png"));
                    break;

                default:
                    throw new NotImplementedException();
            }

            buttonElement.ElementId = 1;
            buttonElement.Rect = new PageRect(10, 10, 200, 90);

            panel.Elements.Add(buttonElement);

            myTile.PageLayouts.Add(new PageLayout(panel));
        }
        
        private PageElementData GetPageElementData()
        {
            switch (_buttonKind)
            {
                case ButtonKind.Text:
                    return new TextButtonData(1, "Click here");

                case ButtonKind.Filled:
                    return new FilledButtonData(1, new BandColor(128, 0, 0));

                case ButtonKind.Icon:
                    return new IconButtonData(
                        elementId: 1,
                        iconIndex: 2, // The first 2 indexes are taken by the tile icons.
                        pressedIconIndex: 3, // The first 2 indexes are taken by the tile icons.
                        iconColor: new BandColor(0, 128, 0),
                        pressedIconColor: new BandColor(128, 0, 0),
                        backgroundColor: new BandColor(0x35, 0x35, 0x35),
                        pressedBackgroundColor: new BandColor(0x20, 0x20, 0x20));

                default:
                    throw new NotImplementedException();
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

        ButtonKind ButtonKindFromModel()
        {
            if (this._viewModel.UseTextButton)
            {
                return ButtonKind.Text;
            }
            else if (this._viewModel.UseFilledButton)
            {
                return ButtonKind.Filled;
            }
            else if (this._viewModel.UseIconButton)
            {
                return ButtonKind.Icon;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private enum ButtonKind
        {
            Text,
            Filled,
            Icon
        }

    }
}
