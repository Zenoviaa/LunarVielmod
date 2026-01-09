using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Stellamod.Items.Harvesting;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS
{
    public class SearingSawRemote : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            SearingSawPlayer sawPlayer = player.GetModPlayer<SearingSawPlayer>();
            sawPlayer.hasSearingSawRemote = true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankAccessory>();
        }
    }

    public class SearingSawPlayer : ModPlayer
    {
        public bool hasSearingSawRemote = false;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSearingSawRemote = true;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            SwingPlayerV2 swingPlayer = Player.GetModPlayer<SwingPlayerV2>();
            if (swingPlayer.useStaminaThisFrame)
            {
                if(Player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, -Vector2.UnitY * 15, ModContent.ProjectileType<SearingSaw>(), 20, 1, Player.whoAmI);
                }
            }
        }
    }

    public class SearingSaw : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle sawSound = new SoundStyle("Stellamod/Assets/Sounds/Saw1");
                sawSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(sawSound, Projectile.position);
            }

            if (Main.rand.NextBool(5))
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        DustParticle sp = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.3f, 16), Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        sp.gravity = 0f;
                        sp.fast = true;
                        sp.dampening = 0.1f;
                        break;
                    case 1:
                        FlameParticle sp2 = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 16), Scale: Main.rand.NextFloat(0.1f, 0.2f));
                        sp2.gravity = 0f;
                        sp2.fast = true;
                        sp2.dampening = 0.1f;
                        break;
                }

            }

            if (Main.rand.NextBool(8))
            {
                FlameSparksParticle sp = Particle<FlameSparksParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 8f),
                    color: Color.OrangeRed, Scale: Main.rand.NextFloat(0.35f, 0.75f));
                sp.gravity = 0f;
                sp.fast = true;
                sp.dampening = 0.1f;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            Projectile.rotation += 0.05f;
            NPC nearest = NPCHelper.FindClosestNPC(Projectile.position, 512);
            if (nearest == null)
                return;
            Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center);
            Projectile.velocity = homingVelocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sawTexture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = sawTexture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color drawColor = Color.Yellow;
            drawColor.A = 0;
            spriteBatch.Draw(sawTexture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);


            Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 glowDrawOrigin = glowMask.Size() / 2f;
            Color glowColor = Color.Lerp(Color.OrangeRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 8));
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.3f, SpriteEffects.None, 0);
            // spriteBatch.RestartDefaults();


            glowMask = AssetManager.GlowMask.SpiralVortex.Value;
            glowDrawOrigin = glowMask.Size() / 2f;
            glowColor = Color.Red;
            glowColor.A = 0;
            spriteBatch.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 0.6f, SpriteEffects.None, 0);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if(Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}
