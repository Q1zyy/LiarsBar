using System;
using System.Collections.Generic;
namespace LiarsBarApplication
{
    public class Player
    {
        private Gun _gun;
        
        public string Name { get;  set; }

        public List<Card> Cards {  get; set; } = new List<Card>();

        public bool IsPlaying { get; set; } = true;

        public Player()
        { 
            _gun = new Gun();
        }

        public List<Card> GiveCards(List<int> indexies)
        {
            List<Card> res = new List<Card>();
            foreach (int i in indexies)
            {
                res.Add(Cards[i]);
            }
            foreach (var i in res)
            {
                Cards.Remove(i);
            }
            return res;
        }

        public bool Shot()
        {
            if (_gun.Shot())
            {
                IsPlaying = false;
                return true;
            }
            return false;
        }

    }
}
