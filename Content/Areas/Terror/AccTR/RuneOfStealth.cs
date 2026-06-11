using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.AccTR
{
    public class RuneOfStealthGlow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            overPlayers.Add(index);
        }

        public override void AI()
        {
            base.AI();
            Player owner = Main.player[Projectile.owner];
            RuneOfStealthPlayer runeOfStealthPlayer = owner.GetModPlayer<RuneOfStealthPlayer>();
            if (runeOfStealthPlayer.stealthProgress > 0)
            {
                Projectile.timeLeft = 2;
            }

            Projectile.Center = owner.Center;
            if (runeOfStealthPlayer.hideVisual)
                return;
            Timer++;
            if (Timer % 32 == 0)
            {
                Dust.NewDust(owner.position, owner.width, owner.height, DustID.Firework_Red, Scale: 1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            RuneOfStealthPlayer runeOfStealthPlayer = owner.GetModPlayer<RuneOfStealthPlayer>();
            if (runeOfStealthPlayer.hideVisual)
                return false;

            float progress = runeOfStealthPlayer.stealthProgress;
            float scale = MathHelper.Lerp(0f, 1f, progress) + VectorHelper.Osc(0f, 0.1f, speed: 2);
            Color drawColor = Color.Lerp(Color.Transparent, Color.Red, progress);
  
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = drawColor * ExtraMath.Osc(0.7f, 1f, speed: 12) *  1f;
            drawer.color.A = 0;
            drawer.scale *= 1;
            drawer.scale.Y *= 1f;
            MagicBandShader bandShader = ShaderContent.GetInstance<MagicBandShader>();
            bandShader.Time = Main.GlobalTimeWrappedHourly * 2;

            SpriteBatch spriteBatch = Main.spriteBatch;

            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            glowDrawer.color = Color.Red * 0.2f;
            glowDrawer.color.A = 0;
            glowDrawer.scale = Vector2.One * scale * 0.5f;
            spriteBatch.Draw(glowDrawer);
            spriteBatch.Restart(effect: bandShader.Effect);
            drawer.worldPosition = owner.Center + new Vector2(0, owner.gfxOffY + 0);
            spriteBatch.Draw(drawer);


            spriteBatch.RestartDefaults();
            return false;
        }
    }

    public class RuneOfStealthPlayer : ModPlayer
    {
        public bool hasStealthRune;
        public bool hideVisual;
        public float stealthRuneTimer;
        public float stealthProgress => stealthRuneTimer / 900f;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasStealthRune = false;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (hasStealthRune)
            {
                stealthRuneTimer++;
            }
            else
            {
                stealthRuneTimer--;
            }
            stealthRuneTimer = MathHelper.Clamp(stealthRuneTimer, 0f, 900);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);
            stealthRuneTimer = 0f;
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            base.ModifyWeaponDamage(item, ref damage);
            if (hasStealthRune)
            {
                float progress = stealthRuneTimer / 900f;
                float maxDamageMultiplier = 1.15f;
                float damageMultiplier = MathHelper.Lerp(1f, maxDamageMultiplier, progress);
                damage *= damageMultiplier;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            float progress = stealthRuneTimer / 900f;
            float multiplier = MathHelper.Lerp(1f, 0.75f, progress);
            r *= multiplier;
            g *= multiplier;
            b *= multiplier;
            a *= multiplier;
        }
    }

    public class RuneOfStealth : BaseRune
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RuneOfStealthPlayer runeOfStealthPlayer = player.GetModPlayer<RuneOfStealthPlayer>();
            runeOfStealthPlayer.hasStealthRune = true;
            runeOfStealthPlayer.hideVisual = hideVisual;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RuneOfStealthGlow>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<RuneOfStealthGlow>(), 0, 0, player.whoAmI);
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(), material: ModContent.ItemType<TerrorFragments>());
        }
    }
}