using ReLogic.Content;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class LeafGliderDrawLayer : PlayerDrawLayer
{

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        //     Main.NewText(drawInfo.drawPlayer.body);
        return drawInfo.drawPlayer.GetModPlayer<LeafGliderPlayer>().holdingOutTimer > 0;

    }

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.BackAcc);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        float holdingOutTimer = drawInfo.drawPlayer.GetModPlayer<LeafGliderPlayer>().holdingOutTimer;
        if (holdingOutTimer <= 0)
            return;

        Asset<Texture2D> wingsTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/LeafGlider_Hold");
        float ease = EasingFunction.InOutSine(holdingOutTimer / 30f);
        Color wingColor = Color.White;
        float rotation = ExtraMath.Osc(-0.05f, 0.05f);

        var drawData = new DrawData(
            wingsTextureAsset.Value,
            drawInfo.drawPlayer.Center - Main.screenPosition + new Vector2(0, -16),
            null,
            wingColor,
            rotation,
            wingsTextureAsset.Size() * new Vector2(0.5f, 1f),
            ease,
            drawInfo.playerEffect
        );
        drawData.shader = drawInfo.cHead;
        drawInfo.DrawDataCache.Add(drawData);
    }
}

public class LeafGliderPlayer : ModPlayer
{
    public bool hasLeafGlider;
    public bool holdingOut;
    public float holdingOutTimer;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasLeafGlider = false;
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        holdingOutTimer += holdingOut ? 1 : -1;
        holdingOutTimer = MathHelper.Clamp(holdingOutTimer, 0, 30);
        if (!hasLeafGlider)
        {
            holdingOut = false;
            return;
        }

        int fallAmount = 25;
        fallAmount += Player.extraFall;
        int fallDistance = (int)(Player.position.Y / 16f) - Player.fallStart;
        if (((Player.gravDir == 1f && fallDistance > fallAmount) || (Player.gravDir == -1f && fallDistance < -fallAmount)))
        {
            holdingOut = true;
        }

        if (Player.velocity.Y == 0)
            holdingOut = false;
    }

    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if (holdingOut)
        {
            Player.velocity.Y *= 0.8f;
            Player.slowFall = true;
        }
    }
}

public class LeafGlider : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<LeafGliderPlayer>().hasLeafGlider = true;

    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankAccessory>();
    }
}
