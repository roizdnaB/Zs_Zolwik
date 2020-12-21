using System.Windows;
using System.Windows.Input;

namespace Zolwik.DialogBoxes
{
    internal class MessageDialogBox : CommandDialogBox
    {
        #region Własności zależności i ich rejestracje

        // Polecenie, gdy wybrano odpowiedź MessageBoxResult.Yes
        protected static readonly DependencyProperty commandYesProperty =
            DependencyProperty.Register("CommandYes", typeof(ICommand), typeof(MessageDialogBox));

        // Polecenie, gdy wybrano odpowiedź MessageBoxResult.No
        protected static readonly DependencyProperty commandNoProperty =
            DependencyProperty.Register("CommandNo", typeof(ICommand), typeof(MessageDialogBox));

        // Polecenie, gdy wybrano odpowiedź MessageBoxResult.Cancel
        protected static readonly DependencyProperty commandCancelProperty =
            DependencyProperty.Register("CommandCancel", typeof(ICommand), typeof(MessageDialogBox));

        // Polecenie, gdy wybrano odpowiedź MessageBoxResult.OK
        protected static readonly DependencyProperty commandOKProperty =
            DependencyProperty.Register("CommandOK", typeof(ICommand), typeof(MessageDialogBox));

        // Czy okno ma zostać wyświetlone
        public static DependencyProperty IsMessageDialogShowProperty =
            DependencyProperty.Register("IsMessageDialogShow", typeof(bool), typeof(MessageDialogBox));

        #endregion Własności zależności i ich rejestracje

        #region Własności publiczne

        // W tej własności będzie zapisywana wartość zwracana przez metodę MessageBox.Show
        public MessageBoxResult? LastResult { get; protected set; }

        // Jakie przyciski ma mieć okno
        public MessageBoxButton Buttons { get; set; } = MessageBoxButton.OK;

        // Jaką ikonę ma mieć okno
        public MessageBoxImage Icon { get; set; } = MessageBoxImage.None;

        // Czy okno ma zostać wyświetlone
        public bool IsMessageDialogShow
        {
            get { return (bool)GetValue(IsMessageDialogShowProperty); }
            set { SetValue(IsMessageDialogShowProperty, value); }
        }

        // W przypadku, gdy okno nie zostanie wyświetlone
        // (Właściwość IsDialogBypassed ma wartość false),
        // to na podstawie poniższej właściowści będzie
        // ustalana odpowiedź okna dialogowego
        public MessageBoxResult DialogBypassButton { get; set; } = MessageBoxResult.None;

        // Własności dotyczące ostatniej odpowiedzi okna dialogowego
        public bool IsLastResultYes
        {
            get
            {
                if (!LastResult.HasValue)
                    return false;
                return LastResult.Value == MessageBoxResult.Yes;
            }
        }

        public bool IsLastResultNo
        {
            get
            {
                if (!LastResult.HasValue)
                    return false;
                return LastResult.Value == MessageBoxResult.No;
            }
        }

        public bool IsLastResultCancel
        {
            get
            {
                if (!LastResult.HasValue)
                    return false;
                return LastResult.Value == MessageBoxResult.Cancel;
            }
        }

        public bool IsLastResultOK
        {
            get
            {
                if (!LastResult.HasValue)
                    return false;
                return LastResult.Value == MessageBoxResult.OK;
            }
        }

        // Polecenia dla poszczególnych odpowiedzi z listy wyliczeniowej
        // MessageBoxResult

        public ICommand CommandYes
        {
            get { return (ICommand)GetValue(commandYesProperty); }
            set { SetValue(commandYesProperty, value); }
        }

        public ICommand CommandNo
        {
            get { return (ICommand)GetValue(commandNoProperty); }
            set { SetValue(commandNoProperty, value); }
        }

        public ICommand CommandCancel
        {
            get { return (ICommand)GetValue(commandCancelProperty); }
            set { SetValue(commandCancelProperty, value); }
        }

        public ICommand CommandOK
        {
            get { return (ICommand)GetValue(commandOKProperty); }
            set { SetValue(commandOKProperty, value); }
        }

        #endregion Własności publiczne

        #region Konstruktor

        public MessageDialogBox()
        {
            // Domyślnie wartość właściwości IsMessageDialogShow
            IsMessageDialogShow = true;
            // Do pola execute przypisanie funkcji showMessageBox
            execute =
                o => showMessageBox(o);
        }

        #endregion Konstruktor

        #region Metody pomocnicze

        public void showMessageBox(object contentText)
        {
            // Odpowiedź okna dialogowego
            MessageBoxResult result;

            if (IsMessageDialogShow)
            {
                // Wyświetlenie okna dialogowego i zapisanie odpowiedzi we własności LastResult
                LastResult = MessageBox.Show((string)contentText, Caption, Buttons, Icon);
                OnPropertyChanged("LastResult");
                result = LastResult.Value;
            }
            else
            {
                result = DialogBypassButton;
            }

            // W zależności od wyniku uruchamiamy odpowiednie polecenie
            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (IsMessageDialogShow)
                        OnPropertyChanged("IsLastResultYes");
                    executeCommand(CommandYes, CommandParameter);
                    break;

                case MessageBoxResult.No:
                    if (IsMessageDialogShow)
                        OnPropertyChanged("IsLastResultNo");
                    executeCommand(CommandNo, CommandParameter);
                    break;

                case MessageBoxResult.Cancel:
                    if (IsMessageDialogShow)
                        OnPropertyChanged("IsLastResultCancel");
                    executeCommand(CommandCancel, CommandParameter);
                    break;

                case MessageBoxResult.OK:
                    if (IsMessageDialogShow)
                        OnPropertyChanged("IsLastResultOK");
                    executeCommand(CommandOK, CommandParameter);
                    break;
            }
        }

        #endregion Metody pomocnicze
    }
}