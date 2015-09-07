namespace Mentula.Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (MainGame game = new MainGame())
            {
                game.Run();
            }
        }
    }
}