namespace TopDownHordeShooter.Utils
{
    /*
     * Singleton class pattern with lock to ensure for thread-safety
     * when multiple enemies are generated simultaneously so that IDs
     * are generated correctly regardless.
     */ 
    public sealed class HitboxCounter
    {
        private static HitboxCounter _counterInstance;
        private static readonly object Lock = new object();
        public int CurrentCounter { get; private set; }

        static HitboxCounter(){ }
        private HitboxCounter(){ }

        public static HitboxCounter GetInstance()
        {
            //if (counterInstance != null) return counterInstance;
            
            lock (Lock)
            {
                if (_counterInstance == null) _counterInstance = new HitboxCounter {CurrentCounter = 0};
                else _counterInstance.CurrentCounter++;
            }
            return _counterInstance;
        }
    }
}