using Stellamod.Content.Areas.WaterSide.BossesWS;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide;

public class LeviathanHornBubblePlayer : ModPlayer
{
    public float bubbleTimer;
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (bubbleTimer <= 0)
            return;
        bubbleTimer--;
        if(bubbleTimer % 8 == 0)
        {
            Vector2 pos = Player.Center;
            pos.X += Player.direction * 24;
            pos += Main.rand.NextVector2Circular(32, 32);
            var bp = BubbleParticle.Spawn(pos, -Vector2.UnitY);
            bp.Scale *= Main.rand.NextFloat(0.3f, 0.6f);
            bp.gravity = 0;
        }
        if(bubbleTimer == 0)
        {
            NPCUtilities.SpawnNPCFromClient(Player.Center + new Vector2(-700, -700), ModContent.NPCType<LeviathanEel>());
        }
    }
}

public class LeviathanHorn : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.WaterSide.Items.LeviathanHorn.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.useTime = Item.useAnimation = 120;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = AssetReferences.Assets.Sounds.LeviathanEel.LeviBigSmokeHorn.Asset;
        Item.rare = ModContent.RarityType<ShopRarity>();
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(0, 16);
    }

  
    public override bool? UseItem(Player player)
    {
     //   NPCUtilities.SpawnNPCFromClient(player.Center + new Vector2(-700, -700), ModContent.NPCType<LeviathanEel>());
        player.GetModPlayer<LeviathanHornBubblePlayer>().bubbleTimer = 120;
        return true;
        //return base.UseItem(player);
    }
}
