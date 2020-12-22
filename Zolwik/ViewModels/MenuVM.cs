using MVVM.ViewModel;
using System.IO;
using System.Windows;
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
            Run = new RelayCommand(
                arg =>
                {
                    _compiler.CompileTurtleCommands(Text).Invoke(Turtle, TurtlePresentationHook);
                }
            );
        }
    }
}