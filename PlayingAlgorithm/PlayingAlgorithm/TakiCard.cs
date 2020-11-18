using System;
using Newtonsoft.Json.Linq;

namespace PlayingAlgorithm
{
    public class TakiCard
    {
        public TakiCardType type { get; private set; }
        public int face { get; private set; }
        public TakiColor color;//{ get; private set; }
        public TakiCard( TakiCardType _type, TakiColor _color= null, int _face= -1)
        {
            this.type = _type;
            face = _face;
            if( _color == null )
            {
                _color = TakiColor.any;
            }
            color = _color;
        }
        public static TakiCard FromJSON(Newtonsoft.Json.Linq.JToken token) {
            TakiColor color= TakiColor.any;
            TakiCardType type= TakiCardType.numberCard;
            int value= -1;
            foreach (JToken v in token)
            {
                if( v is JProperty)
                {
                    
                    JProperty p = (JProperty)v;
                    //Console.WriteLine(p.Name+" "+p.Value);
                    switch( p.Name)
                    {
                        case "color":
                            color = TakiColor.FromString(p.Value.ToString());
                            break;
                        case "type":
                            type = TakiCardType.FromString(p.Value.ToString());
                            break;
                        case "value":
                            string sv = p.Value.ToString();
                            if(sv.Length == 0)
                            {
                                value = -1;
                            } else
                            {
                                value = int.Parse(sv);
                            }
                            
                            break;
                    }
                    
                }
                
            }
            return new TakiCard(type, color, value);
        }
        public JObject ToJSON()
        {
            JObject json;
            if (face < 0)
            {
                json = new JObject(
                new JProperty("type", type.ToString()),
                new JProperty("color", color.ToString()),
                new JProperty("value", "")
                );
                return json;
            }
            json = new JObject(
                new JProperty("type", type.ToString()),
                new JProperty("color", color.ToString() ),
                new JProperty("value", face )
                );
            return json;
        }
        public virtual bool CanPutOnColor(TakiColor color)
        {
            return color.SameColor( this.color );
        }
        public virtual void CopyCardFrom(TakiCard card)
        {
            type = card.type;
            face = card.face;
            color = card.color;
        }
        public bool SameType( TakiCardType t)
        {
            return t.SameType(type);
        }
        public bool SameType(TakiCard card)
        {
            return card.type.SameType(type);
        }
        public bool SameColor(TakiColor color)
        {
            return color.SameColor(this.color);
        }
        public bool SameCard( TakiCard card)
        {
            return type.SameType(card.type) &&
                color.SameColor(card.color) &&
                face == card.face;
        }
        public bool SameColor(TakiCard card)
        {
            return card.color.SameColor(color);
        }
        public override string ToString()
        {
            // return "type: " + type.ToString() + " color: " + color.ToString() + " face: " + face;
            return ((type.type != TakiCardType.numberCard_type)?type.ToString():"#") + " " + color.ToString() + " " + face;
        }
    }
}


