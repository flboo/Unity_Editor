public class LevelTile
{
    public BlockerType blockerType;
}

public class BlockTile : LevelTile
{
    public BlockType type;
}

public class BoosterTile : LevelTile
{
    public BoosterType type;
}


public enum BlockerType
{
    None,
    Ice
}

public enum BlockType
{
    Block1,
    Block2,
    Block3,
    Block4,
    Block5,
    Block6,
    RandomBlock,
    Ball,
    Stone,
    Collectable,
    Empty
}

public enum BoosterType
{
    HorizontalBomb,
    VerticalBomb,
    Gynamite,
    ColorBomb
}

public enum ColorBlockType
{
    ColorBlock1,
    ColorBlock2,
    ColorBlock3,
    ColorBlock4,
    ColorBlock5,
    ColorBlock6
}