using System;
using System.Windows.Forms;

namespace SudokuServer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SudokuServer()); // mở form chính
        }
    }
}