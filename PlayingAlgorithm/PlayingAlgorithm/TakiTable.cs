using System;


namespace PlayingAlgorithm
{
    public class TakiTable
    {
        public TakiCardCollection discardPile { get; private set; }
        public TakiCardCollection drawPile{ get; private set; }
        public TakiCard leadingCard;
        public TakiColor actionColor; //{ get; private set; }
        public bool takiAction; //{ get; private set; }
        public bool stopAction = false;
        public bool plus2Action { get; private set; }
        bool direction = true; // forward
        public int plus2amount=0; // should be in construction and in the beggining of game
        public Player[] players;
        int numPlayers, addedPlayers;
        public TakiSimulation simulation;

        public TakiTable(int numPlayers)
        {
            this.numPlayers = numPlayers;
            addedPlayers = 0;
            players = new Player[numPlayers];
            drawPile = new TakiCardCollection();
            discardPile = new TakiCardCollection();
            leadingCard = new TakiCard(TakiCardType.numberCard);
            actionColor = new TakiColor(TakiColor.Color.any);
            for(int k=0; k< numPlayers; k++)
            {
                players[k] = new Player();
            }
        }

        /** 
         * play one turn of the game return if game is finished
         */
        public static readonly TakiCardCollection takiDeck = GenerateTakiDeck();
        public static TakiCardCollection GenerateTakiDeck()
        {
            //change to real num
            int numRepetitions = 2;
            int specialRepetitions = 4;

            TakiCardCollection takiDeck = new TakiCardCollection();
            // generate numericCards and coloredSpecialCards
            
            for (int i = 0; i < numRepetitions; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    TakiFace face = new TakiFace(j);
                    takiDeck.AddCard(new TakiCard(TakiCardType.numberCard, TakiColor.blue, j));
                    takiDeck.AddCard(new TakiCard(TakiCardType.numberCard, TakiColor.red, j));
                    takiDeck.AddCard(new TakiCard(TakiCardType.numberCard, TakiColor.green, j));
                    takiDeck.AddCard(new TakiCard(TakiCardType.numberCard, TakiColor.yellow, j));                   
                }
                AddColorsFor(TakiCardType.stop, takiDeck);
                AddColorsFor(TakiCardType.taki, takiDeck);
                AddColorsFor(TakiCardType.plus, takiDeck);
                AddColorsFor(TakiCardType.plus2, takiDeck);
                AddColorsFor(TakiCardType.changeDirection, takiDeck);                
                takiDeck.AddCard(new TakiCard(TakiCardType.superTaki));
            }
            // generate 4x switch color
            for (int k = 0; k < specialRepetitions; k++)
            {                
                takiDeck.AddCard(new TakiCard(TakiCardType.changeColor));
            }
            return takiDeck;
        }
        public static void AddColorsFor( TakiCardType type, TakiCardCollection takiDeck)
        {
            takiDeck.AddCard(new TakiCard(type, TakiColor.blue));
            takiDeck.AddCard(new TakiCard(type, TakiColor.red));
            takiDeck.AddCard(new TakiCard(type, TakiColor.green));
            takiDeck.AddCard(new TakiCard(type, TakiColor.yellow));
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
            if (table.leadingCard != null)
            {
                if( leadingCard ==null)
                {
                    leadingCard = new TakiCard(TakiCardType.numberCard);
                }
                leadingCard.CopyCardFrom(table.leadingCard);
            } else
            {
                leadingCard = null;
            }
            actionColor=table.actionColor;
            takiAction = table.takiAction;
            plus2Action = table.plus2Action;
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
            bool takiRoolback= false;
            TakiMove move;
            TakiCard card;
            //plus2Action = false;
            
            //bool stopAction = false;
            int i = (direction) ? 0 : players.Length-1;
            while( true )
            {/* due to the possibility of changing direction we should terminate turn when all players inside loop*/
                while (true)
                {/* one player can put many cards during his/her/its/undefined turn  */ 
                    if (stopAction)
                    {//TODO: check that you can not play any card in any circumstances
                        stopAction = false;                 
                        break;
                    }

                    //Console.WriteLine("player             " + i +"  cards ("+ players[i].hand.numCards + ")");
                    //Console.WriteLine("                                  ta: "+ takiAction + 
                        //"  p2: "+ plus2amount+"  ac: "+actionColor );

                    //Console.WriteLine(players[i].hand.ToString());
                    if(players[i].hand.numCards == 0)
                    {
                        return false;
                    }
                    move = players[i].PlayCard(this);
                    Console.WriteLine("                   "+i+"  playing "+move);
                    //plus2Action = false;
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
                            /*if( simulation!= null)
                            {
                                simulation.handCards[i]++;
                            }*/
                        }
                        players[i].DrawOneCard(DrawOneCard());
                        /*if (simulation != null)
                        {
                            simulation.handCards[i]++;
                        }*/
                    }
                    else
                    {
                        // check if should stop takiAction 
                        if (takiAction)
                        {//cards that finish taki action: stop, plus2, changeColor, changeDirection
                            // can be later optimized
                            /*
                            if (card.SameType( TakiCardType.stop ) ||
                                card.SameType( TakiCardType.plus2) ||
                                card.SameType(TakiCardType.changeColor) ||
                                card.SameType(TakiCardType.changeDirection) )
                            {
                                takiAction = false;
                            }
                            else */
                            if (move.stopTaki)
                            {
                                takiAction = false;
                            }
                        }

                        /*
                         * we can have:
                         * first move: leading card == null
                         * taki rollback: leading card != null, but card == null 
                         * normal move: leading card != null, but card != null 
                         */

                        card = move.card;
                        if (leadingCard != null)
                        {
                            if (card != null)
                            {  // normal move
                                drawPile.PutAtRandom(leadingCard);
                                leadingCard = card;
                                int idx = players[i].hand.FindCard(card);
                                players[i].hand.RemoveCard(idx);
                            }
                            else
                            { // Taki rollback
                                card = leadingCard;
                                if(actionColor!=null)
                                    move.actionColor = actionColor;
                                if( card.SameType( TakiCardType.taki ) ||
                                    card.SameType(TakiCardType.taki))
                                {
                                    takiRoolback = true;
                                }
                            }
                        }
                        else
                        { // first move in a game
                            leadingCard = card;
                            int idx = players[i].hand.FindCard(card);
                            players[i].hand.RemoveCard(idx);
                        }                  
                        if (! takiAction )
                        {                             
                            switch ( card.type.type )
                            {
                                case TakiCardType.plus_type:
                                    actionColor = card.color;
                                    continue;
                                case TakiCardType.plus2_type:
                                    actionColor = card.color;
                                    plus2amount += 2;                                    
                                    break;
                                case TakiCardType.stop_type:
                                    actionColor = card.color;
                                    stopAction = true;
                                    break;
                                case TakiCardType.superTaki_type:
                                    actionColor = move.actionColor;
                                    if (takiRoolback)
                                    {
                                        takiAction = false;
                                        break;
                                    }
                                    takiAction = !move.stopTaki;
                                    takiRoolback = false;
                                    continue;
                                case TakiCardType.taki_type:
                                    actionColor = card.color;
                                    if (takiRoolback)
                                    {
                                        takiAction = false;
                                        break;
                                    }
                                    takiAction = !move.stopTaki;
                                    takiRoolback = false;
                                    continue;                                   
                                case TakiCardType.changeColor_type:
                                    actionColor = move.actionColor;
                                    break;
                                case TakiCardType.changeDirection_type:
                                    actionColor = card.color;
                                    direction = !direction;
                                    break;
                                 default:
                                    actionColor = card.color;
                                    break;
                            }
                        }
                        else
                        {
                            actionColor = card.color;
                        }
                    }
                    if (! takiAction  )
                    {
                        break;
                    }                    
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
        /*
        FIRST_STATE_MACHINE = {
    CardType.NUMBER_CARD: lambda card, other, plus_2_active: card.color == other.color or card.value == other.value or
        other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
    CardType.PLUS: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or   
        other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
    CardType.PLUS_2: lambda card, other, plus_2_active: card.type == other.type 
        or(not plus_2_active and (card.color == other.color or other.type in 
           (CardType.CHANGE_COLOR, CardType.SUPER_TAKI))),
    CardType.STOP: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or 
        other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
    CardType.CHANGE_DIRECTION: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color
        or other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
    CardType.CHANGE_COLOR: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or 
        other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
    CardType.TAKI: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or 
        other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
    CardType.SUPER_TAKI: lambda card, other, plus_2_active: card.type == other.type or other.type == CardType.TAKI or 
        card.color == other.color or other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
    None: lambda card, other, plus_2_active: True
    }

    REGULAR_STATE_MACHINE = {
    CardType.NUMBER_CARD: lambda card, other, in_taki: and card.color == other.color,
    CardType.PLUS: lambda card, other, in_taki: card.type == other.type or card.color == other.color,
    CardType.PLUS_2: lambda card, other, in_taki: False,
    CardType.STOP: lambda card, other, in_taki: False,
    CardType.CHANGE_DIRECTION: lambda card, other, in_taki: False,
    CardType.CHANGE_COLOR: lambda card, other, in_taki: False,
    CardType.TAKI: lambda card, other, in_taki: card.color == other.color,
    CardType.SUPER_TAKI: lambda card, other, in_taki: card.color == other.color
}


def valid_move(card, last_card= Card(None, '', ''), first= False, in_taki= False, plus_2_active= False):
    if first:
        return FIRST_STATE_MACHINE[last_card.type] (last_card, card, plus_2_active)

    return REGULAR_STATE_MACHINE[last_card.type] (last_card, card, in_taki)
    */
        public bool CanPlay(TakiCard card, bool plus2Action= false)
        {
            //Console.WriteLine("  . ");
            if (leadingCard != null)
            {
                //Console.WriteLine("          -- " + leadingCard.ToString());
            } else {
                //Console.WriteLine("          -- ");
            }
            //Console.WriteLine("checking     " + card.ToString());
            if( leadingCard == null)
            {
                // this is first move in the game - 
                // anything possible
                return true;
            }
            if (takiAction)
            {
                // regular machine
                switch( leadingCard.type.type )
                { 
                    // CardType.PLUS: lambda card, other, in_taki: card.type == other.type or 
                    //card.color == other.color,
                    case TakiCardType.plus_type:
                            
                        return leadingCard.SameType(card)|| card.SameColor(actionColor);

                    /*
                        CardType.PLUS_2: lambda card, other, in_taki: False,
                        CardType.STOP: lambda card, other, in_taki: False,
                        CardType.CHANGE_DIRECTION: lambda card, other, in_taki: False,
                        CardType.CHANGE_COLOR: lambda card, other, in_taki: False,
                    */
                    case TakiCardType.plus2_type:
                    case TakiCardType.stop_type:
                    case TakiCardType.changeDirection_type:
                    case TakiCardType.changeColor_type:
                            return false;
                    /*
                    CardType.TAKI: lambda card, other, in_taki: card.color == other.color,
                    CardType.SUPER_TAKI: lambda card, other, in_taki: card.color == other.color
                    CardType.NUMBER_CARD:lambda card, other, in_taki: and card.color == other.color,
                    */
                    case TakiCardType.taki_type:
                    case TakiCardType.superTaki_type:
                    case TakiCardType.numberCard_type:
                        return card.SameColor(actionColor);
                }               
            }
            else
            {
                // first machine
                    switch (leadingCard.type.type)
                    {

                        /*                 
                        CardType.PLUS: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or   
                            other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
                        CardType.STOP: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or 
                            other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),     
            CardType.CHANGE_DIRECTION: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or
                            other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
                CardType.CHANGE_COLOR: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or 
                            other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
                        CardType.TAKI: lambda card, other, plus_2_active: card.type == other.type or card.color == other.color or 
                            other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
                         */
                        case TakiCardType.stop_type:
                        case TakiCardType.plus_type:
                        case TakiCardType.changeDirection_type:
                        case TakiCardType.changeColor_type:
                        case TakiCardType.taki_type:
                            return card.SameType(leadingCard) ||
                                    card.SameColor(actionColor) ||
                                    card.SameType(TakiCardType.superTaki) ||
                                    card.SameType(TakiCardType.changeColor);
                        /*
                          CardType.PLUS_2: lambda card, other, plus_2_active: card.type == other.type 
                          or(not plus_2_active and (card.color == other.color or other.type in 
                          (CardType.CHANGE_COLOR, CardType.SUPER_TAKI)))
                     */
                        case TakiCardType.plus2_type:
                            return card.SameType(leadingCard) ||
                                ( !(leadingCard.SameType(TakiCardType.plus2) && plus2amount>0 ) && 
                                (card.SameColor(actionColor) ||
                                  card.SameType(TakiCardType.superTaki)|| 
                                  card.SameType(TakiCardType.changeColor)
                                ));
                        /*
      CardType.SUPER_TAKI: lambda card, other, plus_2_active: card.type == other.type or other.type == CardType.TAKI or 
                                card.color == other.color or other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),
                         */
                        case TakiCardType.superTaki_type:
                            return card.SameType(leadingCard) ||
                                card.SameType(TakiCardType.taki) ||
                                card.SameColor(actionColor) ||
                                card.SameType(TakiCardType.changeColor) ||
                                card.SameType(TakiCardType.superTaki);
                    /*CardType.NUMBER_CARD: lambda card, other, plus_2_active: card.color == other.color or card.value == other.value or
                           other.type in (CardType.CHANGE_COLOR, CardType.SUPER_TAKI),*/
                    case TakiCardType.numberCard_type:
                        return card.SameColor(actionColor) ||
                            leadingCard.face == card.face ||
                            (card.type.type & (TakiCardType.changeColor_type | TakiCardType.superTaki_type)) != 0;
                    }
            }
            return false;
        }
    }
}


