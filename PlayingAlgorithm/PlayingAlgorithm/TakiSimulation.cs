using System;
using System.Collections.Generic;

namespace PlayingAlgorithm
{
    public class TakiSimulation
    {
        /* TakiSimulator assumes that players[0] is me */

        TakiCardCollection optionalColoredCards;
        public int[] handCards;
        public TakiTable game { get; private set; } // current table on server 
        TakiTable moveTable; // table for checking a move
        public Player me { get; private set; } // the player that wants to win
        public TakiSimulation(int numPlayers)
        {
            game = new TakiTable(numPlayers);
            // generate full collection of cards
            game.drawPile.CopyCardsFrom(TakiTable.takiDeck);
            game.drawPile.Shuffle();
            // draw cards to my player
            Player me = new Player();
            game.AddPlayer(me);
            this.me = me;
            moveTable = new TakiTable(numPlayers);
            for (int i = 0; i < numPlayers - 1; i++)
            {
                game.AddPlayer(new Player());
            }
            optionalColoredCards = new TakiCardCollection();
            TakiTable.AddColorsFor(TakiCardType.superTaki, optionalColoredCards);
            TakiTable.AddColorsFor(TakiCardType.changeColor, optionalColoredCards);
            handCards = new int[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                handCards[i] = 8;
            }
            canPlay = new List<TakiMove>();
        }
        public static bool inOptionalColorCollection(TakiCard card)
        {
            return (card.type.type & (TakiCardType.superTaki_type | TakiCardType.changeColor_type)) != 0;
        }
        public static TakiSimulation LoadGame(Client.ClientSocket sock)
        {
            Console.WriteLine(" TakiSimulation loading game ");
            TakiSimulation sim = new TakiSimulation(4);
            return sim;
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
        List<TakiMove> canPlay;

        private bool secondTaki=false;

        public void CheckAdditionalMove(TakiCardCollection hand, TakiMove curMove, TakiMove rootMove)
        {
            // this is taki action active
            TakiCardCollection canPlayTaki = new TakiCardCollection();
            TakiCardCollection playSelectedTaki = new TakiCardCollection();
            for (int cardIndex = 0; cardIndex < hand.numCards; cardIndex++)
            {               
                if (moveTable.CanPlay(hand.cards[cardIndex]))
                {
                    TakiCard c = hand.cards[cardIndex];
                    canPlayTaki.AddCard(c);             
                }
            }
            long confToPlay=1;
            for (int i = 0; i < canPlayTaki.numCards; i++)
            {
                confToPlay *= 2;
            }
            for (long conf = 0; conf < confToPlay; conf++)
            {
                long nCards=0;
                for (long k = conf; k >0; k>>=1)
                {
                    nCards += k % 2;
                }
                for (int last = 0; last < nCards; last++)
                {
                    long k = 0;
                }
            }
            TableState state = TableState.Store(moveTable);
            TakiCardCollection tmp = new TakiCardCollection();
            tmp.CopyCardsFrom(hand);
            for (int k = 0; k < tmp.numCards; k++)
            {
                state.Restore(moveTable);
                hand.CopyCardsFrom(tmp);
                TakiCard card = hand.cards[k];
               /* Console.WriteLine("     ==taki==>: "+card+ "     | "
                    +moveTable.leadingCard + " | "
                    +moveTable.actionColor);*/
                if (moveTable.CanPlay(card))
                {
                    int idx = hand.FindCard(card);
                    hand.RemoveCard(idx);
                    TakiMove move = rootMove.Clone().AddMove(new TakiMove(card, null, false));
                    //Console.WriteLine("                     ==taki next ==> \n"+ move);
                    CheckAdditionalMove(hand, null , move);
                }                 
            }
            TakiMove move1 = rootMove.Clone();
            TakiMove nextMove1 = move1.Last();
            moveTable.takiAction = false;
            if (move1 == nextMove1 )
            {
                // this is only taki and nothing else
                nextMove1.AddMove(new TakiMove(null, null, true));
                Console.WriteLine("              + <- \n" + move1);
                canPlay.Add(move1);
            } else 
            {
                nextMove1.stopTaki = true;
                if (nextMove1.card.SameType(TakiCardType.plus))
                {
                    CheckIfCanPlayPlus(hand, move1);
                }
                else if (nextMove1.card.SameType(TakiCardType.taki) ||
                  nextMove1.card.SameType(TakiCardType.superTaki))
                {
                    if (secondTaki)
                    {
                        nextMove1.AddMove(new TakiMove(null, null, true));
                        Console.WriteLine("              + <- \n" + move1);
                        canPlay.Add(move1);
                        secondTaki = false;
                    }
                    else
                    {
                        secondTaki = true;
                        CheckAdditionalMove(hand, null, move1);
                    }
                    
                }
                else
                {
                    Console.WriteLine("              + <- \n" + move1);
                    canPlay.Add(move1);
                }
            }
        }

        public void CheckIfCanPlayPlus( TakiCardCollection hand, TakiMove rootMove)
        {
            bool noMove = true;
            TableState state = TableState.Store(moveTable);
            TakiCardCollection tmp = new TakiCardCollection();
            tmp.CopyCardsFrom(hand);
            //TakiCardCollection tmp2 = new TakiCardCollection();
            for (int i = 0; i < hand.numCards; i++)
            {
                state.Restore(moveTable);
                hand.CopyCardsFrom(tmp);
                TakiCard card = hand.cards[i];
                //Console.WriteLine("    == ++ ==>: "+card.ToString());
                if (moveTable.CanPlay(card))
                {
                    noMove = false;
                    if (inOptionalColorCollection(card))
                    {
                        tmp.CopyCardsFrom(hand);
                        for (int k = 0; k < 4; k++)
                        {
                            //state.Restore(moveTable);
                            hand.CopyCardsFrom(tmp);
                            if (card.type.SameType(TakiCardType.superTaki))
                            {
                                TakiMove move = rootMove.Clone().AddMove(new TakiMove(card, TakiColor.colors[k]));
                                moveTable.leadingCard = card;
                                moveTable.actionColor = TakiColor.colors[k];
                                moveTable.takiAction = true;
                                int idx = hand.FindCard(card);
                                hand.RemoveCard(idx);
                                // root move can be not null
                                CheckAdditionalMove(hand, move, move);
                            }
                            else
                            {
                                TakiMove move = rootMove.Clone().AddMove(new TakiMove(card, TakiColor.colors[k]));
                                Console.WriteLine("              + <- \n" + move);
                                canPlay.Add(move);
                            }
                        }
                    }
                    else
                    {
                        if (card.type.SameType(TakiCardType.taki))
                        {
                            TakiMove move = rootMove.Clone().AddMove(new TakiMove(card));
                            moveTable.leadingCard = card;
                            moveTable.actionColor = card.color;
                            moveTable.takiAction = true;
                            int idx = hand.FindCard(card);
                            hand.RemoveCard(idx);
                            CheckAdditionalMove(hand, move, move);
                        }
                        else if (card.type.SameType(TakiCardType.plus))
                        {
                            TakiMove move = rootMove.Clone().AddMove(new TakiMove(card));
                            moveTable.leadingCard = card;
                            moveTable.actionColor = card.color;
                            int idx = hand.FindCard(card);
                            hand.RemoveCard(idx);
                            CheckIfCanPlayPlus(hand, move);
                        }
                        else
                        {
                            TakiMove move = rootMove.Clone().AddMove(new TakiMove(card));
                            Console.WriteLine("              + <- \n" + move);
                            canPlay.Add(move );
                        }
                    }
                }
            }
            if (noMove)
            {
                TakiMove move = rootMove.AddMove(new TakiMove());
                Console.WriteLine("              + <- " + move);
                canPlay.Add( move );
            }
        }

        class TableState
        {
            TakiCard leadingCard;
            TakiColor actionColor;
            bool takiAction;

            static internal TableState Store( TakiTable table)
            {
                TableState state = new TableState();
                state.leadingCard = table.leadingCard;
                state. actionColor= table.actionColor;
                state.takiAction= table.takiAction;
                return state;
            }

            internal void Restore(TakiTable table)
            {
                table.leadingCard = leadingCard;
                table.actionColor = actionColor;
                table.takiAction = takiAction;
            }

            public override string ToString()
            {
                return leadingCard+" "+actionColor + " "+ takiAction;
            }
        }

        public void CheckIfCanPlay(TakiCardCollection hand, int i )
        {
            // this is first move
            TakiCard card = hand.cards[i];
            Console.WriteLine("==chk=>    "+card +"   | "+moveTable.leadingCard );
            if (moveTable.CanPlay(card))
            {
                if (inOptionalColorCollection(card))
                {
                    TakiCardCollection tmp = new TakiCardCollection();
                    tmp.CopyCardsFrom(hand);
                    for (int k = 0; k < 4; k++)
                    {
                        hand.CopyCardsFrom(tmp);
                        if (card.type.SameType(TakiCardType.superTaki))
                        {
                            TakiMove move = new TakiMove(card, TakiColor.colors[k]);
                            moveTable.leadingCard = card;
                            moveTable.actionColor = TakiColor.colors[k];
                            moveTable.takiAction = true;
                            int idx = hand.FindCard(card);
                            hand.RemoveCard(idx);
                            // root move can be not null
                            CheckAdditionalMove(hand, move, move);
                        }
                        else
                        {
                            TakiMove move = new TakiMove(card, TakiColor.colors[k]);
                            Console.WriteLine("              + <- " + move);
                            canPlay.Add(move);                            
                        }
                    }
                } else
                {
                    if (card.type.SameType(TakiCardType.taki))
                    {
                        TakiMove move = new TakiMove(card);
                        moveTable.leadingCard = card;
                        moveTable.actionColor = card.color;
                        moveTable.takiAction = true;
                        int idx = hand.FindCard(card);
                        hand.RemoveCard(idx);
                        CheckAdditionalMove(hand, move, move);
                    }
                    else if (card.type.SameType(TakiCardType.plus))
                    {
                        TakiMove move = new TakiMove(card);
                        moveTable.leadingCard = card;
                        moveTable.actionColor = card.color;
                        int idx = hand.FindCard(card);
                        hand.RemoveCard(idx);
                        CheckIfCanPlayPlus(hand, move );                        
                    }
                    else
                    {
                        TakiMove move = new TakiMove(card);
                        Console.WriteLine("              + <- "+ move);
                        canPlay.Add(move);
                    }
                }            
            }           
        }
        public TakiMove SimulateNextMove()
        {
            int numCardsIcanPlay = 0;
            Player.debugCollection.CopyCardsFrom(me.hand);
            canPlay.Clear();
            moveTable.CopyTableFrom(game);
            TakiCardCollection hand = new TakiCardCollection();
            hand.CopyCardsFrom(me.hand);
            Console.WriteLine("Selecting move from hand");
            Console.WriteLine(hand);
            TableState state = TableState.Store( moveTable );
            for (int i = 0; i < me.hand.numCards; i++)
            {
                
                moveTable.leadingCard = game.leadingCard;
                moveTable.takiAction = game.takiAction;
                moveTable.plus2amount = game.plus2amount;
                CheckIfCanPlay(hand, i);
                state.Restore( moveTable );
                hand.CopyCardsFrom(me.hand);
            }
            numCardsIcanPlay = canPlay.Count;
            if (numCardsIcanPlay == 0)
            {
                return null;
            }
            // we do it here once, but should do it several times (10000) each time shuffling drawPile and 
            // drawing cards to other palyers
            float[] moveCost = new float[numCardsIcanPlay];
            if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                Console.WriteLine("herna ----------------------------");
            for (int mvInd = 0; mvInd < numCardsIcanPlay; mvInd++)
            {
                canPlay[mvInd].moveCost = 0.0;
            }
            int maxCostInd=0;
            int nIter;
       
            for (nIter = 0; nIter < 3; nIter++)
            {
                if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                    Console.WriteLine("herna ----------------------------"); 
                Console.WriteLine("iteration "+ nIter);
                for (int mvInd = 0; mvInd < canPlay.Count; mvInd++)
                {
                    if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                        Console.WriteLine("herna ----------------------------");
                   // Console.WriteLine("simulating "+ canPlay[mvInd]);
                    double cost= SimulateMove(canPlay[mvInd]);
                    if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                        Console.WriteLine("herna ----------------------------");
                    canPlay[mvInd].moveCost += cost;
                    // do not forget to update moveTable after each turn
                    // or copy it from game when rolling back
                    if (canPlay[mvInd].moveCost > canPlay[maxCostInd].moveCost)
                    {
                        maxCostInd = mvInd;
                    }
                }
            }
            //Console.WriteLine("hand same as debugCollection? "+hand.IsExactlySameCollection(Player.debugCollection));
            if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                Console.WriteLine("herna ----------------------------"); ;

            return canPlay[maxCostInd];
        }
       /*
        public TakiCard playingTaki(TakiTable table)
        {
            /**
             * TODO:
             * Taki move is complicated move that consists of first playing Taki, and than the sequence of cards with the last card. 
             * We need to store it somewhere in the simulator so that we can play it after simulation in a real game.           
             * 
             * 
             * if this is first round we need to test all combinations of last cards that stops takiAction.  
             * 
             * /
            

            return new TakiCard(TakiCardType.numberCard);
        }
       */
        /* simulating specific move specified in arguments
           returns cost which:
             * should have 1 when winning
             * should have 0 if someone won (should play turn returns whether game ended?)
             * So ???
             * ?? ratio of my cards vs best player?
             * nCards(bestPlayer)/(nCards(me)+nCards(bestPlayer)); nm= nCards(me), nb= nCards(bestPlayer)
             * if nb=0 => 0/(0+??)= 0 we lost
             * if nm=0 => ?/(?+0)= 1 we wAn
             * if nm= nb => n/(n+n)= 1/2
             */
        public double SimulateMove( TakiMove move )
        {

            moveTable.CopyTableFrom(game);
            if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                Console.WriteLine("herna ----------------------------");
            moveTable.simulation = this;
            moveTable.players[0].currentPlayerState = Player.PlayerState.specificMove;
            moveTable.players[0].simulator = this;
            moveTable.players[0].specificMoveToPlay = move;
            moveTable.players[0].specificCardsPlayed = 0;
            // TODO: if specific move is taki or super_taki make correct stopTaki flag
            // draw cards to all players according to constraints they have
            moveTable.drawPile.Shuffle();
            if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                Console.WriteLine("herna ----------------------------");
            for (int k = 1; k < 4; k++)
            {
                // TODO: implements constraints
                Player p = moveTable.players[k];
                p.hand.Empty();
                int[] faces = new int[10];
                int[] colors = new int[4];
                for (int i = 0; i < handCards[k]; i++)
                {
                    TakiCard card = null;

                    for (int i3 = 0; i3 < 10000; i3 ++)
                    { // in case we really have a problem with a few cards in  
                        // the draw pile ignore restriction after 10000 iterations.                   
                        card = moveTable.DrawOneCard();
                        bool draw = true;
                        
                        if (card.color.isAnyColor())
                        {                        
                            for (int i2 = 0; i2 < 4; i2++)
                            {
                                if (p.colorRestriction.maxCards[i2] == colors[i2])
                                {
                                    moveTable.drawPile.PutAtRandom(card);
                                    draw = false;
                                    break;
                                }
                            }
                        }
                        else if (p.colorRestriction.maxCards[(int)card.color.myColor] ==
                         colors[(int)card.color.myColor])
                        {
                            moveTable.drawPile.PutAtRandom(card);
                            draw = false;
                            continue;
                        }
                        if (draw && card.type.SameType(TakiCardType.numberCard))
                        {
                            if( p.faceRestriction.maxCards[card.face] == 
                                faces[card.face])
                            {
                                moveTable.drawPile.PutAtRandom(card);
                                draw = false;
                            }
                        }
                        if (draw)
                        {
                            //p.DrawOneCard(card);
                            if (card.face > 0)
                            {
                                faces[card.face]++;
                            }
                                if (card.color.isAnyColor())
                                {                                   
                                    for (int i2 = 0; i2 < 4; i2++)
                                    {
                                        colors[i2]++;
                                    }
                                } else
                                {
                                    colors[(int)card.color.myColor]++;
                                }
                                break;                            
                        }                        
                    }
                    p.DrawOneCard(card);
                }
            }

            if (moveTable.PlayTurn())
            {
                moveTable.players[0].currentPlayerState = Player.PlayerState.randomMove;

                int depth = 1; // should do stratified depth depending on move cost
                               // i.e. look deeper for moves that are good and shallow for not so good moves
                for (int turn = 0; turn < depth; turn++)
                {
                    if (!moveTable.PlayTurn())
                    {
                        break;
                    }
                }
            }
            // computing cost here

            int nm = moveTable.players[0].hand.numCards;
            int nb = moveTable.players[1].hand.numCards;
            for (int player = 2; player < moveTable.players.Length; player++)
            {
                int tmp = moveTable.players[player].hand.numCards; 
                if ( tmp< nb)
                {
                    nb = tmp;
                }
            }
            return ((double)nb) / ((double)(nm + nb));
        }
        public TakiMove AdditionalSpecificMove()
        {
            // in the first move we can play more
            // we need to sort all variants here
            /*
             * There are 2 ways we can come here
             * either because we already planned it
             * or because we did not plan it
             */

            // check if already planned move
            Player p = moveTable.players[0];
            bool planned = false;
            TakiMove curMove = p.specificMoveToPlay;
            for (int i = 0; i < p.specificCardsPlayed && curMove!=null; i++, curMove = curMove.additionalMove)
            {}
            if(curMove!= null)
            {
                return curMove;
            }
            // we can come here onchange direction
            return null;
        }
    }
}


