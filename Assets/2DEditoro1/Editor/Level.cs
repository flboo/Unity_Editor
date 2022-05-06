using System.Collections.Generic;

public enum LimitType
{
    Moves,
    Time
}

public class Level
{
    public Dictionary<BoosterType, bool> availableBoosters = new();
    public List<ColorBlockType> availableColors = new();

    public BoosterType awardedBoosterType;
    public int collectableChance;
    public int height;
    public int id;

    public int limit;

    public LimitType limitType;

    public int penalty;

    public bool qwardBoosterWithRemaningMoves;

    public int score1;
    public int score2;
    public int score3;

    public List<LevelTile> tiles = new();
    public int width;
}