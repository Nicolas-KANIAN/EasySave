namespace CryptoSoft
{
    public static class Program
    {
        private static Mutex _mutex = new Mutex(false, "Global\\CryptoSoft_SingleInstance_Mutex");

        public static void Main(string[] args)
        {
            _mutex.WaitOne();

            try
            {
                if (args.Length < 2) Environment.Exit(-1);

                var fileManager = new FileManager(args[0], args[1]);
                int elapsedTime = fileManager.TransformFile();

                Environment.Exit(elapsedTime);
            }
            catch
            {
                Environment.Exit(-99);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }
    }
}