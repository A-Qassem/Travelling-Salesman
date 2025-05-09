namespace TSP
{
    public class City
    {
        public Point Position { get; set; }
        public int Index { get; set; }
        public bool IsStartCity { get => Index == 0; }

        public City(int x, int y, int index)
        {
            Position = new Point(x, y);
            Index = index;
        }

        public static int Distance(City a, City b)
        {
            int dx = a.Position.X - b.Position.X;
            int dy = a.Position.Y - b.Position.Y;
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}