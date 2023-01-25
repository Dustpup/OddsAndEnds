namespace Lottery_Game
{
    internal class Program
    {
        const short MAX_LOT_NUMBER = 49;
        const short MIN_LOT_NUMBER = 1;
        const short PLAYER_COUNT = 1;
        const short COMPUTER_COUNT = 1;
        const short MAX_NUM_DRAW_PLAYER = 7;
        const short MAX_NUM_DRAW_COMPUTER = 6;

        static byte[,] PlayerNumbers { get; set; }
        static byte[,] ComputerNumbers { get; set; }

        static void Main(string[] args)
        {
            Random rnd = new Random((int)DateTime.UtcNow.ToFileTimeUtc());
            PlayerNumbers = new byte[PLAYER_COUNT,MAX_NUM_DRAW_PLAYER];
            ComputerNumbers = new byte[COMPUTER_COUNT, MAX_NUM_DRAW_COMPUTER];

            // Player Lottery Numbers
            for (int x = 0; x < PLAYER_COUNT; x++)
                for (int y = 0; y < MAX_NUM_DRAW_PLAYER; y++)
                    PlayerNumbers[x, y] = (byte)rnd.Next(MIN_LOT_NUMBER,MAX_LOT_NUMBER);

            // Computer Lotter Numbers
            for (int x = 0; x < COMPUTER_COUNT; x++)
                for (int y = 0; y < MAX_NUM_DRAW_COMPUTER; y++)
                    ComputerNumbers[x, y] = (byte)rnd.Next(MIN_LOT_NUMBER, MAX_LOT_NUMBER);


            // SPIT OUT Player Lottery Numbers
            for (int x = 0; x < PLAYER_COUNT; x++)
            {
                Console.Write("Player [{0}]\t", x);

                for (int y = 0; y < MAX_NUM_DRAW_PLAYER; y++)
                    Console.Write(" <{0}>\t", PlayerNumbers[x, y]);

                Console.WriteLine();
            }

            // SPIT OUT Computer Lottery Numbers
            for (int x = 0; x < COMPUTER_COUNT; x++)
            {
                Console.Write("Computer [{0}]\t", x);

                for (int y = 0; y < MAX_NUM_DRAW_COMPUTER; y++)
                    Console.Write(" <{0}>\t", ComputerNumbers[x, y]);

                Console.WriteLine();
            }

            //Compair the two
            for (int pl_x = 0; pl_x < PLAYER_COUNT; pl_x++)
            {
                short matching = 0;

                for (int pl_y = 0; pl_y < MAX_NUM_DRAW_PLAYER; pl_y++)
                    for (int cpu_x = 0; cpu_x < COMPUTER_COUNT; cpu_x++)
                        for (int cpu_y = 0; cpu_y < MAX_NUM_DRAW_COMPUTER; cpu_y++)
                            if (ComputerNumbers[cpu_x, cpu_y] == PlayerNumbers[pl_x, pl_y]) matching++;

                Console.Write("Player [{0}] Has ", pl_x);
                Console.Write("{0} Matching Lottery Numbers.",matching);
            }

            Console.ReadKey();
        }
    }
}
