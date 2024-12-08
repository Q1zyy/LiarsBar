using LiarsBarApplication;
using System.Data.Common;

namespace Client.Services.GameService
{
    public interface IGameService
    {

        Task ConnectToHub();

        Task<bool> CreateGame(string gameName, string username);
        
        Task<bool> JoinGame(string game, string username);

		Task DeleteGame(string game);

		Task<List<string>> GetPlayerNames(string game);

		IDisposable CreateConnection(string method, Action handler);

		IDisposable CreateConnection(string method, Action<string> handler);

        IDisposable CreateConnection(string method, Action<Game> handler);

        Task<bool> StartGame(string game);

        Task<Game> GetGameState(string game);
        
        Task<Game> StartPlayingGame(string game);

        Task<Game> MakeStep(string game, List<int> indexies);

        Task<Game> Lie(string game);

	}
}
