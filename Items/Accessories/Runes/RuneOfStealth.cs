using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfStealthGlow : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
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
            Timer++;
            if (Timer % 32 == 0)
            {
                Dust.NewDust(owner.position, owner.width, owner.height, DustID.Firework_Red, Scale: 0.2f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string texture = "Stellamod/Assets/NoiseTextures/ZuiEffect";
            Texture2D maskTexture = ModContent.Request<Texture2D>(texture).Value;
            Vector2 drawOrigin = maskTexture.Size() / 2;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //Lerping
            Player owner = Main.player[Projectile.owner];
            RuneOfStealthPlayer runeOfStealthPlayer = owner.GetModPlayer<RuneOfStealthPlayer>();
            float progress = runeOfStealthPlayer.stealthProgress;
            float scale = MathHelper.Lerp(0f, 1f, progress) + VectorHelper.Osc(0f, 0.1f, speed: 2);
            Color drawColor = Color.Lerp(Color.Transparent, Color.Red, progress);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(maskTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return base.PreDraw(ref lightColor);
        }
    }

    internal class RuneOfStealthPlayer : ModPlayer
    {
        public bool hasStealthRune;
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

    internal class RuneOfStealth : BaseRune
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