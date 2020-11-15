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


