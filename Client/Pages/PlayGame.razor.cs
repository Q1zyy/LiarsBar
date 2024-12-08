using Client.Services.GameService;
using LiarsBarApplication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Client.Pages
{
    public partial class PlayGame : IDisposable
    {

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IGameService GameService { get; set; }

        private string currentGame;
        private Game gameState;
        private Player currentStepPlayer;
        private Player currentPlayer;
        private string currentUserName;
        private List<Card> playerCards;
        private CardValue selectedValue;
        private int Steps;
        private string Logs;
        private bool[] isTaken = new bool[5] { false, false, false, false, false };
        private IDisposable gameStateUpdatedSubscription;
		private IDisposable gameEndedSubscription;
        private bool isGameOver = false;
        private string gameMessage;
        

		protected override async Task OnInitializedAsync()
        {

            var uri = new Uri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("game", out var gameParam))
            {
                currentGame = gameParam;
            }

            if (queryParams.TryGetValue("player", out var playerParam))
            {
                currentUserName = playerParam;
                Console.WriteLine($"[PlayPig] Initialized with user: {currentUserName}");
            }


            await GameService.ConnectToHub();

            gameStateUpdatedSubscription = GameService.CreateConnection("GameStateUpdated", async (Game newGameState) => {
                await InvokeAsync(async () => await UpdateGameState(newGameState));
            });

			gameEndedSubscription = GameService.CreateConnection("GameEnded", async (string winner) =>
			{
				gameMessage = $"Game Over! {winner} wins!";
				isGameOver = true;
                await GameService.DeleteGame(currentGame);
				await InvokeAsync(StateHasChanged);
				await Task.Delay(2000);
				NavigationManager.NavigateTo($"/results?winner={winner}");
			});


			gameState = await GameService.GetGameState(currentGame);

            gameState = await GameService.StartPlayingGame(currentGame);

            currentStepPlayer = gameState.GetPlayerTurn();
        
            foreach (var player in gameState.Players)
            {
                if (player.Name.Equals(currentUserName, StringComparison.OrdinalIgnoreCase))
                {
                    currentPlayer = player;
                    playerCards = player.Cards;
                }
            }

            selectedValue = gameState.SelectedValue;
        }

        public async Task MakeStep()
        {
            List<int> indexies = new();
            for (int i = 0; i < 5; i++)
            {
                if (isTaken[i])
                {
                    indexies.Add(i);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                isTaken[i] = false;
            }
            gameState = await GameService.MakeStep(currentGame, indexies);
            //currentStepPlayer = gameState.GetPlayerTurn();
            //UpdateGame();
            //StateHasChanged();
        }     
        
        public async Task Lie()
        {
            gameState = await GameService.Lie(currentGame);
        }

        public async Task PickCard(ChangeEventArgs e, int value)
        {
            isTaken[value] = !isTaken[value];
        }

        public async Task UpdateGameState(Game newState)
        {
            gameState = newState;
			currentStepPlayer = gameState.GetPlayerTurn();
			Steps = gameState.Steps;
            Logs = gameState.Logs;
            foreach (var player in gameState.Players)
			{
				if (player.Name.Equals(currentUserName, StringComparison.OrdinalIgnoreCase))
				{
					currentPlayer = player;
					playerCards = player.Cards;
				}
			}
            StateHasChanged();
		}

        public void Dispose()
        {
			gameStateUpdatedSubscription?.Dispose();
		}
	}
}
