using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.WitchySystem;

public class PotionHealthPlayer : ModPlayer
{
    public Color bloodColor;
    public Color darkBloodColor;
    public float hueShift;
    public float maxReserveLife;
    public float aggression;
    public float DistSquared => 256 * 256;
    public static float hurtTimer;
    public override void ResetEffects()
    {
        base.ResetEffects();
        bloodColor = Color.Red;
        darkBloodColor = Color.DarkRed;
        hueShift = 0;
        maxReserveLife = Player.ConsumedLifeFruit * 5;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);

    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (hurtTimer > 0)
            hurtTimer--;
        return;

        var npc = NPCHelper.FindClosestNPC(Player.Center, 2048);
        if(npc != null)
        {
            var distToNpcSquared = Vector2.DistanceSquared(Player.Center, npc.Center);
            if(distToNpcSquared > DistSquared)
            {
                LoseAggression();
            }
        }
        else
        {
            LoseAggression();
        }
        if(aggression >= maxReserveLife)
        {
            aggression = 0;
            Player.Heal((int)maxReserveLife);
        }
    }

    void LoseAggression()
    {
        aggression--;
        if (aggression <= 0)
            aggression = 0;
    }
    public override void PostHurt(Player.HurtInfo info)
    {
        base.PostHurt(info);
        hurtTimer = 30;
    }
}
