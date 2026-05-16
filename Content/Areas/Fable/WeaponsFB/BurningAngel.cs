using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class BurningAngel : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 12;
            Item.shoot = ModContent.ProjectileType<BurningAngelSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<BurningAngelProj>();
            meleeWeaponType = MeleeWeaponType.Hammer;
            staminaCost = 1;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankJuggler>(),
                material: ModContent.ItemType<AlcadizScrap>());
        }
    }








    public class BurningAngelSlash : BaseSwingProjectileV2
    {
        private float _hitCount;
        private bool _hit;
        private bool _playSound;
        public override void DefineCombo()
        {
            base.DefineCombo();
            BlackFireShader blackFireShader = new BlackFireShader();
            blackFireShader.SetDefaults();

            SlashTrailer devilsPeak = new SlashTrailer
            {
                Shader = blackFireShader,
                TrailWidthFunction = (interpolant) =>
                {
                    return EasingFunction.QuadraticBump(interpolant) * 32;
                },
                TrailColorFunction = (interpolant) =>
                {
                    Color lerp1 = Color.Lerp(Color.OrangeRed, Color.RosyBrown, interpolant);
                    return Color.Lerp(lerp1, Color.Transparent, EasingFunction.InExpo(interpolant));
                }

            };

            Trailer = devilsPeak;

            SwingV2Helper.AddHammerSwingStyle(this);
            useAfterImage = true;
            hitStopTime = 4 * EXTRA_UPDATE_COUNT;
        }


        public override void AI()
        {
            base.AI();
            glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.QuadraticBump(Interpolant));
            growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            _hitCount++;
            target.AddBuff(BuffID.OnFire, 60);
            float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
            SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);

            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                Bounce(8);
                FXUtil.ShakeCamera(target.Center, 1024, 16);
                FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.5f, 2, 30);
                _hit = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (!_hit)
            {
                modifiers.Knockback *= 0.5f;
            }
            else
            {
                modifiers.Knockback *= 2;
            }

            if (ComboIndex == ComboCount - 1)
            {
                modifiers.FinalDamage += 0.5f;
            }
        }
    }



    public class BurningAngelProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.height = 32;
            Projectile.width = 32;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 100;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotation = Projectile.rotation;
            Timer++;

            player.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation -= 0.5f;
            Projectile.velocity *= 0.97f;

            if (Timer < 30)
            {
                if (Main.myPlayer == Projectile.owner && player.controlUseItem)
                {
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 12;
                    Projectile.netUpdate = true;
                }

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemTime = 10;
                player.itemAnimation = 10;
                player.itemRotation = rotation * player.direction;
            }

            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);
            //Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(254, 231, 97), new Color(247, 118, 34), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
        }
    }
}












