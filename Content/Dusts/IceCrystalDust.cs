using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Dusts;

public class IceCrystalDust : ModDust
{
    public const int FRAME_WIDTH = 20;
    public const int FRAME_HEIGHT = 20;
    public const int FRAME_COUNT = 3;
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.scale = 1f;
        dust.frame = new Rectangle(0, FRAME_WIDTH * Main.rand.Next(FRAME_COUNT), FRAME_HEIGHT, FRAME_HEIGHT);
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        //  dust.velocity *= 0.92f;
        dust.rotation += 0.06f;
        dust.scale *= Main.rand.NextFloat(0.94f, 0.98f);
        if (dust.scale < 0.05f)
        {
            dust.active = false;
        }
        return false;
    }
}
