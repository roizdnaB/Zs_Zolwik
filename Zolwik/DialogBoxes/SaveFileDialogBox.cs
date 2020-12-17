using System;
using System.Collections.Generic;
using System.Text;

namespace Zolwik.DialogBoxes
{
    public class SaveFileDialogBox : FileDialogBox
    {
        public SaveFileDialogBox()
        {
            fileDialog = new Microsoft.Win32.SaveFileDialog();
        }
    }
}
