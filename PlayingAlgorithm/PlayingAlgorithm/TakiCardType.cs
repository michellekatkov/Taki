using System;


namespace PlayingAlgorithm
{
    public class TakiCardType
    {
        public int type { get; private set; }
        public const int stop_type = 0x1 + 0x8000;
        public const int plus2_type = 0x2 + 0x8000;
        public const int changeDirection_type = 0x4 + 0x8000;
        public const int changeColor_type = 0x8;
        public const int taki_type = 0x10 + 0x8000;
        public const int superTaki_type = 0x20;
        public const int plus_type = 0x40 + 0x8000;
        public const int numberCard_type = 0x80 + 0x8000;
        //specialCard

        static public TakiCardType FromString(string v)
        {
            switch( v)
            {
                case "plus": return plus;
                case "number_card": return numberCard;
                case "plus_2": return plus2;
                case "stop": return stop;
                case "change_direction": return changeDirection;
                case "change_color":  return changeColor;
                case "taki":return taki;
                case "super_taki": return superTaki;
            }
            throw new Exception(" unknown type ");
        }
        public TakiCardType( int type)
        {
            this.type = type;
        }

        public static readonly TakiCardType stop = new TakiCardType(stop_type);
        public static readonly TakiCardType plus2 = new TakiCardType(plus2_type);
        public static readonly TakiCardType changeDirection = new TakiCardType(changeDirection_type);
        public static readonly TakiCardType changeColor = new TakiCardType(changeColor_type);
        public static readonly TakiCardType taki = new TakiCardType(taki_type);
        public static readonly TakiCardType superTaki = new TakiCardType(superTaki_type);
        public static readonly TakiCardType plus = new TakiCardType(plus_type);
        public static readonly TakiCardType numberCard = new TakiCardType(numberCard_type);

        public bool SameType( TakiCardType t)
        {
            return type == t.type;
        }
        public override string ToString()
        {
            switch (type)
            {
                case stop_type: return "stop";
                case plus2_type: return "plus_2";
                case changeDirection_type: return "change_direction";
                case changeColor_type: return "change_color";
                case taki_type: return "taki";
                case superTaki_type: return "super_taki";
                case plus_type: return "plus";
                case numberCard_type: return "number_card";
            }
            return "";

        }
    }
}


