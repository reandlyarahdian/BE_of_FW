namespace Watermelon
{
    [System.Serializable]
    public class LevelSave : ISaveObject
    {
        public int MaxReachedLevelIndex = 0;

        public int RealLevelIndex = 0;
        public int DisplayLevelIndex = 0;
        public bool IsPlayingRandomLevel = false;

        public int LastPlayerLevelIndex = -1;
        
        public void Flush()
        {

        }

        public void Init(int value)
        {
            MaxReachedLevelIndex = value;
            RealLevelIndex = value;
            DisplayLevelIndex = value;
            IsPlayingRandomLevel = false;
            LastPlayerLevelIndex = value - 1;
        }
    }
}
