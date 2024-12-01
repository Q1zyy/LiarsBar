using LiarsBarApplication;
using System.Data.Common;

namespace Client.Services.GameService
{
    public interface IGameService
    {

        Task ConnectToHub();

        Task<bool> CreateGame(string gameName, string username);
        
        Task<bool> JoinGame(string game, string username);

        Task<List<string>> GetPlayerNames(string game);

        IDisposable CreateConnection(string method, Action<string> handler);

        Task<bool> StartGame(string game);

        Task<Game> GetGameState(string game);
        
        Task<Game> StartPlayingGame(string game);

	}
}
