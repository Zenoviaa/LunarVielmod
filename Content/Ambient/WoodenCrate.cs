using Stellamod.Common.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Ambient;


public class WoodenCrate : ABreakableNPC
{
    protected ref float Frame => ref NPC.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Ambient.WoodenCrate.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 4;
    }
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        Frame = Main.rand.Next(2, 4);
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
            if(Frame < 2)
            {
                var rand = Main.rand.Next(2);
                var sound = AssetReferences.Assets.Sounds.RefreshNAmbience.BreakBox2.Asset;
                if (Main.rand.NextBool(2))
                {
                    sound = AssetReferences.Assets.Sounds.RefreshNAmbience.BreakBox3.Asset;
                }
                sound = sound with { PitchVariance = 0.6f };
                SoundEngine.PlaySound(sound, NPC.position);
            }
            else
            {
                var sound = AssetReferences.Assets.Sounds.RefreshNAmbience.BreakBox1.Asset;
                sound = sound with { PitchVariance = 0.6f };
                SoundEngine.PlaySound(sound, NPC.position);
            }

            //Spawn Gores
            for (var i = 0; i < 10; i++)
            {
                var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = (pos - NPC.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 7);
                vel.Y -= 5;
                Particles.WoodenCrate.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 90,
                });
            }

            foreach(PositionVelocity posVel in new CircleOrientation(NPC.Center, 32, 4))
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = posVel.position,
                    velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.SkyBlue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.4f, 0.9f))
                });
            }
        }
    }
}

public class WoodenCrateTileItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ambient.WoodenCrateTileItem.KEY;
    public override void SetDefaults()
    {
        // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
        Item.DefaultToPlaceableTile(ModContent.TileType<WoodenCrateTile>());
    }
}

public class WoodenCrateTile : ABreakableTile<WoodenCrateTileEntity>
{

}

public class WoodenCrateTileEntity : ABreakableSpawningNPCTileEntity<WoodenCrate, WoodenCrateTile>
{

}

public class WoodenCrateBig : WoodenCrate
{
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        Frame = Main.rand.Next(0, 2);
    }
}

public class WoodenCrateBigTileItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ambient.WoodenCrateBigTileItem.KEY;
    public override void SetDefaults()
    {
        // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
        Item.DefaultToPlaceableTile(ModContent.TileType<WoodenCrateBigTile>());
    }
}

public class WoodenCrateBigTile : ABreakableTile<WoodenCrateBigTileEntity>
{

}

public class WoodenCrateBigTileEntity : ABreakableSpawningNPCTileEntity<WoodenCrateBig, WoodenCrateBigTile>
{

}
