using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Gores;

public class StarGongGore : ModGore
{
    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.numFrames = 7;
        gore.frame = (byte)Main.rand.Next(7);
        gore.timeLeft = 240;
    }

    public override bool Update(Gore gore)
    {
        gore.velocity.Y += 0.5f;
        gore.velocity.X *= 0.98f;
        gore.position += gore.velocity;
        gore.rotation = Utils.AngleLerp(gore.rotation, gore.velocity.ToRotation() - MathHelper.PiOver2, 0.03f);
        gore.timeLeft--;
        gore.alpha += 2;
        if (gore.timeLeft <= 0)
            gore.active = false;
        return false;
    }
}
