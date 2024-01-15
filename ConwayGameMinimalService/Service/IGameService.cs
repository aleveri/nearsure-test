using ConwayGameMinimalService.Entities;

namespace ConwayGameMinimalService.Service
{
    public interface IGameService
    {
        Task<bool> Exist(int id);
        Task<GameOfLife> SpawnNextGeneration(GameOfLife gameOfLife);
        Task<GameOfLife> Start(GameOfLife gameOfLife);
        Task<GameOfLife> Update(GameOfLife gameOfLife);
        Task<GameOfLife> Get(int id);
        Task<List<GameOfLife>> SpawnNextGenerations(GameOfLife gameOfLife, int generations);
        Task<bool> GetFinalState(GameOfLife gameOfLife);
    }
}
