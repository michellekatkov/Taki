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
            for (int i = 0; i < numPlayers-1; i++)
            {
                game.AddPlayer(new Player());
            }
            optionalColoredCards = new TakiCardCollection();
            TakiTable.AddColorsFor(TakiCardType.superTaki, optionalColoredCards);
            TakiTable.AddColorsFor(TakiCardType.changeColor, optionalColoredCards);
            handCards= new int[numPlayers];
            for (int i = 0; i < numPlayers ; i++)
            {
                handCards[i] = 8;
            }
        }
        public static bool inOptionalColorCollection( TakiCard card)
        {
            return (card.type.type & (TakiCardType.superTaki_type | TakiCardType.changeColor_type)) != 0;
        }
        public static TakiSimulation LoadGame( Client.ClientSocket sock )
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
        public TakiMove SimulateNextMove()
        {
            int numCardsIcanPlay = 0;
            canPlay = new List<TakiMove>();
            for (int i = 0; i < me.hand.numCards; i++)
            {
                TakiCard card = me.hand.cards[i];
                if (inOptionalColorCollection(card))
                {
                    // try all colors                    
                    if (game.CanPlay(card))
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            canPlay.Add(new TakiMove(card,TakiColor.colors[k]));
                        }
                    }                    
                }
                else if (game.CanPlay(card))
                {
                    //Console.WriteLine("    --== +++ ==--   ");
                    numCardsIcanPlay++;
                    canPlay.Add(new TakiMove(card));
                }
                else
                {
                    Console.WriteLine("    -    no     -   ");
                }
            }
            numCardsIcanPlay = canPlay.Count;
            if (numCardsIcanPlay == 0)
            {
                return null;
            }
            // we do it here once, but should do it several times (10000) each time shuffling drawPile and 
            // drawing cards to other palyers
            float[] moveCost = new float[numCardsIcanPlay];
            for (int mvInd = 0; mvInd < numCardsIcanPlay; mvInd++)
            {
                moveCost[mvInd] = 0.0F;
            }
            int maxCostInd=0;
            int nIter;
            /*
            int[] handCards = new int[4]; // TODO: preallocate in class
            for (int k = 0; k < 4; k++)
            {
                handCards[k] = game.players[k].hand.numCards;
            }
            SimulateContext context = new SimulateContext(handCards); */
            for (nIter = 0; nIter < 1; nIter++)
            {
                for (int mvInd = 0; mvInd < canPlay.Count; mvInd++)
                {
                    moveCost[mvInd] = SimulateMove(canPlay[mvInd]);
                    // do not forget to update moveTable after each turn
                    // or copy it from game when rolling back
                    if (moveCost[mvInd] > moveCost[maxCostInd])
                    {
                        maxCostInd = mvInd;
                    }
                }
            }
            return canPlay[maxCostInd];
        }
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
             */
            

            return new TakiCard(TakiCardType.numberCard);
        }
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
        public float SimulateMove( TakiMove move )
        {
            moveTable.CopyTableFrom(game);
            moveTable.simulation = this;
            moveTable.players[0].currentPlayerState = Player.PlayerState.specificMove;
            moveTable.players[0].simulator = this;
            moveTable.players[0].specificMoveToPlay = move;
            moveTable.players[0].specificCardsPlayed = 0;
            // TODO: if specific move is taki or super_taki make correct stopTaki flag
            // draw cards to all players according to constraints they have

            for (int k = 1; k < 4; k++)
            {
                // TODO: implements constraints
                Player p = moveTable.players[k];
                p.hand.Empty();
                int[] faces = new int[10];
                int[] colors = new int[4];
                for (int i = 0; i < handCards[k]; i++)
                {
                    while (true)
                    {
                        TakiCard card = moveTable.DrawOneCard();
                        bool draw = true;
                        p.DrawOneCard(card);
                        /*if (card.color.isAnyColor())
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
                                p.DrawOneCard(card);
                                faces[card.face]++;
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
                        } */
                        break;
                    }
                }
            }

            moveTable.PlayTurn();

            moveTable.players[0].currentPlayerState = Player.PlayerState.randomMove;

            int depth = 5; // should do stratified depth depending on move cost
            // i.e. look deeper for moves that are good and shallow for not so good moves
            for( int turn=0; turn< depth; turn++)
            {
                moveTable.PlayTurn();
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
            return ((float)nb) / ((float)(nm + nb));
        }
        public TakiMove AdditionalSpecificMove()
        {
            // in the first move we can play more

            // we need to sort all variants here

            return null;
        }
    }
}


