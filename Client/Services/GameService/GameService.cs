
using LiarsBarApplication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Data.Common;

namespace Client.Services.GameService
{
    public class GameService : IGameService
    { 

        private readonly HubConnection _connection;

        public GameService()
        {
            _connection = new HubConnectionBuilder()
                        .WithUrl("https://localhost:7011/gamehub")
                        .Build();
        }

        public async Task ConnectToHub()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async Task<bool> CreateGame(string gameName, string username)
        {
            try
            {
                await _connection.InvokeAsync("CreateGame", gameName, username);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> JoinGame(string game, string username)
        {
            try
            {
                await _connection.InvokeAsync("JoinGame", game, username);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> GetPlayerNames(string game)
        {
            try
            {
                return await _connection.InvokeAsync<List<string>>("GetPlayerNames", game);
            }
            catch
            {
                return new List<string>();
            }
        }

        public IDisposable CreateConnection(string method, Action<string> handler)
        {
            return _connection.On(method, handler);
        }
        
        public IDisposable CreateConnection(string method, Action<Game> handler)
        {
            return _connection.On(method, handler);
        }
		public IDisposable CreateConnection(string method, Action handler)
		{
			return _connection.On(method, handler);
		}

        public async Task<bool> StartGame(string game)
        {
            try
            {
                await _connection.InvokeAsync("StartGame", game);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Game> GetGameState(string game)
        {
            try
            {
                return await _connection.InvokeAsync<Game>("GetGameState", game);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<Game> StartPlayingGame(string game)
        {
			try
			{
				return await _connection.InvokeAsync<Game>("StartPlayingGame", game);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

		public async Task<Game> MakeStep(string game, List<int> indexies)
		{
			try
			{
				return await _connection.InvokeAsync<Game>("MakeStep", game, indexies);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

		public async Task<Game> Lie(string game)
		{
			try
			{
				return await _connection.InvokeAsync<Game>("Lie", game);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}


		public async Task DeleteGame(string game)
		{
			try
			{
				await _connection.InvokeAsync("DeleteGame", game);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error deleting game: {ex.Message}");
			}
		}
	}
}
