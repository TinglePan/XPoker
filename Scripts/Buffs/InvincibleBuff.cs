namespace XCardGame;

public class InvincibleBuff: BaseBuff
{
    public InvincibleBuff(int amount) : base(
        "Invincible", $"Negates incoming attacks this round",
        "res://Sprites/BuffIcons/invincible.png", isTemporary:true)
    {
    }
}