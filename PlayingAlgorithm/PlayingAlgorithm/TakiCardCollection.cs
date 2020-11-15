namespace PlayingAlgorithm
{
    public class TakiCardCollection
    {
        const int maxCardSize = 256; // put real number 
        public int numCards { get;private set; }
        public TakiCard[] cards;
        int cardsSize;

        public TakiCardCollection(int _numCards=0)
        {
            numCards = _numCards;
            if (numCards > 0)
            {
                _allocateCards(_numCards);                
            }
        }
        private void _allocateCards(int _numCards)
        {
            if ( _numCards <= cardsSize)
            {
                return;
            }
            int newSize = (_numCards % maxCardSize == 0) ? _numCards : maxCardSize * (_numCards / maxCardSize + 1);
            TakiCard[] old_cards = cards;
            cards = new TakiCard[newSize];
            if (old_cards != null)
            {
                for (int k = 0; k < numCards; k++)
                {
                    cards[k] = old_cards[k];
                }
            }
            cardsSize = newSize;
        }
        public void  RemoveCardsIn(TakiCardCollection _cards)
        {            
            for( int k=0; k< _cards.numCards; k++)
            {
                int idx = FindCard(_cards.cards[k]);
                if( idx >= 0)
                {
                    RemoveCard(idx);
                }
            }
        }
        public void CopyCardsFrom(TakiCardCollection _cards)
        {
            if (cardsSize <= _cards.numCards)
            {
                cardsSize = (_cards.numCards % maxCardSize == 0) ? _cards.numCards : maxCardSize * (_cards.numCards / maxCardSize + 1);           
                cards = new TakiCard[cardsSize];
            }
            
            for (int k = 0; k < _cards.numCards; k++)
            {
                cards[k] = _cards.cards[k];
            }
            numCards= _cards.numCards;
        }
        public void AddCard(TakiCard card)
        {
            _allocateCards(numCards + 1);
            cards[numCards] = card;
            numCards++;
        }
        public void PutAtRandom(TakiCard card)
        {
            int ind = MyRandom.randomUniform(numCards);
            AddCard(cards[ind]);
            cards[ind] = card;
        }
        public void RemoveCard(int cardIndex)
            // I do believe (for speed) that index is within cards
        {
            //  XXXXAXXXY 
            //       <--+
            numCards--;
            if ( cardIndex < numCards)
            {
                cards[cardIndex] = cards[numCards];
            }
        }
        public TakiCard RandomCard()
        {
            if (numCards == 0)
            {
                return null;
            }
            return cards[MyRandom.randomUniform(numCards)];
        }
        public int FindCard(TakiCard card)
        {
            int cardIndex = -1;
            for(int k=0; k< numCards; k++)
            {
                if( cards[k].SameCard( card) )
                {
                    cardIndex = k;
                    break;
                }
            }
            return cardIndex;
        }
        public void Shuffle()
        {
            for(int k=numCards-1; k>0; k--)
            {
                int ii = MyRandom.randomUniform(k+1);
                TakiCard tmp = cards[k];
                cards[k] = cards[ii];
                cards[ii] = tmp;
            }
        }
        public TakiCard Pop()
        {
            if (numCards == 0)
            {
                return null;
            }
            numCards--;
            return cards[numCards];
        } 
        public bool IsEmpty()
        {
            return numCards == 0;
        }
        public void Empty()
        {
            if( cards == null)
            {
                _allocateCards(1);
            }
            numCards = 0;
        }
        public void Swap(int k, int m)
        {
            TakiCard tmp = cards[m];
            cards[m] = cards[k];
            cards[k] = tmp;
        }
        public override string ToString()
        {
            string cardsStr = "";
            for (int i = 0; i < numCards; i++)
            {
                cardsStr += cards[i].ToString() + "\n";
            }
            return cardsStr;
        }
    }
}


