using Microsoft.AspNetCore.SignalR;
using LiarsBarApplication;

namespace Server.Hubs
{
    public class GameHub : Hub
    {
        private int _id = 0;
        private static readonly Dictionary<string, List<string>> GamePlayers = new();
        private static readonly Dictionary<string, Game> GameStates = new();
        private static readonly Dictionary<string, (string Game, string Username)> ConnectionMap = new();

        public async Task CreateGame(string gameName, string username)
        { 
            GamePlayers.Add(gameName, new List<string> { username });
            ConnectionMap[Context.ConnectionId] = (gameName, username);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameName);
            await Clients.Group(gameName).SendAsync("PlayerJoined", username);
        }

        public async Task JoinGame(string game, string username)
        {
            if (!GamePlayers.ContainsKey(game))
            {
                throw new HubException("This game doesn't exist!");
            }

            if (GamePlayers[game].Count >= 4)
            {
                throw new HubException("This game is full! Maximum 4 players allowed.");
            }

            if (!GamePlayers[game].Contains(username, StringComparer.OrdinalIgnoreCase))
            {
                GamePlayers[game].Add(username);
                ConnectionMap[Context.ConnectionId] = (game, username);
                await Groups.AddToGroupAsync(Context.ConnectionId, game);
                await Clients.OthersInGroup(game).SendAsync("PlayerJoined", username);
            }
        }

        public Task<List<string>> GetPlayerNames(string game)
        {
            if (!GamePlayers.ContainsKey(game))
            {
                return Task.FromResult(new List<string>());
            }
            return Task.FromResult(GamePlayers[game]);
        }

        public async Task StartGame(string game)
        {
            if (!GamePlayers.ContainsKey(game))
            {
                throw new HubException("Game not found!");
            }

            var players = GamePlayers[game];
            if (players.Count < 2)
            {
                throw new HubException("Need at least 2 players to start!");
            }

            if (!GameStates.ContainsKey(game))
            {

                var gameState = new Game(players);
                //gameState.StartGame();
                GameStates[game] = gameState;
                await Clients.Group(game).SendAsync("GameStarted");
            }
        }

        public Task<Game> GetGameState(string game)
        {
            if (!GameStates.ContainsKey(game))
            {
                return Task.FromResult<Game>(null);
            }
            return Task.FromResult(GameStates[game]);
        }

        public Task<Game> StartPlayingGame(string game)
        {
			if (!GameStates.ContainsKey(game))
			{
				return Task.FromResult<Game>(null);
			}
			GameStates[game].StartGame();
			return Task.FromResult(GameStates[game]);
		}

    }
}
