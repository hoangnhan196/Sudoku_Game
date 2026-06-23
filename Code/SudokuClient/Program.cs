using System;
using System.Windows.Forms;

namespace SudokuClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm()); // mở form chính
        }
    }
}