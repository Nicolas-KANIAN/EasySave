namespace CryptoSoft
{
    public static class Program
    {
        public static void Main(string[] args)
        {
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
        }
    }
}