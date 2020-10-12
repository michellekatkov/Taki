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

        public static TakiColor red = new TakiColor(TakiColor.Color.red);
        public static TakiColor green = new TakiColor(TakiColor.Color.green);
        public static TakiColor blue = new TakiColor(TakiColor.Color.blue);
        public static TakiColor yellow = new TakiColor(Color.yellow);

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
        public void CopyColorFrom(TakiColor color)
        {
            myColor = color.myColor;
        }
    }

    public class TakiFace
    {
        int num;
        public TakiFace(int num)
        {
            this.num = num;
        }
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
        public virtual void CopyCardFrom(TakiCard card)
        {
            type = card.type;
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

        public static SpecialTakiCardType stop = new SpecialTakiCardType(SpecialTakiCardType.stop);
        public static SpecialTakiCardType plus2 = new SpecialTakiCardType(SpecialTakiCardType.plus2);
        public static SpecialTakiCardType switchDirection = new SpecialTakiCardType(SpecialTakiCardType.switchDirection);
        public static SpecialTakiCardType switchColor = new SpecialTakiCardType(SpecialTakiCardType.switchColor);
        public static SpecialTakiCardType taki = new SpecialTakiCardType(SpecialTakiCardType.taki);
        public static SpecialTakiCardType superTaki = new SpecialTakiCardType(SpecialTakiCardType.superTaki);
        public static SpecialTakiCardType plus = new SpecialTakiCardType(SpecialTakiCardType.plus);

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
        public void CopyCardsFrom(TakiCardCollection _cards)
        {
            if (cardsSize <= _cards.numCards)
            {
                cardsSize = (_cards.numCards % maxCardSize == 0) ? _cards.numCards : maxCardSize * (_cards.numCards / maxCardSize + 1);           
                cards = new TakiCard[cardsSize];
            }
            
            for (int k = 0; k < numCards; k++)
            {
                cards[k] = _cards.cards[k];
            }
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
        public TakiCard RandomCard()
        {
            return cards[MyRandom.randomUniform(numCards)];
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
        public TakiCardCollection discardPile { get; private set; }
        public TakiCardCollection drawPile{ get; private set; }
        TakiCard leadingCard;
        TakiColor actionColor;
        bool takiAction, specialAction;
        bool direction = true; // forward
        int plus2amount=0; // should be in construction and in the beggining of game
        public Player[] players;
        int numPlayers, addedPlayers;

        public TakiTable(int numPlayers)
        {
            this.numPlayers = numPlayers;
            addedPlayers = 0;
            players = new Player[numPlayers];
        }

        /** 
         * play one turn of the game return if game is finished
         */
        public static TakiCardCollection takiDeck = GenerateTakiDeck();
        public static TakiCardCollection GenerateTakiDeck()
        {
            //change to real num
            int numRepetitions = 2;
            int specialRepetitions = 4;

            TakiCardCollection takiDeck = new TakiCardCollection();
            // generate numericCards and coloredSpecialCards
            NumericTakiCard numCard;
            SpecialTakiCard specialCard;
            for (int i = 0; i < numRepetitions; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    TakiFace face = new TakiFace(j);
                    numCard = new NumericTakiCard(face, TakiColor.blue);
                    takiDeck.AddCard(numCard);
                    numCard = new NumericTakiCard(face, TakiColor.red);
                    takiDeck.AddCard(numCard);
                    numCard = new NumericTakiCard(face, TakiColor.green);
                    takiDeck.AddCard(numCard);
                    numCard = new NumericTakiCard(face, TakiColor.yellow);
                    takiDeck.AddCard(numCard);
                }
                AddColorsFor(SpecialTakiCard.stop, takiDeck);
                AddColorsFor(SpecialTakiCard.taki, takiDeck);
                AddColorsFor(SpecialTakiCard.plus, takiDeck);
                AddColorsFor(SpecialTakiCard.plus2, takiDeck);
                AddColorsFor(SpecialTakiCard.switchDirection, takiDeck);
                specialCard = new SpecialTakiCard(SpecialTakiCard.superTaki);
                takiDeck.AddCard(specialCard);
            }
            // generate 4x switch color
            for (int k = 0; k < specialRepetitions; k++)
            {
                specialCard = new SpecialTakiCard(SpecialTakiCard.switchColor);
                takiDeck.AddCard(specialCard);
            }
            return takiDeck;
        }
        private static void AddColorsFor(SpecialTakiCard.SpecialTakiCardType type, TakiCardCollection takiDeck)
        {
            ColoredSpecialTakiCard coloredSpecialCard;
            coloredSpecialCard = new ColoredSpecialTakiCard(type, TakiColor.blue);
            takiDeck.AddCard(coloredSpecialCard);
            coloredSpecialCard = new ColoredSpecialTakiCard(type, TakiColor.red);
            takiDeck.AddCard(coloredSpecialCard);
            coloredSpecialCard = new ColoredSpecialTakiCard(type, TakiColor.green);
            takiDeck.AddCard(coloredSpecialCard);
            coloredSpecialCard = new ColoredSpecialTakiCard(type, TakiColor.yellow);
            takiDeck.AddCard(coloredSpecialCard);
        }
        public TakiCard DrawOneCard()
        {
            TakiCard card = drawPile.Pop();
            if (drawPile.IsEmpty())
            {
                TakiCardCollection tmp = discardPile;
                discardPile = drawPile;
                drawPile = discardPile;
                drawPile.Shuffle();
            }
            return card;
        }
        public void CopyTableFrom(TakiTable table)
        {
            discardPile.CopyCardsFrom(table.discardPile);
            drawPile.CopyCardsFrom(table.drawPile);
            leadingCard.CopyCardFrom(table.leadingCard);
            actionColor.CopyColorFrom(table.actionColor);
            takiAction = table.takiAction;
            specialAction = table.specialAction;
            direction = table.direction;
            plus2amount = table.plus2amount;
            numPlayers = table.numPlayers;
            for (int i = 0; i < numPlayers; i++)
            {
                players[i].CopyPlayerFrom(table.players[i]);
            }
        }

        public void AddPlayer(Player player)
        {
            players[addedPlayers] = player;
            addedPlayers++;
        }
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
                           
                            players[i].DrawOneCard(DrawOneCard());                           
                        }
                        players[i].DrawOneCard(DrawOneCard());
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
        TakiCardCollection canPlay;
        public enum PlayerState
        {
            specificMove,
            randomMove
        }
        public PlayerState currentPlayerState;
        public TakiCard specificCardToPlay;

        public TakiCardCollection hand;
        public Player()
        {
            hand = new TakiCardCollection();
            currentPlayerState = PlayerState.randomMove;
            canPlay = new TakiCardCollection();
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
            if( currentPlayerState == PlayerState.specificMove)
            {
                return specificCardToPlay;
            }
            
            for (int cardIndex=0; cardIndex < hand.cards.Length; cardIndex++)
            {
                if (table.CanPlay(hand.cards[cardIndex]))
                {
                    canPlay.AddCard(hand.cards[cardIndex]);
                }
            }
            TakiCard randomCard = canPlay.RandomCard();
            return randomCard;
        }
        public void CopyPlayerFrom(Player player)
        {
            hand.CopyCardsFrom(player.hand);
        }
        public bool ShouldFinishTakiAction()
        {
            //TODO: implement
            return false;
        }
    }
    
    public class TakiSimulation
    {

        TakiTable game, // current table on server 
            moveTable; // table for checking a move
        Player me; // the player that wants to win

        public TakiSimulation(int numPlayers)
        {
            // generate full collection of cards
            game.drawPile.CopyCardsFrom(TakiTable.takiDeck);
            game.drawPile.Shuffle();
            // draw cards to my player
            Player me = new Player();
            game.AddPlayer(me);
            this.me = me;
            moveTable = new TakiTable(numPlayers-1);
            for (int i = 0; i < numPlayers-1; i++)
            {
                game.AddPlayer(new Player());
            }
        }
        public static TakiSimulation LoadGame()
        {
            return new TakiSimulation(1);
        }

        /**
         * in the game we only know our hand, discard pile and leading card.
         * we do not know other hands
         * to simulate other hands we keep all cards that are not discarded and not in our hand in a drawPile
         * and then each turn draw cards to other hands according to constraints.
        */

        public static TakiSimulation GenerateRandomGame()
        {
            TakiSimulation sim = new TakiSimulation(4);
            // draw cards for my player
            for (int cardIndex = 0; cardIndex < 8; cardIndex++)
            {
                sim.me.DrawOneCard(sim.game.DrawOneCard());
            }            
            // draw cards to pile to simulate first round before us.
            // TODO:check first special leading card
            TakiCard leadingCard = sim.game.drawPile.Pop();
            sim.game.discardPile.AddCard(leadingCard);
            return sim;
        }
        public void SimulateNextMove()
        {
            int numCardsIcanPlay = 0;
            TakiCardCollection canPlay = new TakiCardCollection();
            for (int i = 0; i < me.hand.numCards; i++)
            {
                if (game.CanPlay(me.hand.cards[i]))
                {
                    numCardsIcanPlay++;
                    canPlay.AddCard(me.hand.cards[i]);
                }
            }

            // we do it here once, but should do it several times (10000) each time shuffling drawPile and 
            // drawing cards to other palyers
            float[] moveCost = new float[numCardsIcanPlay];
            for (int mvInd = 0; mvInd < numCardsIcanPlay; mvInd++)
            {
                moveCost[mvInd] = SimulateMove(canPlay.cards[mvInd]);
                // do not forget to update moveTable after each turn
                // or copy it from game when rolling back
            }
        }

        public float SimulateMove(TakiCard move)
        {
            moveTable.CopyTableFrom(game);
            me.currentPlayerState = Player.PlayerState.specificMove;
            me.specificCardToPlay = move;
            moveTable.PlayTurn();

            me.currentPlayerState = Player.PlayerState.randomMove;

            int depth = 5; // should do stratified depth depending on move cost
            // i.e. look deeper for moves that are good and shallow for not so good moves
            for( int turn=0; turn< depth; turn++)
            {
                moveTable.PlayTurn();
            }
            // computing cost here
            /**
             * should have 1 when winning
             * should have 0 if someone won (should play turn returns whether game ended?)
             * So ???
             * ?? ratio of my cards vs best player?
             * nCards(bestPlayer)/(nCards(me)+nCards(bestPlayer)); nm= nCards(me), nb= nCards(bestPlayer)
             * if nb=0 => 0/(0+??)= 0 we lost
             * if nm=0 => ?/(?+0)= 1 we wAn
             * if nm= nb => n/(n+n)= 1/2
             */
            int nm = game.players[0].hand.numCards;
            int nb = game.players[1].hand.numCards;
            for (int player = 2; player < game.players.Length; player++)
            {
                int tmp = game.players[player].hand.numCards; 
                if ( tmp< nb)
                {
                    nb = tmp;
                }
            }
            return ((float)nb) / ((float)(nm + nb));
        }

    }
}


