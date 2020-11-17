using Client;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace PlayingAlgorithm
{
    public class Player
    {
        TakiCardCollection canPlay;
        //TakiCard takiActionLastCard;
        public enum PlayerState
        {
            specificMove,
            randomMove
        }
        public TakiSimulation simulator;
        public PlayerState currentPlayerState;
        public TakiMove specificMoveToPlay;
        public int specificCardsPlayed;
        public TakiCardCollection hand;
        public TakiCardRestriction_Color colorRestriction;
        public TakiCardRestriction_Face faceRestriction;
        public static TakiCardCollection debugCollection=new TakiCardCollection();
        public Player()
        {
            hand = new TakiCardCollection();
            currentPlayerState = PlayerState.randomMove;
            canPlay = new TakiCardCollection();
            simulator = null;
            serverPlayers = new Dictionary<string, int>();
            colorRestriction = new TakiCardRestriction_Color();
            faceRestriction = new TakiCardRestriction_Face();
        }
        private static float alphaWeight = 0.5F;
        public void DrawOneCard(TakiCard card)
        {
            hand.AddCard(card);
        }
        public TakiColor changeColorTo()
        {
            // TODO: implement
            /**
             * in a simple version we compute color weight (cw[color]) somehow
             * than compute probability weight as pw[color] = exp( alpha * cw[color] )
             * initially we will fix it to constants and than can change them due to results of the game
             * cw[color] should depend on the number of cards (linearly) presence of some special cards,
             * for example +2 should add some weight (that we do not know, and that is why we need to learn it across games)
             * stop should have another weight
             * TODO: define constants for each special card (may be it is static member in the special type class that would
             * be loaded) whe n game starts) 
            */
            return null;
        }

        const int stopTakiFlag = (TakiCardType.stop_type | TakiCardType.plus2_type |
                           TakiCardType.changeColor_type |
                           TakiCardType.changeDirection_type) & 0xff;

        /**
             * this function is called by TakiTable when it is a players turn
             * return single card played by Player or null if he can't.
             */
        public TakiMove PlayCard(TakiTable table)
        {
            canPlay.Empty();
            bool canPlayPlus = false;
            bool canPlayPlus2 = false;
            bool canPlayTaki = false;
            for (int cardIndex=0; cardIndex < hand.numCards; cardIndex++)
            {
                if (table.CanPlay(hand.cards[cardIndex]))
                {
                    TakiCard c = hand.cards[cardIndex];
                    canPlay.AddCard( c );
                    switch(c.type.type)
                    {
                        case TakiCardType.plus_type:
                            canPlayPlus = true;
                            break;
                        case TakiCardType.plus2_type:
                            canPlayPlus2 = true;
                            break;
                        case TakiCardType.taki_type:
                        case TakiCardType.superTaki_type:
                            canPlayTaki = true;
                            break;
                    }                    
                    //Console.WriteLine("\\________________/");
                }
            }
            //Console.WriteLine("     ----- Can play: -----  ");
            //Console.WriteLine(  canPlay.ToString() );
            if( currentPlayerState == PlayerState.specificMove)
            {
                // delegate all responsibility to simulator
                /*if (table.takiAction && simulator != null)
                {
                    return simulator.playingTaki(table);
                }*/
                if( specificCardsPlayed > 0)
                {
                    if( simulator != null)
                    {
                        specificCardsPlayed++;
                        return simulator.AdditionalSpecificMove();
                    }
                }else
                {
                    specificCardsPlayed++;
                    return specificMoveToPlay;
                }
                
            }

            if (table.takiAction)
            { // we want get rid of most of the cards
              /*move.SameType(TakiCardType.stop) ||
                              move.SameType(TakiCardType.plus2) ||
                              move.SameType(TakiCardType.changeColor) ||
                              move.SameType(TakiCardType.changeDirection)*/
                int k, lastCard;
                for (lastCard = canPlay.numCards-1; lastCard >0; lastCard--)
                {
                    if ((canPlay.cards[lastCard].type.type & stopTakiFlag) == 0)
                    {
                        break;
                    }
                }
                for (k = 0; k < lastCard; k++)
                {
                    if ((canPlay.cards[k].type.type & stopTakiFlag) != 0)
                    {
                        canPlay.Swap(k, lastCard);
                        for ( lastCard--; lastCard > 0; lastCard--)
                        {
                            if ((canPlay.cards[lastCard].type.type & stopTakiFlag) == 0)
                            {
                                break;
                            }
                        }
                    }
                }
                if (canPlay.numCards == 0)
                {
                    TakiMove move1 = new TakiMove();
                    move1.stopTaki = true;
                    return move1;
                }
                TakiCard card = canPlay.cards[0];
                TakiMove move=new TakiMove(card);
                if( canPlay.numCards ==1)
                {
                    move.stopTaki = true;
                }else
                {
                    move.stopTaki = false;
                }

                if (TakiSimulation.inOptionalColorCollection(card))
                {
                    move.actionColor = TakiColor.GetRandomColor();
                }
                return move;
            }
            else
            {
                if (canPlay.numCards > 0)
                {
                    if (canPlayPlus) {
                        for (int i = 0; i < canPlay.numCards; i++)
                        {
                            TakiCard c = canPlay.cards[i];                            
                            if (c.SameType(TakiCardType.plus))
                            {
                                TakiMove move = new TakiMove(c);
                                return move;
                            }
                        }
                    }
                    else if (canPlayPlus2)
                    {
                        for (int i = 0; i < canPlay.numCards; i++)
                        {
                            TakiCard c = canPlay.cards[i];
                            if (c.SameType(TakiCardType.plus2))
                            {
                                TakiMove move = new TakiMove(c);
                                return move;
                            }
                        }
                    } else if (canPlayTaki)
                    {
                        for (int i = 0; i < canPlay.numCards; i++)
                        {
                            TakiCard c = canPlay.cards[i];
                            if (c.SameType(TakiCardType.taki))
                            {
                                TakiMove move = new TakiMove(c);
                                move.stopTaki = canPlay.numCards == 1;
                                return move;
                            }
                        }
                        for (int i = 0; i < canPlay.numCards; i++)
                        {
                            TakiCard c = canPlay.cards[i];
                            if (c.SameType(TakiCardType.superTaki))
                            {
                                TakiMove move = new TakiMove(c);
                                move.stopTaki = canPlay.numCards == 1;
                                selectWeightedColor(move);
                                return move;
                            }
                        }
                    }
                    else
                    {
                        TakiCard card = canPlay.RandomCard();
                        TakiMove move = new TakiMove(card);
                        if (TakiSimulation.inOptionalColorCollection(card))
                        {
                            selectWeightedColor(move);
                        }
                        
                        return move;
                    }
                }
                return null;
            }            
        }

        void selectWeightedColor( TakiMove move )
        {
            int[] colArray = new int[256];
            int nCol = 0;
            for (int i = 0; i < hand.numCards; i++)
            {
                TakiColor c = hand.cards[i].color;
                if (!c.isAnyColor())
                {
                    colArray[nCol] = (int)c.myColor;
                    nCol++;
                }
            }

            if (nCol == 0)
            {
                move.actionColor = TakiColor.GetRandomColor();
            }
            else
            {
                move.actionColor = TakiColor.colors[colArray[MyRandom.randomUniform(nCol)]];
            }
        }

        public void CopyPlayerFrom(Player player)
        {
            hand.CopyCardsFrom(player.hand);
        }
        public bool ShouldFinishTakiAction(TakiTable game)
        {
            //TODO: implement advance version
            // TODO: decide in the beginning which card would be payed last and put it last in the player hand
            for (int i = 0; i < hand.numCards; i++)
            {
                if (hand.cards[i].CanPutOnColor(game.actionColor))
                {
                    return false;
                }
            }           
            return true;
        }
        Dictionary<string, int> serverPlayers;
        string currentPlayer, myName;
        public void ProcessServerMessage( Response response )
        {
            Console.WriteLine(response);
            Console.WriteLine(response.arguments);
            TakiCardCollection hand = null;
            // here we get message from server
            switch (response.code)
            {
                case "move_done":
                    if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                        Console.WriteLine("herna ----------------------------"); ;
                    
                    int numTakenCards = 0;
                    hand = new TakiCardCollection();
                    int playerIdx = serverPlayers[response.arguments["player_name"]];
                    if (response.arguments["type"] == "cards_taken")
                    {
                        numTakenCards = response.arguments["amount"];
                        simulator.handCards[playerIdx] += numTakenCards;
                        // put constraint here

                    }
                    else
                    {
                        if (response.arguments["cards"] is JArray)
                        {
                            JArray arr = response.arguments["cards"];
                            TakiCard card = null;
                            foreach (JToken v in arr)
                            {
                                //Console.WriteLine(v);
                                card = TakiCard.FromJSON(v);
                                hand.AddCard(card);
                            }
                            simulator.game.actionColor = card.color;
                            simulator.game.takiAction = false;
                            simulator.handCards[playerIdx] -= hand.numCards;
                            int ci = simulator.game.drawPile.FindCard(card);
                            if (ci < 0)
                            {
                                // means color= any
                                card.color = TakiColor.any;
                                ci = simulator.game.drawPile.FindCard(card);
                            }
                            simulator.game.leadingCard = card;
                        }
                    }
                    if(!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                        Console.WriteLine("herna ----------------------------"); ;
                    break;
                case "update_turn":
                    currentPlayer = response.arguments["current_player"];
                    if (currentPlayer == myName)
                    {
                        // here we should really play
                        TakiMove move = new TakiMove();
                        move= simulator.SimulateNextMove();
                        if (move == null)
                        {
                            Request req = new Request("take_cards");
                            req.arguments.Add("jwt", token);
                            sock.SendRequest(req);
                        }
                        else
                        {
                            Request req = new Request("place_cards");
                            req.arguments.Add("jwt", token);
                            // send card to server here

                            JArray cards = new JArray();
                            if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                                Console.WriteLine("herna ----------------------------"); ;
                            
                            for (TakiMove m1 = move; m1 != null; m1 = m1.additionalMove)
                            {
                                
                                if (m1.card != null) {
                                    cards.Add(new TakiCard(m1.card.type,
                                        m1.card.color.isAnyColor() ? m1.actionColor : m1.card.color,
                                        m1.card.face)
                                        .ToJSON());
                                }
                            }

                            req.arguments.Add("cards", cards);
                        Console.WriteLine("player "+currentPlayer+" sending request: "+req.ToString());
                        sock.SendRequest(req);
                        }
                        if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                            Console.WriteLine("herna ----------------------------"); ;

                    }
                    break;
                case "game_starting":
                    hand = new TakiCardCollection();
                    foreach (KeyValuePair<string, dynamic> entry in response.arguments)
                    {
                        switch( entry.Key)
                        {
                            case "cards":
                                Console.WriteLine(entry.Value);
                                if(entry.Value is Newtonsoft.Json.Linq.JArray)
                                {
                                    JArray arr = entry.Value;
                                    foreach(JToken v in arr)
                                    {
                                        Console.WriteLine( v );
                                        TakiCard card = TakiCard.FromJSON( v );
                                        hand.AddCard(card);
                                    }
                                }
                                break;
                            case "players":
                                //playerNames = Convert.ToString(entry.Value);
                                if (entry.Value is Newtonsoft.Json.Linq.JArray)
                                {
                                    Newtonsoft.Json.Linq.JArray arr = entry.Value;
                                    int k = 0;
                                    foreach (Newtonsoft.Json.Linq.JToken v in arr)
                                    {
                                        Console.WriteLine(v);
                                        serverPlayers[ v.ToString() ] = k;
                                        if (k == 0)
                                        {
                                            myName = v.ToString();
                                        }
                                        k++;
                                    }
                                }
                                break;
                        }
                    }
                    // define game table here, we have all information
                    simulator = new TakiSimulation(serverPlayers.Count);
                    simulator.game.players[0].hand.CopyCardsFrom(hand);
                    simulator.game.drawPile.RemoveCardsIn(hand);
                    simulator.game.actionColor = TakiColor.any;
                    simulator.game.leadingCard = null;

                    // leading card ??
                    while (false)
                    {
                        simulator.game.leadingCard = simulator.game.drawPile.Pop();
                        if (!simulator.game.leadingCard.color.isAnyColor())
                        {
                            simulator.game.actionColor = simulator.game.leadingCard.color;
                            break;
                        } 
                        else
                        {
                            simulator.game.drawPile.PutAtRandom(simulator.game.leadingCard);
                        }
                    }
                    break;
            }
        }
        private ClientSocket sock;
        private string token;
        public void RegisterServer(ClientSocket sock, string token)
        {
            // this is needed to send messages to server
            this.sock = sock;
            this.token = token;
        }
    }
}


