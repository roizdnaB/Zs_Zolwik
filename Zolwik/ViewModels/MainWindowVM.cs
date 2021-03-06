﻿using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MVVM.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TurtleSharp;
using System.Linq;
using MVVM.DialogBoxes;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Threading;
using TurtleSharp.WPF;
using System.Windows.Input;
using System.Windows.Threading;
using MVVM.Commands;
using Zolwik.Helpers;

namespace Zolwik.ViewModels
{
    internal class MainWindowVM : ViewModelBase
    {
        private Turtle _turtle = new Turtle();

        private string _text = string.Empty;
        private string _path = null;
        private string _pathToSave = null;
        public string Path { get => _path; set => _path = value; }
        public string PathToSave { get => _pathToSave; set => _pathToSave = value; }
        private ITurtlePresentation _canvas;
        private CancellationTokenSource _cts;
        private CancellationToken _currentCancellationToken;
        private bool _canvasBusy;
        public bool CanvasBusy { get => _canvasBusy; set {
                _canvasBusy = value;
                OnPropertyChanged(nameof(CanvasBusy)
                    ); } } 
        public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }
        public ITurtlePresentation TurtlePresentationHook { get => _canvas; set { _canvas = value; OnPropertyChanged(nameof(TurtlePresentationHook)); } }
        public RelayCommand LoadTextFromFile { get; private set; }
        public RelayCommand SaveTextFromFile { get; private set; }
        public RelayCommand SaveAsTextFromFile { get; private set; }
        public RelayCommand CopyText { get; private set; }
        public ICommand Run { get; private set; }
        public RelayCommand Abort { get; private set; }
        public RelayCommand Clean { get; private set; }
        public RelayCommand About { get; private set; }
        public RelayCommand ShowExampleCode { get; private set; }
        public RelayCommand SaveAsJPG { get; private set; }
        public RelayCommand SaveAsPNG { get; private set; }
        public RelayCommand SaveAsSVG { get; private set; }
        public RelayCommand SaveAsBTM { get; private set; }

        private Dispatcher Dispatcher = Application.Current.Dispatcher;

        //The 'wow' function
        private void _saveAsSVG(object path)
        {
            string FilePath = path as string;

            //Get the code of the SVG file from the converter
            string code = SVGConverter.GetSVGCode((_canvas as Canvas).Children.OfType<Line>().ToList());

            //Save it as svg
            File.WriteAllText(FilePath, code);

            Path = FilePath;
        }

        private CroppedBitmap bitmapHelper()
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap(5000, 5000, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render((System.Windows.Media.Visual)_canvas);

            var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)Math.Ceiling((_canvas as Canvas).ActualWidth), (int)Math.Ceiling((_canvas as Canvas).ActualHeight)));

            return crop;
        }

        private void _saveAsBTM(object path)
        {
           // _turtle.IsVisible = false;
            string FilePath = path as string;

            var crop = bitmapHelper();

            BitmapEncoder btmEncoder = new BmpBitmapEncoder();
            btmEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = File.OpenWrite(FilePath))
            {
                btmEncoder.Save(fs);
            }
          //  _turtle.IsVisible = true;
        }

        private void _saveAsJPG(object path)
        {
        //    _turtle.IsVisible = false;
            string FilePath = path as string;
            //Nie działa dobrze
            var crop = bitmapHelper();

            BitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = File.OpenWrite(FilePath))
            {
                jpgEncoder.Save(fs);
            }

          //  _turtle.IsVisible = true;
        }

        private void _saveAsPNG(object path)
        {
           // _turtle.IsVisible = false;
            string FilePath = path as string;

            var crop = bitmapHelper();

            BitmapEncoder pngEncoder = new JpegBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = File.OpenWrite(FilePath))
            {
                pngEncoder.Save(fs);
            }
            _turtle.IsVisible = true;
        }

        private void _loadTextFromFileCommand(object path)
        {
            string FilePath = path as string;

            if (File.Exists(FilePath))
            {
                Text = File.ReadAllText(FilePath);
            }
        }

        private void _saveAsTextFromFileCommand(object path)
        {
            string FilePath = path as string;

            File.WriteAllText(FilePath, Text);
            Path = FilePath;
        }

        private void _saveTextFromFileCommand()
        {
            if (PathToSave == null)
            {
                var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\script";

                var dialogBox = new SaveFileDialogBox()
                {
                    Name = "SaveFile",
                    Filter = "Text Files txt files (*.txt)|*.txt",
                    DefaultExtension = "txt",
                    FilterIndex = 0,
                    FilePath = filePath,
                    CommandFileOk = SaveAsTextFromFile
                };

                dialogBox.Show.Execute(null);

                if (dialogBox.FileDialogResult == true)
                { 
                    PathToSave = dialogBox.FilePath;
                }

            }
            _saveAsTextFromFileCommand(PathToSave);
        }

        private void _showExampleCode(object fileName)
        {
            string File = fileName.ToString();
            string FilePath = $"{Environment.GetEnvironmentVariable("AppData")}\\Zolwik\\Examples\\{File}.txt";
            //_canvas.Clear();
            _loadTextFromFileCommand(FilePath);
        }

        private void _copyTextCommand()
        {
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.UnicodeText, Text);
        }

        private void _aboutCommand()
        {
            var dialogBox = new MessageDialogBox()
            {
                Caption = "O projekcie",
                Icon = System.Windows.MessageBoxImage.Information,
                Buttons = System.Windows.MessageBoxButton.OK
            };
            dialogBox.showMessageBox("Projekt zaliczeniowy z przedmiotu Inżynieria Oprogramowania. \n\n Natalia Szarek, Krzysztof Kłak, Daniel Jambor");
        }

        private void _abortCommand()
        {
            if (_cts != null && _currentCancellationToken != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
                _currentCancellationToken = CancellationToken.None;

                _cts = new CancellationTokenSource();
                _currentCancellationToken = _cts.Token;
                (_canvas as TurtleCanvas).CancellationToken = _currentCancellationToken;

                _canvas.Clear();
            }
        }

        private void _cleanCommand()
        {
            TurtlePresentationHook.Clear();
        }

        public MainWindowVM()
        {
            LoadTextFromFile = new RelayCommand(arg => _loadTextFromFileCommand(arg));
            SaveTextFromFile = new RelayCommand(arg => _saveTextFromFileCommand());
            SaveAsTextFromFile = new RelayCommand(arg => _saveAsTextFromFileCommand(arg));
            CopyText = new RelayCommand(arg => _copyTextCommand());
            About = new RelayCommand(arg => _aboutCommand());
            ShowExampleCode = new RelayCommand(arg => _showExampleCode(arg));
            SaveAsJPG = new RelayCommand(arg => _saveAsJPG(arg));
            SaveAsPNG = new RelayCommand(arg => _saveAsPNG(arg));
            SaveAsSVG = new RelayCommand(arg => _saveAsSVG(arg));
            SaveAsBTM = new RelayCommand(arg => _saveAsBTM(arg));
            Abort = new RelayCommand(arg => _abortCommand(), arg => CanvasBusy);
            Clean = new RelayCommand(arg => _cleanCommand(), arg => !CanvasBusy);

            Run = new RelayCommand(
                arg =>
                {
                    CanvasBusy = true;

                    _cts = new CancellationTokenSource();
                    _currentCancellationToken = _cts.Token;
                    (_canvas as TurtleCanvas).CancellationToken = _currentCancellationToken;

                    var task = Task.Run(() =>
                    {
                        try
                        {
                            CSharpScript.RunAsync(
                                Text,
                                globals: new TurtleCanvasPair { Turtle = _turtle, Canvas = TurtlePresentationHook },
                                globalsType: typeof(TurtleCanvasPair),
                                options: ScriptOptions.Default.WithEmitDebugInformation(true));
                        }
                        catch (Exception e)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                var dialogBox = new MessageDialogBox()
                                {
                                    Caption = "Błąd kompilacji",
                                    Icon = System.Windows.MessageBoxImage.Warning,
                                    Buttons = System.Windows.MessageBoxButton.OK
                                };
                                dialogBox.showMessageBox(e.Message);
                            });
                            CanvasBusy = false;
                        }

                    });

                    task.ContinueWith(prev =>
                    {
                    if (prev.IsFaulted)
                    {
                        var e = prev.Exception.InnerException;

                        if (e is CompilationErrorException)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                var dialogBox = new MessageDialogBox()
                                {
                                    Caption = "Błąd kompilacji",
                                    Icon = System.Windows.MessageBoxImage.Warning,
                                    Buttons = System.Windows.MessageBoxButton.OK
                                };
                                dialogBox.showMessageBox(e.Message);
                            });
                        }
                        else if (e is OperationCanceledException)
                        {
                            Dispatcher.Invoke(() => {
                                var dialogBox = new MessageDialogBox()
                                {
                                    Caption = "Anulowano",
                                    Icon = System.Windows.MessageBoxImage.Information,
                                    Buttons = System.Windows.MessageBoxButton.OK
                                };
                                dialogBox.showMessageBox(e.Message);
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() => {
                                var dialogBox = new MessageDialogBox()
                                {
                                    Caption = "Błąd programu",
                                    Icon = System.Windows.MessageBoxImage.Information,
                                    Buttons = System.Windows.MessageBoxButton.OK
                                };
                                dialogBox.showMessageBox(e.Message);
                            });
                        }
                        CanvasBusy = false;
                    }
                });
            }, arg => !CanvasBusy);
        }
    }

    public class TurtleCanvasPair
    {
        public Turtle Turtle { get; set; }
        public ITurtlePresentation Canvas { get; set; }
    }
}