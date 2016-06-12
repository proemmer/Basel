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
using Basel.Detection.Recognizer.Dollar;
using Basel.Recorder;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
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
        private ISensorDataProducer _producer;
        private IDataRecorder _recorder;
        private IDataPlayer _player;
        private IRecord _currentRecord;

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

                    if (_currentRecord != null)
                    {
                        _player = new DataPlayer(_viewModel.Config)
                        {
                            Record = _currentRecord
                        };

                        await _player.StartAsync();
                        _viewModel.StatusMessage = "Playing...";
                    }
                    else
                        _viewModel.StatusMessage = "No Record to Play loaded.";
                }
                else
                {
                    _producer = new BandManager(BandClientManager.Instance, _viewModel.Config);
                    _recorder = new DataRecorder(_producer, _viewModel.Config);

                    await _recorder.StartAsync();
                    this._viewModel.StatusMessage = "Recording...";
                }
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

            this._viewModel.StatusMessage = "Stopping ...";

            _handlingClick = true;
            try
            {
                if (_viewModel.Playing)
                    await _player.StopAsync();
                else
                    await _recorder.StopAsync();

                this._viewModel.StatusMessage = "Stopped";
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
                if (_player != null)
                    await _player.PauseAsync();
                if (_recorder != null)
                    await _recorder.PauseAsync();

                this._viewModel.StatusMessage = "Paused";
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
                    this._viewModel.StatusMessage = "";
                    FileOpenPicker picker = new FileOpenPicker();
                    picker.ViewMode = PickerViewMode.Thumbnail;
                    picker.FileTypeFilter.Add(".bsd");
                    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                    StorageFile file = await picker.PickSingleFileAsync();
                    if (file != null)
                    {
                        string jsonText = await FileIO.ReadTextAsync(file);
                        _currentRecord = JsonRecordPersistor.Deserialize(jsonText);
                        _viewModel.FilePath = file.Name;
                        this._viewModel.StatusMessage = "File loaded";
                    }
                    else
                        this._viewModel.StatusMessage = "No file selected";
                }
                else
                    this._viewModel.StatusMessage = "Invalid State";
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
                this._viewModel.StatusMessage = "";
                FileSavePicker picker = new FileSavePicker();
                picker.FileTypeChoices.Add("file style", new string[] { ".bsd" });
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.SuggestedFileName = _viewModel.FilePath;
                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, JsonRecordPersistor.Serialize(_recorder?.Record));
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

        private async void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            if (_handlingClick)
            {
                return;
            }

            _handlingClick = true;
            try
            {
                this._viewModel.StatusMessage = "";
                await Task.Run(() =>
               {
                   var rec = new DollarRecognizer();

                   rec.AddGesture("Test", new Unistroke("Test", _currentRecord.Accelerometer.ToList()));
                   var result = rec.Recognize(_currentRecord.Accelerometer.ToList(), false);
               });



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
