using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Game
    {
        /* Game class hold the state of the game
        */
        private int gameID;
        private bool plus2Active;
        private Dictionary<string, int> playersCards; // A dict where the key is the player name and the value is their number of cards
        private List<Card> myCards; // Card is a dict where the key/value is a string
        private Card currentPlacedCard; // Current card on the pile
        
        public Game()
        {
            this.plus2Active = false;
            this.playersCards = new Dictionary<string, int>();
            this.myCards = new List<Card>();
        }


        public Card GetPlacedCard()
        {
            return this.currentPlacedCard;
        }

        public void UpdatePlacedCard(Card card)
        {
            this.currentPlacedCard = card;
        }

        public List<Card> GetMyCards()
        {
            return this.myCards;
        }

        public void AddMyCards(List<Card> cards)
        {
            this.myCards.AddRange(cards);
        }

        public void RemoveMyCard(int index)
        {
            this.myCards.RemoveAt(index);
        }

        public void AddPlayerCards (string playerName, int numOfCards)
        {
            this.playersCards[playerName] += numOfCards;
        }

        public void RemovePlayerCards(string playerName, int numOfCards)
        {
            this.playersCards[playerName] -= numOfCards;
        }

    }
}
