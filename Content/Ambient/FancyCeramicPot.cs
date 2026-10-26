using Stellamod.Common.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Ambient;


public class FancyCeramicPot : ABreakableNPC
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
        Frame = Main.rand.Next(5, 8);
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
            var min = 0;
            var max = 4;

            switch (Frame)
            {
                case 0:
                    min = 4;
                    max = 8;
                    break;
                case 1:
                    min = 8;
                    max = 12;
                    break;
                case 2:
                    min = 0;
                    max = 4;
                    break;
                case 3:
                    min = 8;
                    max = 12;
                    break;
            }
            //Spawn Gores
            for (var i = 0; i < 4; i++)
            {
                var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = (pos - NPC.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 7);
                Particles.CeramicPot.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 90,
                    frame = (byte)Main.rand.Next(min, max)
                });
            }
        }
    }
}

public class FancyCeramicPotTileItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ambient.FancyCeramicPotTileItem.KEY;
    public override void SetDefaults()
    {
        // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
        Item.DefaultToPlaceableTile(ModContent.TileType<FancyCeramicPotTile>());
    }
}

public class FancyCeramicPotTile : ABreakableTile<FancyCeramicPotTileEntity>
{

}

public class FancyCeramicPotTileEntity : ABreakableSpawningNPCTileEntity<FancyCeramicPot, FancyCeramicPotTile>
{

}

public class FancyCeramicPotBig : FancyCeramicPot
{
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        Frame = 4;
    }
}

public class FancyCeramicPotBigTileItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Ambient.FancyCeramicPotBigTileItem.KEY;
    public override void SetDefaults()
    {
        // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
        Item.DefaultToPlaceableTile(ModContent.TileType<FancyCeramicPotBigTile>());
    }
}

public class FancyCeramicPotBigTile : ABreakableTile<FancyCeramicPotBigTileEntity>
{

}

public class FancyCeramicPotBigTileEntity : ABreakableSpawningNPCTileEntity<FancyCeramicPotBig, FancyCeramicPotBigTile>
{

}