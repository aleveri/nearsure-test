namespace ConwayGameMinimalService.Entities
{
    public class GameOfLife
    {
        public int Id { get; set; }
        public int X { get; }
        public int Y { get; }
        public int[,] CurrentGeneration { get; set; }
        public int[,] NextGeneration { get; set; }

        /// <summary>
        /// Accepts a custom board size or by default sets the board size to 16x16
        /// </summary>
        /// <param name="id"></param>
        public GameOfLife(int id, int x = 16, int y = 16)
        {
            Id = id;
            X = x;
            Y = y;
            CurrentGeneration = new int[X, Y];
            NextGeneration = new int[X, Y];
        }
    }
}
