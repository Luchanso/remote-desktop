using System;

namespace RS_Clinet
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Sender sender = new Sender();
            sender.Run();
        }
    }
}
