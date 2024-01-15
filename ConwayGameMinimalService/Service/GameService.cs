using ConwayGameMinimalService.Data;
using ConwayGameMinimalService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConwayGameMinimalService.Service
{
    public class GameService : IGameService
    {
        private readonly ConwayGameDB _conwayGameDB;
        private readonly IConfiguration _configuration;

        public GameService(ConwayGameDB conwayGameDB, IConfiguration configuration)
        {
            _conwayGameDB = conwayGameDB;
            _configuration = configuration;
        }

        /// <summary>
        /// Validate if game exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Exist(int id)
            => await _conwayGameDB.GamesOfLife.AnyAsync(g => g.Id == id);

        /// <summary>
        /// Get game 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GameOfLife> Get(int id)
            => await _conwayGameDB.GamesOfLife.FindAsync(id);

        /// <summary>
        /// Start the game
        /// </summary>
        public async Task<GameOfLife> Start(GameOfLife gameOfLife)
        {
            SeedGame(ref gameOfLife);
            _conwayGameDB.GamesOfLife.Add(gameOfLife);
            await _conwayGameDB.SaveChangesAsync();

            return gameOfLife;
        }

        /// <summary>
        /// Update game
        /// </summary>
        public async Task<GameOfLife> Update(GameOfLife gameOfLife)
        {
            _conwayGameDB.GamesOfLife.Update(gameOfLife);
            await _conwayGameDB.SaveChangesAsync();

            return gameOfLife;
        }

        /// <summary>
        /// Seed Game with live and dead cells
        /// Whereby the live cells occupy approx. 20% of the board
        /// </summary>
        private void SeedGame(ref GameOfLife gameOfLife)
        {
            // Initiate the current and next generation boards
            gameOfLife.CurrentGeneration = new int[gameOfLife.X, gameOfLife.Y];
            gameOfLife.NextGeneration = new int[gameOfLife.X, gameOfLife.Y];

            // Cycle cells using rng to set live/dead cells
            var rng = new Random();
            for (int i = 0; i < gameOfLife.X; i++)
            {
                for (int j = 0; j < gameOfLife.Y; j++)
                {
                    // Random Board
                    if (rng.Next(1, 101) < 70)
                        gameOfLife.CurrentGeneration[i, j] = 0;
                    else
                        gameOfLife.CurrentGeneration[i, j] = 1;
                }
            }
        }

        /// <summary>
        /// Spawn the next generation
        /// </summary>
        public async Task<GameOfLife> SpawnNextGeneration(GameOfLife gameOfLife)
        {
            for (int x = 0; x < gameOfLife.X; x++)
            {
                for (int y = 0; y < gameOfLife.Y; y++)
                {
                    int liveNeighbours = CalculateLiveNeighbours(ref gameOfLife, x, y);

                    gameOfLife.NextGeneration[x, y] = gameOfLife.CurrentGeneration[x, y] switch
                    {
                        1 when liveNeighbours < 2 => 0,
                        1 when liveNeighbours > 3 => 0,
                        0 when liveNeighbours == 3 => 1,
                        _ => gameOfLife.CurrentGeneration[x, y],
                    };
                }
            }
            TransferNextGenerations(ref gameOfLife);

            _conwayGameDB.GamesOfLife.Update(gameOfLife);
            await _conwayGameDB.SaveChangesAsync();

            return gameOfLife;
        }

        /// <summary>
        /// Spawn the next generations
        /// </summary>
        public async Task<List<GameOfLife>> SpawnNextGenerations(GameOfLife gameOfLife, int generations)
        {
            int loop = 0;
            List<GameOfLife> gameStates = new();

            while (loop < generations)
            {
                for (int x = 0; x < gameOfLife.X; x++)
                {
                    for (int y = 0; y < gameOfLife.Y; y++)
                    {
                        int liveNeighbours = CalculateLiveNeighbours(ref gameOfLife, x, y);

                        gameOfLife.NextGeneration[x, y] = gameOfLife.CurrentGeneration[x, y] switch
                        {
                            1 when liveNeighbours < 2 => 0,
                            1 when liveNeighbours > 3 => 0,
                            0 when liveNeighbours == 3 => 1,
                            _ => gameOfLife.CurrentGeneration[x, y],
                        };
                    }
                }
                loop++;
                TransferNextGenerations(ref gameOfLife);
                gameStates.Add(gameOfLife);
            }

            _conwayGameDB.GamesOfLife.Update(gameOfLife);
            await _conwayGameDB.SaveChangesAsync();

            return gameStates;
        }


        /// <summary>
        /// Get final state
        /// </summary>
        public async Task<bool> GetFinalState(GameOfLife gameOfLife)
        {
            int loop = 0;
            _ = int.TryParse(_configuration["Settings:Attempt"], out int attempt);

            while (loop < attempt)
            {
                for (int x = 0; x < gameOfLife.X; x++)
                {
                    for (int y = 0; y < gameOfLife.Y; y++)
                    {
                        int liveNeighbours = CalculateLiveNeighbours(ref gameOfLife, x, y);

                        gameOfLife.NextGeneration[x, y] = gameOfLife.CurrentGeneration[x, y] switch
                        {
                            1 when liveNeighbours < 2 => 0,
                            1 when liveNeighbours > 3 => 0,
                            0 when liveNeighbours == 3 => 1,
                            _ => gameOfLife.CurrentGeneration[x, y],
                        };
                    }
                }
                loop++;
                TransferNextGenerations(ref gameOfLife);
            }

            _conwayGameDB.GamesOfLife.Update(gameOfLife);
            await _conwayGameDB.SaveChangesAsync();

            return false;
        }

        /// <summary>
        /// Given any cell - calculate live neighbours
        /// </summary>
        /// <param name="x">X coord of Cell</param>
        /// <param name="y">Y coord of Cell</param>
        /// <returns></returns>
        private int CalculateLiveNeighbours(ref GameOfLife gameOfLife, int x, int y)
        {
            // Calculate live neighours
            int liveNeighbours = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (x + i < 0 || x + i >= gameOfLife.X)   // Out of bounds
                        continue;
                    if (y + j < 0 || y + j >= gameOfLife.Y)   // Out of bounds
                        continue;
                    if (x + i == x && y + j == y)       // Same Cell
                        continue;

                    // Add cells value to current live neighbour count
                    liveNeighbours += gameOfLife.CurrentGeneration[x + i, y + j];
                }
            }

            return liveNeighbours;
        }

        /// <summary>
        /// Transfer next generation to current generation 
        /// </summary>
        private void TransferNextGenerations(ref GameOfLife gameOfLife)
        {
            for (int i = 0; i < gameOfLife.X; i++)
            {
                for (int j = 0; j < gameOfLife.Y; j++)
                    gameOfLife.CurrentGeneration[i, j] = gameOfLife.NextGeneration[i, j];
            }
        }
    }
}
