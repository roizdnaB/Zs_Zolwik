using System.Windows;
using System.Windows.Input;

namespace MVVM.DialogBoxes
{
    public abstract class FileDialogBox : CommandDialogBox
    {
        #region Pola klasy

        // Odpowiedź okna dialogowego
        public bool? FileDialogResult { get; protected set; }

        // Okno dialogowe dotyczące plików
        protected Microsoft.Win32.FileDialog fileDialog = null;

        #endregion Pola klasy

        #region Własności zależności i ich rejestracje

        // Ścieżka do pliku
        protected static readonly DependencyProperty filePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(FileDialogBox));

        // Filtr dla plików
        protected static readonly DependencyProperty filterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FileDialogBox));

        protected static readonly DependencyProperty filterIndexProperty =
            DependencyProperty.Register("FilterIndex", typeof(int), typeof(FileDialogBox));

        // Domyślne rozszerzenie pliku
        protected static readonly DependencyProperty defaultExtensionProperty =
            DependencyProperty.Register("DefaultExtension", typeof(string), typeof(FileDialogBox));

        // Polecenie w przypadku wybrania pliku
        protected static readonly DependencyProperty commandFileOkProperty =
            DependencyProperty.Register("CommandFileOk", typeof(ICommand), typeof(FileDialogBox));

        #endregion Własności zależności i ich rejestracje

        #region Właściwości

        public string FilePath
        {
            get { return (string)GetValue(filePathProperty); }
            set { SetValue(filePathProperty, value); }
        }

        public string Filter
        {
            get { return (string)GetValue(filterProperty); }
            set { SetValue(filterProperty, value); }
        }

        public int FilterIndex
        {
            get { return (int)GetValue(filterIndexProperty); }
            set { SetValue(filterIndexProperty, value); }
        }

        public string DefaultExtension
        {
            get { return (string)GetValue(defaultExtensionProperty); }
            set { SetValue(defaultExtensionProperty, value); }
        }

        public ICommand CommandFileOk
        {
            get { return (ICommand)GetValue(commandFileOkProperty); }
            set { SetValue(commandFileOkProperty, value); }
        }

        #endregion Właściwości

        #region Konstruktor

        protected FileDialogBox()
        {
            execute = o => setFileDialogBox(o);
        }

        #endregion Konstruktor

        #region Metody pomocnicze

        private void setFileDialogBox(object path)
        {
            // Ustawienie danych okna dialogowego
            fileDialog.Title = Caption;
            fileDialog.Filter = Filter;
            fileDialog.FilterIndex = FilterIndex;
            fileDialog.DefaultExt = DefaultExtension;

            // Ścieżka do pliku
            string fileToPath = "";

            if (FilePath != null)
                fileToPath = FilePath;
            else
                FilePath = "";

            if (path != null)
                fileToPath = (string)path;

            if (!string.IsNullOrWhiteSpace(fileToPath))
            {
                // Katalog początkowy
                fileDialog.InitialDirectory =
                System.IO.Path.GetDirectoryName(fileToPath);
                // Nazwa pliku
                fileDialog.FileName = System.IO.Path.GetFileName(fileToPath);
            }

            // Odpowiedź okna dialogowego
            FileDialogResult = fileDialog.ShowDialog();
            OnPropertyChanged("FileDialogResult");
            if (FileDialogResult.HasValue && FileDialogResult.Value)
            {
                FilePath = fileDialog.FileName;
                OnPropertyChanged("FilePath");
                object commandParameter = CommandParameter;
                // Jeśli parametr jest null, to ustalamy go jako ścieżka do pliku
                if (commandParameter == null)
                    commandParameter = FilePath;
                // Uruchamiamy polecenie CommandFileOk,
                // a jako parametr przekazujemy (domyślnie!) ściężkę do pliku
                executeCommand(CommandFileOk, commandParameter);
            }
        }

        #endregion Metody pomocnicze
    }
}