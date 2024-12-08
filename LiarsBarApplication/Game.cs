using System.Text.Json.Serialization;

namespace LiarsBarApplication
{
    public class Game
    {
        // Поля, которые не обязательно сериализовать
        private int _playersCount;
        private List<Card> _allCards;

        // Свойства для сериализации
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Card> LastCards { get; set; } = new List<Card>();
        public int CurrentPlayer { get; set; } = 0;

        public int Steps { get; set; } = 0;

        public CardValue SelectedValue { get; set; }
        public int ActivePlayers { get; set; }
        public bool IsStarted { get; set; } = false;

        public bool IsEnded { get; set; } = false;

        public string Winner { get; set; }

        public string Logs { get; set; }

        // Конструктор без параметров
        public Game() { }

        // Основной конструктор
        public Game(List<string> players)
        {
            InitializeGame(players);
        }

        private void InitializeGame(List<string> players)
        {
            _playersCount = players.Count;
            ActivePlayers = players.Count;
            _allCards = new List<Card>();
            LastCards = new List<Card>();
            IsStarted = false;

            for (int i = 0; i < 6; i++)
            {
                _allCards.Add(new Card { Value = CardValue.ACE });
                _allCards.Add(new Card { Value = CardValue.KING });
                _allCards.Add(new Card { Value = CardValue.QUEEN });
                if (i < 2)
                {
                    _allCards.Add(new Card { Value = CardValue.JOKER });
                }
            }
            foreach (var player in players)
            {
                Players.Add(new Player { Name = player });
            }
            ShuffleCards();
        }

        private void ShuffleCards()
        {
            Random random = new Random();
            var arr = _allCards.ToArray();
            random.Shuffle(arr);
            _allCards = new List<Card>(arr);
			for (int i = 0; i < Players.Count; i++)
			{
                Players[i].Cards = new List<Card>();
				for (int j = i * 5; j < i * 5 + 5; j++)
				{
					Players[i].Cards.Add(_allCards[j]);
				}
			}
		}

        public void StartGame()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                Random random = new Random();
                byte cur = (byte)random.Next(1, 4);
                SelectedValue = (CardValue)Enum.ToObject(typeof(CardValue), cur);

                //for (int i = 0; i < Players.Count; i++)
                //{
                //    for (int j = i * 5; j < i * 5 + 5; j++)
                //    {
                //        Players[i].Cards.Add(_allCards[j]);
                //    }
                //}
            }
        }

        private int NextPlayer()
        {
            int tmp = (CurrentPlayer + 1) % Players.Count;
            while (!Players[tmp].IsPlaying)
            {
                tmp = (tmp + 1) % Players.Count;
            }
            return tmp;
        }

        private int PreviousPlayer()
        {
            int tmp = (CurrentPlayer - 1 + Players.Count) % Players.Count;
            while (!Players[tmp].IsPlaying)
            {
                tmp = (tmp - 1 + Players.Count) % Players.Count;
            }
            return tmp;
        }

        public void GiveCards(List<int> indexes)
        {
            Logs = $"Player {Players[CurrentPlayer].Name} put {indexes.Count} {SelectedValue}\n";
            Steps++;
            LastCards = Players[CurrentPlayer].GiveCards(indexes);
            CurrentPlayer = NextPlayer();
        }

        public void Lie()
        {
            int size = LastCards.Count;
            int cnt = 0;
            Logs = "";
            foreach (var card in LastCards)
            {
                Logs += card.Value.ToString();
                Logs += " ";
                if (card.Value == SelectedValue || card.Value == CardValue.JOKER)
                {
                    cnt++;
                }
            }
            if (cnt == size)
            {
                bool p = Players[CurrentPlayer].Shot();
                Logs += $"Player {Players[CurrentPlayer].Name} take gun.......";
                Logs += (p ? "Killed yourself" : "Lucky bro");
            }
            else
            {
                bool p = Players[PreviousPlayer()].Shot();
				Logs += $"Player {Players[PreviousPlayer()].Name} take gun.......";
				Logs += (p ? "Killed yourself" : "Lucky bro");
			}
            ShuffleCards();
            Steps = 0;
            CurrentPlayer = NextPlayer();
            int alive = 0;
            string name = "";
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].IsPlaying)
                {
                    alive++;
                    name = Players[i].Name;
                }
            }
            if (alive == 1)
            {
                IsEnded = true;
                Winner = name;
            }
        }
        public Player GetPlayerTurn()
        {
            return Players[CurrentPlayer];
        }
    }

}
