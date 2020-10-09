using System;
using System.Collections.Generic;

namespace PlayingAlgorithm
{
    public class TakiColor
    {
        public TakiColor(Color _color)
        {
            myColor = _color;
        }
        public enum Color
        {
            red,
            green,
            blue,
            yellow,
        }
        Color myColor;

        public override bool Equals(object obj)
        {
            if( obj is TakiColor)
            {
                return ((TakiColor)obj).myColor == myColor;
            }
            return false;
        }
        public override string ToString()
        {
            switch (myColor)
            {
                case Color.red:
                    return "red";
                case Color.blue:
                    return "blue";
                case Color.green:
                    return "green";
                case Color.yellow:
                    return "yellow";
            }
            return " ";
        }

        public override int GetHashCode()
        {
            return 542640994 + myColor.GetHashCode();
        }
    }

    public class TakiFace
    {
        int num;
        public override string ToString()
        {
            return num.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj is TakiFace)
            {
                return ((TakiFace)obj).num == num;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return -992186197 + num.GetHashCode();
        }
    }


    public class TakiCard
    {
        public TakiCard(TakiCardType _type)
        {
            this.type = _type;
        }

        public enum TakiCardType
        {
            numberCard,
            specialCard
        }
        
        public TakiCardType type { get; private set; }

        public override bool Equals(object obj)
        {
            var card = obj as TakiCard;
            return card != null &&
                   type == card.type;
        }

        public override int GetHashCode()
        {
            return 34944597 + type.GetHashCode();
        }
    }

    public class NumericTakiCard: TakiCard
    {
        public TakiFace face { get; private set; }
        public TakiColor color { get; private set; }
        public NumericTakiCard(TakiFace _face, TakiColor _color) : 
            base(TakiCardType.numberCard)
        {
            face = _face;
            color = _color;
        }

        public override bool Equals(object obj)
        {
            var card = obj as NumericTakiCard;
            return card != null &&
                   base.Equals(obj) &&
                   face.Equals( card.face) &&
                   color.Equals( card.color );
        }

        public override int GetHashCode()
        {
            var hashCode = 1317838996;
            hashCode = hashCode * -1521134295 + EqualityComparer<TakiFace>.Default.GetHashCode(face);
            hashCode = hashCode * -1521134295 + EqualityComparer<TakiColor>.Default.GetHashCode(color);
            return hashCode;
        }

        public override string ToString()
        {
            return face.ToString() + color.ToString();
        }

    }

    public class SpecialTakiCard:TakiCard
    {
        public SpecialTakiCard(SpecialTakiCardType specialType) :base(TakiCardType.specialCard)
        {
            this.specialType = specialType;
        }

        public class SpecialTakiCardType
        {
            public int type { get; private set; }
            public const int stop = 1 + 32;
            public const int plus2 = 2 + 32;
            public const int switchDirection = 3 + 32;
            public const int switchColor = 4;
            public const int taki = 5 + 32;
            public const int superTaki = 6;
            public const int plus = 7 + 32;
            //plus3,
            //crazyTaki,
            //breakPlus3,
            //kingCard,
            //notYetInventedCard

            public SpecialTakiCardType(int type)
            {
                this.type = type;
            }
            public bool HasColor()
            {
                return this.type >= 32;
            }
        }
        public SpecialTakiCardType specialType { get; private set; }
        public override string ToString()
        {
            return this.specialType.ToString();
        }

        public override bool Equals(object obj)
        {
            var card = obj as SpecialTakiCard;
            return card != null &&
                   base.Equals(obj) &&
                   specialType == card.specialType;
        }

        public override int GetHashCode()
        {
            return 462357464 + specialType.GetHashCode();
        }
    }
    public class ColoredSpecialTakiCard : SpecialTakiCard
    {
        public ColoredSpecialTakiCard(SpecialTakiCardType specialType, TakiColor color):
            base(specialType)
        {
            this.color = color;
        }
        public TakiColor color { get; private set; }

        public override bool Equals(object obj)
        {
            var card = obj as ColoredSpecialTakiCard;
            return card != null &&
                   base.Equals(obj) &&
                   color.Equals( card.color );
        }

        public override int GetHashCode()
        {
            var hashCode = 1655715054;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TakiColor>.Default.GetHashCode(color);
            return hashCode;
        }

        public override string ToString()
        {
            return this.specialType.ToString() +"_"+ this.color.ToString();
        }

    }

    public class MyRandom
    {
        /**
         * This is proxy class initially that we can replace fo speed
         */
        static Random rnd = new Random();
        static public int randomUniform( int max)
        {
            return rnd.Next(max);
        }
    }

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
            for( int k=0; k< numCards; k++)
            {
                cards[k] = old_cards[k];
            }
            cardsSize = newSize;
        }
        public void AddCard(TakiCard card)
        {
            _allocateCards(cardsSize + 1);
            cards[cardsSize] = card;
            cardsSize++;
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
        public int FindCard(TakiCard card)
        {
            int cardIndex = -1;
            for(int k=0; k< numCards; k++)
            {
                if( cards[k].Equals( card) )
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
        public bool IsEmpty()
        {
            return numCards == 0;
        }
    }

    public class TakiCardRestriction
    {
        /* This is a base class to make restriction for taki collections*/
       int minValue, maxValue; // number of cards a given color collection can have

        public TakiCardRestriction(int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }



    public class TakiCardRestriction_Color: TakiCardRestriction
    {
        TakiColor color;

        public TakiCardRestriction_Color(TakiColor color, int minValue, int maxValue ):
            base(minValue, maxValue )
        {
            this.color = color;
        }
    }
    public class TakiCardRestriction_Face : TakiCardRestriction
    {
        TakiFace face;

        public TakiCardRestriction_Face(TakiFace face, int minValue, int maxValue) :
            base(minValue, maxValue)
        {
            this.face = face;
        }
    }

    public class TakiTable
    {
        TakiCardCollection discardPile, drawPile;
        TakiCard leadingCard;
        TakiColor actionColor;
        bool takiAction, specialAction;
        bool direction = true; // forward
        int plus2amount=0; // should be in construction and in the beggining of game
        Player[] players;

        /** 
         * play one turn of the game return if game is finished
         */
        public bool PlayTurn()
        {
            TakiCard move;
            specialAction = false;
            bool stopAction = false;
            int i = (direction) ? 0 : players.Length-1;
            while( true )
            {
                while (true)
                {
                    if (stopAction)
                    {
                        stopAction = false;                 
                        break;
                    }
                    move = players[i].PlayCard(this);
                    specialAction = false;
                    if (move == null)
                    {
                        if (plus2amount > 0)
                        {
                            plus2amount--;
                        }
                        while (plus2amount > 0)
                        {
                            plus2amount--;
                            players[i].DrawOneCard(drawPile.cards[0]);
                            drawPile.RemoveCard(0);
                            if (drawPile.IsEmpty())
                            {
                                TakiCardCollection tmp = discardPile;
                                discardPile = drawPile;
                                drawPile = discardPile;
                                drawPile.Shuffle();
                            }
                        }
                        players[i].DrawOneCard(drawPile.cards[0]);
                        drawPile.RemoveCard(0);
                        if (drawPile.IsEmpty())
                        {
                            TakiCardCollection tmp = discardPile;
                            discardPile = drawPile;
                            drawPile = discardPile;
                            drawPile.Shuffle();
                        }
                    }
                    else
                    {
                        if (takiAction)
                        {
                            if (players[i].ShouldFinishTakiAction())
                            {
                                takiAction = false;
                            }
                        }
                        leadingCard = move;
                        discardPile.AddCard(move);
                        if (move.type == TakiCard.TakiCardType.specialCard)
                        {
                            switch (((SpecialTakiCard)move).specialType.type)
                            {
                                case SpecialTakiCard.SpecialTakiCardType.plus:
                                    actionColor = ((ColoredSpecialTakiCard)move).color;
                                    continue;
                                case SpecialTakiCard.SpecialTakiCardType.plus2:
                                    specialAction = true;
                                    plus2amount += 2;
                                    actionColor = ((ColoredSpecialTakiCard)move).color;
                                    break;
                                case SpecialTakiCard.SpecialTakiCardType.stop:
                                    stopAction = true;
                                    actionColor = ((ColoredSpecialTakiCard)move).color;
                                    break;
                                case SpecialTakiCard.SpecialTakiCardType.superTaki:
                                    specialAction = true;
                                    takiAction = true;                                  
                                    continue;
                                case SpecialTakiCard.SpecialTakiCardType.switchColor:
                                    actionColor = players[i].SwitchColorTo();
                                    break;
                                case SpecialTakiCard.SpecialTakiCardType.switchDirection:
                                    actionColor = ((ColoredSpecialTakiCard)move).color;
                                    direction = !direction;
                                    break;
                                case SpecialTakiCard.SpecialTakiCardType.taki:
                                    specialAction = true;
                                    takiAction = true;
                                    continue;
                            }
                        }
                        else
                        {
                            actionColor = ((NumericTakiCard)move).color;
                        }
                    }
                    if (takiAction)
                    {
                        continue;
                    }
                    break;
                }
                //change player according to direction
                if (direction)
                {// forward direction
                    i++;
                    if (i == players.Length)
                    {
                       
                        break;
                    }
                } else
                {// backward direction
                    if (i == 0)
                    {
                        break;
                    }
                    i--;
                }
            }
            return true;
        }

        public bool CanPlay(TakiCard card, bool specialAction= false)
        {
            if (plus2amount > 0)
            {
                if(card.type == TakiCard.TakiCardType.specialCard)
                {
                    return ((SpecialTakiCard)card).specialType.type == SpecialTakiCard.SpecialTakiCardType.plus2;
                }
                else
                {
                    return false;
                }
            }

            if (card.type == TakiCard.TakiCardType.specialCard) {
                if( ((SpecialTakiCard)card).specialType.HasColor())
                {
                    return ((ColoredSpecialTakiCard)card).color.Equals(actionColor);
                }
                {
                    return true;
                }
            }
            return ((NumericTakiCard)card).color.Equals(actionColor);
        }
    }

    public class Player
    {
        TakiCardCollection hand;
        public Player()
        {
            hand = new TakiCardCollection();
        }
        public void DrawOneCard(TakiCard card)
        {
            hand.AddCard(card);
        }
        public TakiColor SwitchColorTo()
        {
            // TODO: implement
            return null;
        }

        /**
             * this function is called by TakiTable when it is a players turn
             * return single card played by Player or null if he can't.
             */
        public TakiCard PlayCard(TakiTable table)            
        {
            for (int cardIndex=0; cardIndex < hand.cards.Length; cardIndex++)
            {
                if 
                    (table.CanPlay(hand.cards[cardIndex]))
                {
                    return hand.cards[cardIndex];
                }
            }
            return null;
        }
        public bool ShouldFinishTakiAction()
        {
            //TODO: implement
            return false;
        }
    }
}


