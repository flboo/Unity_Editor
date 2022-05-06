using System;

[Serializable]
public class GameConfig
{
    public int maxLives;
    public int timeToNextLife = 10;
    public int livesRefillCost = 20;
}