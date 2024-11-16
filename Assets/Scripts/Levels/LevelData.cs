[System.Serializable]
public struct LevelData
{
    public ushort seed;
    public ushort width;
    public ushort height;
    public float minCompletionTime;
    public int minSteps;

    public LevelData(ushort width, ushort height, ushort seed)
    {
        this.seed = seed;
        this.width = width;
        this.height = height;
        minCompletionTime = float.MaxValue;
        minSteps = int.MaxValue;
    }
    
    public void UpdateCompletionStats(float time, int steps)
    {
        if (time < minCompletionTime) minCompletionTime = time;
        if (steps < minSteps) minSteps = steps;
    }

    public string Share()
    {
        // to whoever is reading this, i'm sorry for ruining your day
        return $"{width.ToString().PadLeft(5, '0')}{height.ToString().PadLeft(5, '0')}{seed.ToString().PadLeft(5, '0')}";
    }
}