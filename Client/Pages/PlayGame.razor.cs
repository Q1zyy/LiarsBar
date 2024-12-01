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
        private Player currentPlayer;
        private string currentUserName;
        private List<Card> playerCards;
        private CardValue selectedValue;

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

            gameState = await GameService.GetGameState(currentGame);

            gameState = await GameService.StartPlayingGame(currentGame);
        
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

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
