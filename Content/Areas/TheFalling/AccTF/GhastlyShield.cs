using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.TheFalling.AccTF;

public class GhastlyShield : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToShield(ModContent.ProjectileType<GhastlyShieldHeld>());
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GhastlySpirit, BlankCard>();
    }
}

public class GhastlyShieldHeld : AbstractShieldProjectile
{
    public override void OnBlockMovement(NPC npc)
    {
        base.OnBlockMovement(npc);
        npc.AddBuff(ModContent.BuffType<GhastlyWeakness>(), 60);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);

        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        float outlineOffset = 2;
        Vector2 left = Vector2.UnitX * -outlineOffset;
        Vector2 right = Vector2.UnitX * outlineOffset;
        Vector2 up = Vector2.UnitY * -outlineOffset;
        Vector2 down = Vector2.UnitY * outlineOffset;
        SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (Projectile.Center.X < Owner.Center.X)
            spriteEffects |= SpriteEffects.FlipVertically;
        SpriteBatch spriteBatch = Main.spriteBatch;
        Rectangle drawFrame = Projectile.Frame();
        Vector2 drawOrigin = drawFrame.Size() / 2;
        float scale = Projectile.scale;
        scale *= ExtraMath.Osc(1.5f, 2f, speed: 3);
        float rotation = Projectile.rotation;
        Color drawColor = Color.White * 0.2f;
        drawColor *= ExtraMath.Osc(0.6f, 1f, speed: 3);
        spriteBatch.Draw(texture, drawPos, drawFrame, drawColor, rotation, drawOrigin, scale, spriteEffects, 0);

    }
}

public class GhastlyWeakness : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        if (Main.rand.NextBool(16))
        {
            Vector2 pos = npc.RandomPositionInNPCRect();
            var ms = MoonSpiralParticle.Spawn(pos, Vector2.Zero, Scale: 0.5f);
            ms.color = Color.GhostWhite;
            //ms.color *= 0.5f;
        }
    }
}

public class GhastlyShieldGlobalNPC : GlobalNPC
{
    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        base.ModifyIncomingHit(npc, ref modifiers);
        if (npc.HasBuff<GhastlyWeakness>())
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath with { Volume = 0.4f }, npc.Center);
            modifiers.FinalDamage *= 1.25f;
            FXUtil.ShakeCamera(npc.Center, 256, 4);
            for(int i = 0; i < 2; i++)
            {
                var dp = DustParticle.Spawn(npc.Center, Main.rand.NextVector2CircularEdge(64, 64));
                dp.noTileCollide = true;
                dp.dampening = 0.3f;
                dp.gravity = 0;
                dp.outerColor = Color.Blue;
            }
            for (int i = 0; i < 2; i++)
            {
                var dp = DustParticle.Spawn(npc.Center, Main.rand.NextVector2CircularEdge(48, 48));
                dp.noTileCollide = true;
                dp.dampening = 0.3f;
                dp.gravity = 0;
                dp.innerColor = Color.Red;
                dp.outerColor = Color.Blue;
            }
        }
    }
}