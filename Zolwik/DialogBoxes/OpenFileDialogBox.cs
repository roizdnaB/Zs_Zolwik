using System;
using System.Collections.Generic;
using System.Text;

namespace Zolwik.DialogBoxes
{
    public class OpenFileDialogBox : FileDialogBox
    {
        public OpenFileDialogBox()
        {
            fileDialog = new Microsoft.Win32.OpenFileDialog();
        }
    }
}
