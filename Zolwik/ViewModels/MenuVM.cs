using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MVVM.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using TurtleSharp;
using Zolwik.Compiler;
using Zolwik.DialogBoxes;

namespace Zolwik.ViewModels
{
    internal class MenuVM : ViewModelBase
    {
        private ITurtleCommandsCompiler _compiler = new RoslynTurtleCommandsCompiler();
        private Turtle Turtle = new Turtle();

        private string _text = string.Empty;
        private string _path = null;
        public string Path { get => _path; set => _path = value; }
        private ITurtlePresentation _canvas;
        public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }
        public ITurtlePresentation TurtlePresentationHook { get => _canvas; set { _canvas = value; OnPropertyChanged(nameof(TurtlePresentationHook)); } }
        public RelayCommand LoadTextFromFile { get; private set; }
        public RelayCommand SaveTextFromFile { get; private set; }
        public RelayCommand SaveAsTextFromFile { get; private set; }
        public RelayCommand CopyText { get; private set; }
        public RelayCommand Run { get; private set; }
        public RelayCommand About { get; private set; }
        public RelayCommand ShowExampleCode { get; private set; }
        public RelayCommand SaveAsJPG { get; private set; }

        private void _saveAsJPG()
        {
            
            RenderTargetBitmap rtb = new RenderTargetBitmap(6000,
    6000, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render((System.Windows.Media.Visual)_canvas);

            var crop = new CroppedBitmap(rtb, new Int32Rect(50, 50, 250, 250));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = File.OpenWrite("turtle.png"))
            {
                pngEncoder.Save(fs);
            }
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
                Icon = System.Windows.MessageBoxImage.Warning,
                Buttons = System.Windows.MessageBoxButton.OK
            };
            dialogBox.showMessageBox("Projekt zaliczeniowy z przedmiotu Inżynieria Oprogramowania. \n\n Natalia Szarek, Krzysztof Kłak, Daniel Jambor");
        }

        public MenuVM()
        {
            LoadTextFromFile = new RelayCommand(arg => _loadTextFromFileCommand(arg));
            SaveTextFromFile = new RelayCommand(arg => _saveTextFromFileCommand(arg));
            SaveAsTextFromFile = new RelayCommand(arg => _saveAsTextFromFileCommand(arg));
            CopyText = new RelayCommand(arg => _copyTextCommand());
            About = new RelayCommand(arg => _aboutCommand());
            ShowExampleCode = new RelayCommand(arg => _showExampleCode(arg));
            SaveAsJPG = new RelayCommand(arg => _saveAsJPG());
            Run = new RelayCommand(
                arg =>
                {
                    _canvas.Clear();
                    CSharpScript.RunAsync(
                        Text,
                        globals: new TurtleCanvasPair { Turtle = Turtle, Canvas = TurtlePresentationHook },
                        globalsType: typeof(TurtleCanvasPair),
                        options: ScriptOptions.Default.WithEmitDebugInformation(true)
                    );
                }
            );
        }
    }

    public class TurtleCanvasPair
    {
        public Turtle Turtle { get; set; }
        public ITurtlePresentation Canvas { get; set; }
    }
}