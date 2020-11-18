namespace PlayingAlgorithm
{
    public class TakiMove {
        public TakiCard card;
        public TakiColor actionColor;
        public bool stopTaki;
        public double moveCost;
        public TakiMove additionalMove;        
        public TakiMove(TakiCard card = null, TakiColor color = null, bool stopTaki = true)
        {
            this.card = card;
            this.actionColor= color;
            this.stopTaki = stopTaki;
            this.additionalMove = null;
            moveCost = 0.0;
        }
        public TakiMove GetNth( int n)
        {
            if (n == 1)
            {
                return this;
            }
            if (additionalMove != null)
            {
                return additionalMove.GetNth(n - 1);
            }
            return null;
        }
        public int Count()
        {
            if(additionalMove == null)
            {
                return 1;
            }
            return 1 + additionalMove.Count();
        }
        public TakiMove Last()
        {
            if( additionalMove == null)
            {
                return this;
            }
            return additionalMove.Last();
        }
        public TakiMove Clone()
        {
            TakiMove move = new TakiMove(card, actionColor, stopTaki);
            if(additionalMove != null)
            {
                move.additionalMove = additionalMove.Clone();                
            }
            return move;
        }
        public TakiMove AddMove(TakiMove move)
        {
            Last().additionalMove = move;
            return this;
        }
        public override string ToString()
        {
            string res = "  =>  ";
            if(card!= null)
            {
                res += card.ToString() + " ";
            }
            if( actionColor != null)
            {
                res += actionColor.ToString();
            } 
            res+=  " stopTaki " + stopTaki + "\n";
            if (additionalMove != null)
            {
                res += additionalMove.ToString();
            }
            return res;
        }
    }
}


