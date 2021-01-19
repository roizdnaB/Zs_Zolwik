namespace MVVM.DialogBoxes
{
    public class SaveFileDialogBox : FileDialogBox
    {
        public SaveFileDialogBox()
        {
            fileDialog = new Microsoft.Win32.SaveFileDialog();
        }
    }
}