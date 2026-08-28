using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Projectiles.Wings;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Wings;

public class GothinWingsDrawLayer : PlayerDrawLayer
{
    private Asset<Texture2D> _backWingsTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _backWingsTextureAsset = ModContent.Request<Texture2D>(ModContent.GetInstance<GothinWings>().Texture + "_BackWings");
    }
    public override void Unload()
    {
        base.Unload();
        _backWingsTextureAsset = null;
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.wings == ModContent.GetInstance<GothinWings>().Item.wingSlot;
    }



    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Backpacks);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var position = drawInfo.Center + new Vector2(0f, -4) - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y += ExtraMath.Osc(0f, 4f);
        position.Y -= 16;
        position.X -= drawInfo.drawPlayer.direction;

        GothinWingsPlayer wingsPlayer = drawInfo.drawPlayer.GetModPlayer<GothinWingsPlayer>();
        Rectangle srcRect = _backWingsTextureAsset.Value.GetFrame(wingsPlayer.frame, wingsPlayer.frameCount);

        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        var drawData = new DrawData(
            _backWingsTextureAsset.Value, 
            position,
            srcRect, 
            drawInfo.colorArmorBody, 
            0f, 
            srcRect.Size() * 0.5f,
            1f,
            spriteEffects,
            0
        );

        drawData.shader = drawInfo.cWings;
        drawInfo.DrawDataCache.Add(drawData);
    }
}

public class GothinWingsPlayer : ModPlayer
{
    public int frameCounter;
    public int frameCount;
    public int frame;
    public float frameSpeed;
    public override void ResetEffects()
    {
        base.ResetEffects();
        frameSpeed = 1;
        frameCount = 60;
    }
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();

        if (IsFlying())
        {
            frameCounter++;
            if (frameCounter >= frameSpeed)
            {
                frameCounter = 0;
                frame++;
                if (frame >= frameCount)
                {
                    frame = 0;
                }
            }
        }
        else
        {
            frame = 0;
        }
    }

    private bool IsFlying()
    {
        return Player.controlJump && !Player.mount.Active && Player.wingTime > 0;
    }

    private bool IsHovering()
    {
        return Player.controlDown && Player.controlJump && !Player.mount.Active && Player.wingTime > 0;
    }

}
[AutoloadEquip(EquipType.Wings)]
public class GothinWings : ModItem
{
    public override void SetStaticDefaults()
    {
        // These wings use the same values as the solar wings
        // Fly time: 180 ticks = 3 seconds
        // Fly speed: 9
        // Acceleration multiplier: 2.5
        ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(5400, 12f, 3);

    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = 1;
        Item.rare = ItemRarityID.LightRed;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        /*
        if (player.ownedProjectileCounts[ModContent.ProjectileType<GothinWingsProj>()] == 0)
        {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
       ModContent.ProjectileType<GothinWingsProj>(), 0, 0, player.whoAmI);
        }*/
    }

    public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
        ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
    {
        ascentWhenFalling = 0.85f; // Falling glide speed
        ascentWhenRising = 0.3f; // Rising speed
        maxCanAscendMultiplier = 1f;
        maxAscentMultiplier = 5f;
        constantAscend = 0.135f;
    }
}
