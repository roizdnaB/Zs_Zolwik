using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MVVM.Commands;

namespace MVVM.DialogBoxes
{
    // Publiczna klasa abstrakcyjna, będąca klasą bazową dla okien dialogowych
    // Dziedziczy po klasie FrameworkElement i implementuje interfejs INotifyPropertyChanged
    public abstract class CommandDialogBox : FrameworkElement, INotifyPropertyChanged
    {
        #region Implementacja interfejsu INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string nazwaWlasnosci)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nazwaWlasnosci));
        }

        #endregion Implementacja interfejsu INotifyPropertyChanged

        #region Pola klasy

        // Akcja uruchamiająca odpowiednie okno dialogowe.
        // Samo okno będzie definiowane w klasach potomnych
        protected Action<object> execute = null;

        // Polecenie pokazania okna
        protected ICommand show;

        #endregion Pola klasy

        #region Własności zależności i ich rejestracje

        // Tytuł okna dialogowego
        protected static readonly DependencyProperty captionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(CommandDialogBox));

        // Parametr polecenia
        protected static readonly DependencyProperty commandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandDialogBox));

        // Polecenie przed wyświetleniem okna dialogowego
        protected static readonly DependencyProperty commandBeforeProperty =
            DependencyProperty.Register("CommandBefore", typeof(ICommand), typeof(CommandDialogBox));

        // Polecenie po zamknięciu okna dialogowego
        protected static readonly DependencyProperty commandAfterProperty =
            DependencyProperty.Register("CommandAfter", typeof(ICommand), typeof(CommandDialogBox));

        #endregion Własności zależności i ich rejestracje

        #region Własności publiczne

        // Tytuł okna dialogowego
        public string Caption
        {
            get { return (string)GetValue(captionProperty); }
            set { SetValue(captionProperty, value); }
        }

        // Parametr polecenia
        public object CommandParameter
        {
            get { return GetValue(commandParameterProperty); }
            set { SetValue(commandParameterProperty, value); }
        }

        // Polecenie przed wyświetleniem okna dialogowego
        public ICommand CommandBefore
        {
            get { return (ICommand)GetValue(commandBeforeProperty); }
            set { SetValue(commandBeforeProperty, value); }
        }

        // Polecenie po zamknięciu okna dialogowego
        public ICommand CommandAfter
        {
            get { return (ICommand)GetValue(commandAfterProperty); }
            set { SetValue(commandAfterProperty, value); }
        }

        // Polecenia pokazania okna
        public ICommand Show
        {
            get
            {
                if (show == null)
                    show = new RelayCommand(
                        o =>
                        {
                            // Uruchom polecenie przed wyświetleniem okna
                            executeCommand(CommandBefore, CommandParameter);
                            // Uruchom polecenie wyświetlające okno
                            execute(o);
                            // Uruchom polecenie po zamknięciu okna
                            executeCommand(CommandAfter, CommandParameter);
                        },
                        arg => true
                        );
                return show;
            }
        }

        #endregion Własności publiczne

        #region Metody pomocnicze

        // Metoda uruchamiająca przekazane polecenie wraz z parametrem polecenia
        protected static void executeCommand(ICommand command, object commandParameter)
        {
            if (command != null)
                if (command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
        }

        #endregion Metody pomocnicze
    }
}