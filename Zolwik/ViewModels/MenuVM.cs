using MVVM.ViewModel;
using System.IO;
using TurtleSharp;

namespace Zolwik.ViewModels
{
    internal class MenuVM : ViewModelBase
    {
        private string _text = string.Empty;
        private ITurtlePresentation _canvas;
        public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }
        public ITurtlePresentation TurtlePresentationHook { get => _canvas; set { _canvas = value; OnPropertyChanged(nameof(TurtlePresentationHook)); } }
        public RelayCommand LoadTextFromFile { get; private set; }
        public RelayCommand SaveTextFromFile { get; private set; }
        public RelayCommand RunTurtle { get; private set; }

        private void _loadTextFromFileCommand(object path)
        {
            //Code of turtle

            var turtle = new Turtle();
            _canvas.PlaceTurtle(turtle);
            //_canvas.TurtleForward(turtle, 150);

            //End of turtle

            string FilePath = path as string;

            if (File.Exists(FilePath))
            {
                Text = File.ReadAllText(FilePath);
            }
        }

        private void _saveTextFromFileCommand(object path)
        {
            string FilePath = path as string;

            File.WriteAllText(FilePath, Text);
        }

        public MenuVM()
        {
            LoadTextFromFile = new RelayCommand(arg => _loadTextFromFileCommand(arg));
            SaveTextFromFile = new RelayCommand(arg => _saveTextFromFileCommand(arg));
        }
    }
}