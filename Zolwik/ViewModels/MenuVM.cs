using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using MVVM.ViewModel;
using System.Windows.Input;
using System.IO;

namespace Zolwik.ViewModels
{
     class MenuVM : ViewModelBase
    {
        private string _text = String.Empty;
        public string Text { get => _text; set { _text = value; OnPropertyChanged(nameof(Text)); } }
        public RelayCommand LoadTextFromFile { get; private set; }
        public RelayCommand SaveTextFromFile { get; private set; }


        private void _loadTextFromFileCommand(object path)
        {
            string FilePath = path as string;

            if (File.Exists(FilePath))
            {
                Text = File.ReadAllText(FilePath);
            }
        }

        private void _saveTextFromFileCommand(object path)
        {
            string FilePath = path as string;

            File.WriteAllText(FilePath,Text);
        }
        public MenuVM()
        {
            LoadTextFromFile = new RelayCommand(arg => _loadTextFromFileCommand(arg));
            SaveTextFromFile = new RelayCommand(arg => _saveTextFromFileCommand(arg));

        }
    }

}
