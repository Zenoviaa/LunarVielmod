using Stellamod.Common.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Ambient;

public class CeramicPot : ABreakableNPC
{
    protected ref float Frame => ref NPC.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Ambient.CeramicPot.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 8;
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
                Particles.CeramicPot.Spawn(new()
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

public class CeramicPotTileItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ambient.CeramicPotTileItem.KEY;
    public override void SetDefaults()
    {
        // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
        Item.DefaultToPlaceableTile(ModContent.TileType<CeramicPotTile>());
    }
}

public class CeramicPotTile : ABreakableTile<CeramicPotTileEntity>
{

}

public class CeramicPotTileEntity : ABreakableSpawningNPCTileEntity<CeramicPot, CeramicPotTile>
{

}


public class CeramicPotBig : CeramicPot
{
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        Frame = 0;
    }
}

public class CeramicPotBigTileItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ambient.CeramicPotBigTileItem.KEY;
    public override void SetDefaults()
    {
        // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
        Item.DefaultToPlaceableTile(ModContent.TileType<CeramicPotBigTile>());
    }
}

public class CeramicPotBigTile : ABreakableTile<CeramicPotBigTileEntity>
{

}

public class CeramicPotBigTileEntity : ABreakableSpawningNPCTileEntity<CeramicPotBig, CeramicPotBigTile>
{

}
