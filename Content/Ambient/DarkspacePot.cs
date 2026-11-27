using Stellamod.Common.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Stellamod.Content.Ambient;

public class DarkspacePot : ABreakableNPC
{
    protected ref float Frame => ref NPC.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Ambient.DarkspacePot.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 4;
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        Frame = Main.rand.Next(1, 4);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.VerticalFrame((int)Frame, Main.npcFrameCount[Type]);
        drawer.BottomCenterOrigin();
        drawer.worldPosition = NPC.Bottom;
        spriteBatch.Draw(drawer);
        return false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        //Spawn hit particles
        if (NPC.life <= 0)
        {
            SoundEngine.PlaySound(SoundID.Shatter with { PitchVariance = 0.6f }, NPC.position);
            //Spawn Gores
            for (var i = 0; i < 16; i++)
            {
                var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = (pos - NPC.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 7);
                vel.Y -= 5;
                PS.DarkspacePot.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 90,
                    frame = (byte)Main.rand.Next(4)
                });
            }
        }
    }
}
