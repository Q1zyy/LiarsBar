namespace LiarsBarApplication
{
    public class Gun
    {

        private LinkedList<bool> _bullets;

        public Gun()
        {
            bool[] bullets = new bool[6] { false, false, false, false, false, true };
            Random random = new Random();
            random.Shuffle(bullets);
            _bullets = new LinkedList<bool>(bullets);
        }

        public bool Shot()
        {
            bool now = _bullets.First();
            _bullets.RemoveFirst();
            return now; 
        }

    }
}
