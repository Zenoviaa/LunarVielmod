using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Scrolls;


public enum ScrollAbility : byte
{
    _None,
    _ACT_1,
    Enrager,
    Anger,
    Flame,
    Poison,
    Endurance,
    SimpleHome,
    SimpleFireball,
    SimpleConstellation,
    SimpleMeteor,
    SimpleBolt,
    SimpleSpikeball,
    SimpleSpore,
    SimpleRadiance,
    SimplePrismabolts,
    SimpleGust,
    Hookspot,
    Lighting,
    _ACT_2,
    MyScarab,
    _ACT_3,
    _Length,
}

public class Anger : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.GetDamage(DamageClass.Generic) += 0.05f * player.GetModPlayer<ScrollBuffPlayer>().angerStacks;

        if (Main.rand.NextBool(14))
        {
            var dp = DustParticle.Spawn(player.Center + Main.rand.NextVector2Circular(24, 24), -(Vector2.UnitY * Main.rand.NextFloat(1f, 5f)).RotatedByRandom(MathHelper.ToRadians(45)));
            //        dp.gravity = 0.05f;
            dp.Scale *= 0.5f;
            dp.dampening = 0.05f;
            dp.innerColor = Color.Red;
        }
    }
}

public class Endurance : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.GetStats().generalEndurance += 0.1f * player.GetModPlayer<ScrollBuffPlayer>().enduranceStacks;
    }
}

public class Enrager : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.GetAttackSpeed(DamageClass.Generic) += 0.1f * player.GetModPlayer<ScrollBuffPlayer>().enrageStacks;
    }
}
public class Flame : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
    }
}
public class Poison : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
    }
}
public class SimpleWhiteHomingBolt : ModProjectile,
    IDrawToRenderTarget
{

    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private Vector2 RotationOrigin
    {
        get
        {
            return new Vector2(Projectile.ai[1], Projectile.ai[2]);
        }
        set
        {
            Projectile.ai[1] = value.X;
            Projectile.ai[2] = value.Y;
        }
    }
    private Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = 3;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.timeLeft = 300;
    }

    public override void AI()
    {
        base.AI();
        
        Timer++;
        if(Timer == 1)
        {
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1") with { PitchVariance = 0.5f, Volume = 0.3f };
            SoundEngine.PlaySound(shootSound, Projectile.position);
            RotationOrigin = Owner.Center;
        }

        if(Timer < 25)
        {
            Projectile.Center = Projectile.Center.RotatedBy(0.12f * MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / 25f)), RotationOrigin);
        }

        if (Timer > 25)
        {
            NPC nearest = NPCHelper.FindClosestNPC(Projectile.position, 1024);
            if (nearest != null)
            {
                Vector2 vel = (nearest.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                vel *= 15;
                Projectile.velocity = Projectile.velocity.MoveTowards(vel, 1f);
            }
        }

        if (Main.rand.NextBool(16))
        {
            var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(24, 24), Vector2.Zero, Color.White, Scale: 0.5f);
            sp.gravity = 0;
            sp.innerColor = Color.White;
            sp.outerColor = Color.DarkGray;
            sp.dampening = 0.05f;
            sp.noTileCollide = true;
        }
    }
    public override bool PreDraw(ref Color lightColor) => false;

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGray, Color.DarkGray, 12, 0.12f);
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.LightGray, Color.DarkGray, 18, 70);
        ShakeScreenPosition.Shake = 2;
        for (int i = 0; i < 7; i++)
        {
            var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(40, 40), -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.NextFloat(3f, 6f));
            dp.innerColor = Color.LightGray;
            dp.dampening = 0.08f;

        }

    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    private void DrawAfterImage(SpriteBatch sb, Vector2 screenPos)
    {
        float scale = EasingFunction.InOutSine(Timer / 30f);
        scale *= ExtraMath.Osc(0.3f, 1f, 0, Projectile.identity);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        for(int i =1; i < Projectile.oldPos.Length; i++)
        {
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            glowDrawer.worldPosition = pos;
            glowDrawer.color = Color.Lerp(Color.White, Color.Transparent, ratio) * 0.25f;
            glowDrawer.color.A = 0;
            glowDrawer.scale = Vector2.One * 0.1f * ExtraMath.Osc(0.75f, 1f, speed: 16, Projectile.identity) * scale;
            glowDrawer.rotation = (Projectile.oldPos[i - 1] - Projectile.oldPos[i]).ToRotation();
            sb.Draw(glowDrawer);
        }
        glowDrawer.worldPosition = Projectile.Center;
        glowDrawer.color = Color.White;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 2 * scale;
        glowDrawer.rotation = 0;
        sb.Draw(glowDrawer);

        Asset<Texture2D> innerTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow");
        glowDrawer = SpritebatchDrawer.FromTextureAsset(innerTexture, Projectile.Center);
        glowDrawer.color = Color.Black * 0.3f;
        glowDrawer.scale *= 0.24f * scale;
        sb.Draw(glowDrawer);
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawAfterImage);
        PixelationManager.QueuePrimitivesDrawAction(DrawWhiteHomingShot);
    }
    private Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio) * ExtraMath.Osc(0.7f, 1f, speed: 16, Projectile.identity) * 0.75f;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(12, 0, completionRatio);
    }

    private void DrawWhiteHomingShot(GraphicsDevice graphicsDevice)
    {
        var shader2 = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader2.LaserColor = Color.White * ExtraMath.Osc(0.7f, 1f, speed: 16, Projectile.identity);
        shader2.InnerColor = Color.DarkGray * 0.5f * ExtraMath.Osc(0.7f, 1f, speed: 16, Projectile.identity);
        shader2.OuterColor = Color.Blue * ExtraMath.Osc(0.7f, 1f, speed: 16, Projectile.identity);
        shader2.LaserTexture = AssetManager.LaserTextures.Aura;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader2, Projectile.Size * 0.5f);
    }
}
public class ScrollHelpExpandingTooltip : AbstractExpandingTooltip
{
    public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
    {
        if (item.ModItem is not ScrollItem scrollItem)
            return;
        TooltipLine line;
        line = new TooltipLine(Mod, "ScrollHelp", LangText.Common("ScrollHelp"));
        line.OverrideColor = Color.White;
        lines.Add(line);
    }
}
public class ScrollGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;
    public ScrollAbility scroll;
    public int StaminaCost => ScrollAbilities.GetStaminaCost(scroll);
    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }
    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        if (ScrollAbilities.IsEnchanting() && ScrollAbilities.IsApplicable(item))
        {
            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Main.screenPosition + position);
            glowDrawer.color = Color.Green * ExtraMath.Osc(0.5f, 1f, speed: 10) * ScrollAbilities.Alpha;
            glowDrawer.color.A = 0;
            glowDrawer.scale *= 0.2f;
            spriteBatch.Draw(glowDrawer);
        }
    }
    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return base.AppliesToEntity(entity, lateInstantiation);
    }

    public override void RightClick(Item item, Player player)
    {
        base.RightClick(item, player);
        if (!ScrollAbilities.IsEnchanting())
            return;
        if (!ScrollAbilities.IsApplicable(item))
            return;
        ScrollAbilities.ConsumeEnchantment(item);
    }

    public override bool CanRightClick(Item item)
    {
        if (ScrollAbilities.IsEnchanting() && ScrollAbilities.IsApplicable(item))
            return true;
        return base.CanRightClick(item);
    }

    public override bool ConsumeItem(Item item, Player player)
    {
        if (scroll != ScrollAbility._None)
            return false;
        return base.ConsumeItem(item, player);
    }
    public override bool AltFunctionUse(Item item, Player player)
    {
        if (scroll > 0)
        {
            return true;
        }
        return base.AltFunctionUse(item, player);
    }
    public override bool CanShoot(Item item, Player player)
    {
        if (scroll != ScrollAbility._None)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (dashPlayer.CanConsume(StaminaCost))
                {
                    dashPlayer.Consume(StaminaCost);
                }
                else
                {
                    return false;
                }

            }
        }
        return base.CanShoot(item, player);
    }
    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse == 2 && scroll != ScrollAbility._None)
        {
            ScrollAbilities.UseAbility(item, player, source, position, velocity, type, damage, knockback, scroll);
            return false;
        }

        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }
    public override void SaveData(Item item, TagCompound tag)
    {
        base.SaveData(item, tag);
        tag["scroll"] = (byte)scroll;
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        base.LoadData(item, tag);
        scroll = (ScrollAbility)tag.Get<byte>("scroll");
    }
}
