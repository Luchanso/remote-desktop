using System;

namespace RS
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Receive game = new Receive())
            {
                game.Run();
            }
        }
    }
#endif
}

