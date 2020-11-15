namespace PlayingAlgorithm
{
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
        public bool SameFace( TakiFace face)
        {
            return num == face.num;
        }
    }
}


