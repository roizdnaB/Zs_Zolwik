namespace MVVM.DialogBoxes
{
    public class OpenFileDialogBox : FileDialogBox
    {
        public OpenFileDialogBox()
        {
            fileDialog = new Microsoft.Win32.OpenFileDialog();
        }
    }
}