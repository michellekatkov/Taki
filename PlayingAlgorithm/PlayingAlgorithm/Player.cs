using Client;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace PlayingAlgorithm
{
    public class Player
    {
        public bool gameEnded;
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
            if(hand.cards[hand.numCards - 1] == null)
            {
                Console.WriteLine("something wrong");
            }
            for (int cardIndex=0; cardIndex < hand.numCards; cardIndex++)
            {
                if (table.CanPlay(hand.cards[cardIndex], table.plusAction))
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
            //Console.WriteLine(response);
            //Console.WriteLine(response.arguments);
            TakiCardCollection hand = null;
            // here we get message from server
            if (response.status != null)
            {
                if (response.status == "success")
                {
                    if (response.arguments.ContainsKey("cards"))
                    {
                        JArray cards = response.arguments["cards"];

                        foreach (JToken c in cards)
                        {
                            //add card to already saved deck
                            TakiCard card = TakiCard.FromJSON(c);
                            simulator.game.players[0].hand.AddCard(card);
                            int cardIndex = simulator.game.drawPile.FindCard(card);
                            simulator.game.drawPile.RemoveCard(cardIndex);
                        }
                    }
                } else if(response.status == "bad_request")
                {
                    if (response.arguments["message"] == "Invalid move done.")
                    {
                        for (TakiMove m1 = simulator.lastMove; m1 != null; m1 = m1.additionalMove)
                        {

                            if (m1.card != null)
                            {
                                int idx = simulator.game.drawPile.FindCard(m1.card);
                                simulator.game.drawPile.RemoveCard(idx);
                                simulator.game.players[0].hand.AddCard(m1.card);
                            }
                        }
                        TakiMove move = simulator.GetNextValidMove();
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
                            //if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                            //    Console.WriteLine(" ----------------------------"); ;
                            Console.WriteLine(" hand before move " + simulator.game.players[0].hand);
                            for (TakiMove m1 = move; m1 != null; m1 = m1.additionalMove)
                            {

                                if (m1.card != null)
                                {
                                    cards.Add(new TakiCard(m1.card.type,
                                        m1.card.color.isAnyColor() ? m1.actionColor : m1.card.color,
                                        m1.card.face)
                                        .ToJSON());

                                    int idx = simulator.game.players[0].hand.FindCard(m1.card);
                                    simulator.game.players[0].hand.RemoveCard(idx);
                                    simulator.game.drawPile.PutAtRandom(m1.card);
                                }
                            }

                            req.arguments.Add("cards", cards);
                            Console.WriteLine("player " + currentPlayer + " sending request: " + req.ToString());
                            sock.SendRequest(req);
                        }

                    }
                }
            }
            switch (response.code)
            {
                case "game_ended":
                    Console.WriteLine("\n\n\n\n\n\nGame Ended "+response.arguments["scoreboard"]);
                    gameEnded = true;
                    break;
                case "move_done":
                    //if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                    //Console.WriteLine(" ----------------------------"); ;
                    Console.WriteLine( "player "+myName+ " "+ simulator.game.players[0].hand.numCards);
                    int numTakenCards = 0;
                    hand = new TakiCardCollection();
                    int playerIdx = serverPlayers[response.arguments["player_name"]];
                    if (response.arguments["type"] == "cards_taken")
                    {
                        numTakenCards = ( int )response.arguments["amount"]; 
                        // put constraint here
                        if(numTakenCards == 1)
                        {
                            // restrictions based on leading card and action color
                            if(simulator.game.leadingCard.SameType(TakiCardType.numberCard))
                            {
                                // no numbers 
                                simulator.game.players[playerIdx].faceRestriction.
                                    maxCards[simulator.game.leadingCard.face]= 0;
                            }
                            simulator.game.players[playerIdx].colorRestriction.
                               maxCards[(int)simulator.game.actionColor.myColor] = 0;
                        }
                        simulator.handCards[playerIdx] += numTakenCards;
                        simulator.game.players[playerIdx].colorRestriction.AddCards(numTakenCards);
                        simulator.game.players[playerIdx].faceRestriction.AddCards(numTakenCards);
                        if (simulator.game.plus2amount > 0)
                        {
                            simulator.game.plus2amount = 0;
                        }
                        
                       

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
                            if (card.SameType(TakiCardType.plus2))
                            {
                                simulator.game.plus2amount += 2;
                            }
                            else
                            {
                                simulator.game.plus2amount = 0;
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
                    //if(!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                        //Console.WriteLine(" ----------------------------"); 
                    break;
                case "update_turn":
                    currentPlayer = response.arguments["current_player"];
                    if (currentPlayer == myName)
                    {
                        // here we should really play
                        foreach (KeyValuePair<string,int> entry in serverPlayers )
                        {
                            Console.WriteLine(myName+" handsCards " + entry.Key+" "+simulator.handCards[entry.Value]);
                        }
                        TakiMove move = null;
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
                            //if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                            //    Console.WriteLine(" ----------------------------"); ;
                            Console.WriteLine(" hand before move "+ simulator.game.players[0].hand);
                            for (TakiMove m1 = move; m1 != null; m1 = m1.additionalMove)
                            {
                                
                                if (m1.card != null) {
                                    cards.Add(new TakiCard(m1.card.type,
                                        m1.card.color.isAnyColor() ? m1.actionColor : m1.card.color,
                                        m1.card.face)
                                        .ToJSON());

                                    int idx = simulator.game.players[0].hand.FindCard(m1.card);
                                    simulator.game.players[0].hand.RemoveCard(idx);
                                    simulator.game.drawPile.PutAtRandom(m1.card);
                                }
                            }

                            req.arguments.Add("cards", cards);
                        Console.WriteLine("player "+currentPlayer+" sending request: "+req.ToString());
                        sock.SendRequest(req);
                        }
                        //if (!TakiTable.takiDeck.IsExactlySameCollection(TakiTable.takiDeck2))
                         //   Console.WriteLine(" ----------------------------");

                    }
                    break;
                case "game_starting":
                    hand = new TakiCardCollection();
                    foreach (KeyValuePair<string, dynamic> entry in response.arguments)
                    {
                        switch( entry.Key)
                        {
                            case "cards":
                                //Console.WriteLine(entry.Value);
                                if(entry.Value is Newtonsoft.Json.Linq.JArray)
                                {
                                    JArray arr = entry.Value;
                                    foreach(JToken v in arr)
                                    {
                                        //Console.WriteLine( v );
                                        TakiCard card = TakiCard.FromJSON( v );
                                        hand.AddCard(card);
                                    }
                                }
                                break;
                            case "players":
                                //playerNames = Convert.ToString(entry.Value);
                                if (entry.Value is Newtonsoft.Json.Linq.JArray)
                                {
                                    JArray arr = entry.Value;
                                    int k = 0;
                                    foreach (JToken v in arr)
                                    {
                                        //Console.WriteLine(v);
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


