using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MVVM.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TurtleSharp;
using Zolwik.Compiler;
using System.Linq;
using Zolwik.DialogBoxes;
using System.Windows.Shapes;
using Zolwik.Helpers;
using System.Threading.Tasks;
using System.Threading;
using TurtleSharp.WPF;
using System.Windows.Input;
using System.Windows.Threading;

namespace Zolwik.ViewModels
{
    internal class MenuVM : ViewModelBase
    {
        private Turtle _turtle = new Turtle();

        private string _text = string.Empty;
        private string _path = null;
        public string Path { get => _path; set => _path = value; }
        private ITurtlePresentation _canvas;
        private CancellationTokenSource _cts;
        private CancellationToken? _currentCancellationToken;
        public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }
        public ITurtlePresentation TurtlePresentationHook { get => _canvas; set { _canvas = value; OnPropertyChanged(nameof(TurtlePresentationHook)); } }
        public RelayCommand LoadTextFromFile { get; private set; }
        public RelayCommand SaveTextFromFile { get; private set; }
        public RelayCommand SaveAsTextFromFile { get; private set; }
        public RelayCommand CopyText { get; private set; }
        public ICommand Run { get; private set; }
        public RelayCommand Abort { get; private set; }
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
            RenderTargetBitmap rtb = new RenderTargetBitmap(500, 500, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render((System.Windows.Media.Visual)_canvas);

            var crop = new CroppedBitmap(rtb, new Int32Rect(50, 50, 250, 250));

            return crop;
        }

        private void _saveAsBTM(object path)
        {
            _turtle.IsVisible = false;
            string FilePath = path as string;

            var crop = bitmapHelper();

            BitmapEncoder btmEncoder = new BmpBitmapEncoder();
            btmEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = File.OpenWrite(FilePath))
            {
                btmEncoder.Save(fs);
            }
            _turtle.IsVisible = true;
        }

        private void _saveAsJPG(object path)
        {
            _turtle.IsVisible = false;
            string FilePath = path as string;
            //Nie działa dobrze
            var crop = bitmapHelper();

            BitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = File.OpenWrite(FilePath))
            {
                jpgEncoder.Save(fs);
            }

            _turtle.IsVisible = true;
        }

        private void _saveAsPNG(object path)
        {
            _turtle.IsVisible = false;
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

        private void _saveTextFromFileCommand(object path)
        {
            string FilePath = path as string;
            if (Path == null)
            {
                Path = FilePath;
            }
            File.WriteAllText(Path, Text);
        }

        private void _showExampleCode(object fileName)
        {
            string File = fileName.ToString();
            string FilePath = $"{Environment.GetEnvironmentVariable("AppData")}\\Zolwik\\Examples\\{File}.txt";
            _canvas.Clear();
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
                _currentCancellationToken = null;
            }
        }

        public MenuVM()
        {
            LoadTextFromFile = new RelayCommand(arg => _loadTextFromFileCommand(arg));
            SaveTextFromFile = new RelayCommand(arg => _saveTextFromFileCommand(arg));
            SaveAsTextFromFile = new RelayCommand(arg => _saveAsTextFromFileCommand(arg));
            CopyText = new RelayCommand(arg => _copyTextCommand());
            About = new RelayCommand(arg => _aboutCommand());
            ShowExampleCode = new RelayCommand(arg => _showExampleCode(arg));
            SaveAsJPG = new RelayCommand(arg => _saveAsJPG(arg));
            SaveAsPNG = new RelayCommand(arg => _saveAsPNG(arg));
            SaveAsSVG = new RelayCommand(arg => _saveAsSVG(arg));
            SaveAsBTM = new RelayCommand(arg => _saveAsBTM(arg));
            Abort = new RelayCommand(arg => _abortCommand());

            Run = new RelayCommand(
                arg =>
                {

                    _canvas.Clear();
                    _cts = new CancellationTokenSource();
                    _currentCancellationToken = _cts.Token;

                    (_canvas as TurtleCanvas).CancellationToken = _currentCancellationToken;

                    var task = Task.Run(() =>
                    {
                        CSharpScript.RunAsync(
                            Text,
                            globals: new TurtleCanvasPair { Turtle = _turtle, Canvas = TurtlePresentationHook },
                            globalsType: typeof(TurtleCanvasPair),
                            options: ScriptOptions.Default.WithEmitDebugInformation(true));
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
                    }
                });
            });
        }
    }

    public class TurtleCanvasPair
    {
        public Turtle Turtle { get; set; }
        public ITurtlePresentation Canvas { get; set; }
    }
}