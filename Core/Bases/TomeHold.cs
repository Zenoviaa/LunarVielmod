using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    /// <summary>
    /// Projectile for a magic tomes holding animation, it doesn't do anything surrounding magic consumption and whatnot
    /// </summary>
    public class TomeHold : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private ref float DeathTimer => ref Projectile.ai[1];
        private Player Owner => Main.player[Projectile.owner];
        private AbstractMagicTome _heldTome;
        private AbstractMagicTome HeldTome
        {
            get
            {
                if(Owner.HeldItem.ModItem is AbstractMagicTome tome)
                {
                    _heldTome = tome;
                }
                return _heldTome;
            }
        }
        private float HoldDistance => 48;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.01f;
            Projectile.timeLeft = 100000;
        }

        public override void AI()
        {
            base.AI();
            
            //Do not run any of this code if we don't have a held tome
            if (HeldTome == null)
                return;

            Timer++;
            if(Timer % 15 == 0)
            {
                Color hintColor = HeldTome.GetTomeHintColor();
                DustParticle p = Particle<DustParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(8, 8), -Vector2.UnitY * Main.rand.NextFloat(2f, 4f), hintColor, Scale: Main.rand.NextFloat(0.3f, 1f));
                p.dampening = 0.1f;
                p.gravity = 0;
            }

            float scaleDuration = 60f;
            float progress = Timer / scaleDuration;
            float easedScaleProgress = EasingFunction.InOutExpo7(progress);
            float outScale = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(DeathTimer / 60f));
            float inScale = MathHelper.Lerp(0f, 1f, easedScaleProgress);
            Projectile.scale = inScale * outScale;



            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
            UpdatePlayerVisuals(player, rrp);
            AI_Channel();
            AI_MoveTowardsCursor();
        }
        private void AI_Channel()
        {
            //here we handle calculating when to attack!
            if (Main.myPlayer != Projectile.owner)
                return;

            Player player = Owner;
            // player.CheckMana returns true if the mana cost can be paid. Since the second argument is true, the mana is actually consumed.
            // If mana shouldn't consumed this frame, the || operator short-circuits its evaluation player.CheckMana never executes.
            bool manaIsAvailable = player.CheckMana(player.HeldItem.mana, false, false);

            // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
            // player.channel indicates whether the player is still holding down the mouse button to use the item.
            bool stillInUse = manaIsAvailable && !player.noItems && !player.CCed && player.controlUseItem;
            if (!stillInUse)
            {
                DeathTimer++;
                if(DeathTimer >= 60)
                    Projectile.Kill();
            }
            else
            {
                if (DeathTimer > 0)
                    DeathTimer--;
            }
        }
        private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {
            // The Prism is a holdout Projectile, so change the player's variables to reflect that.
            // Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
            int playerDir = Projectile.Center.X > player.Center.X ? 1 : -1;
            player.ChangeDir(playerDir);
          //  player.heldProj = Projectile.whoAmI;
           // player.itemTime = 2;
            //player.itemAnimation = 2;

            // If you do not multiply by Projectile.direction, the player's hand will point the wrong direction while facing left.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        private void AI_MoveTowardsCursor()
        {
            //Welp first thing we'll do is the channeling and cursor movement
            if (Main.myPlayer == Projectile.owner)
            {
                //Calculate where the heck we need to go
                Vector2 mouseWorld = Main.MouseWorld;
                Vector2 directionToMouse = (mouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                Vector2 velocityToMouse = directionToMouse * HoldDistance;
                Vector2 targetPosition = Owner.Center + velocityToMouse;

                Vector2 diffToPosition = targetPosition - Projectile.Center;
                Projectile.velocity = diffToPosition * 0.2f;

                //This should create very smooth movement of the tome
                float targetRotation = directionToMouse.ToRotation();
                float velocityRotationOffset = Projectile.velocity.Length() * 0.04f;
                Projectile.rotation = targetRotation + velocityRotationOffset;
                Projectile.netUpdate = true;
            }
        }

        private void DrawTomeSprite(ref Color lightColor)
        {
            if (HeldTome == null)
                return;

            Texture2D closeYourTomeTyrant = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-2, 2, MathUtil.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = closeYourTomeTyrant.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = Projectile.Center.X < Owner.Center.X ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float layerDepth = 0;
            //Actually draw it
            spriteBatch.Draw(closeYourTomeTyrant, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
        }

        private Color GetTrailColor(float ratio)
        {
            if (HeldTome == null)
                return Color.White;
            Color hintColor = HeldTome.GetTomeHintColor();
            return Color.Lerp(hintColor, Color.Transparent, ratio) * 0.3f;
        }

        private float GetTrailWidth(float ratio)
        {
            return MathHelper.SmoothStep(32, 0, ratio);
        }

        private void DrawPixelatedAura(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (HeldTome == null)
                return;

            Asset<Texture2D> auraTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Backglow");
            Vector2 drawOrigin = auraTextureAsset.Size() / 2f;
            Color drawColor = HeldTome.GetTomeHintColor();

            float drawRotation = Projectile.rotation;
            float drawScale = Projectile.scale;
            Vector2 scale = Vector2.One * drawScale;
            scale *= 1.3f;
            scale *= ExtraMath.Osc(0.8f, 1f);
            drawColor.A = 0;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            for(int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(auraTextureAsset.Value, drawPos, null, drawColor, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(auraTextureAsset.Value, drawPos, null, drawColor, Projectile.rotation, drawOrigin, scale * 0.5f, SpriteEffects.None, 0);
            }


            Asset<Texture2D> magicCircleTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/PentagramP2");

            Color manaCircleColor = drawColor;
            float manaCapacity = (float)Owner.statMana / (float)Owner.statManaMax2;
            manaCircleColor *= manaCapacity;
            spriteBatch.Draw(magicCircleTextureAsset.Value, Owner.Center - Main.screenPosition, null, manaCircleColor,  Main.GlobalTimeWrappedHourly * 0.4f, magicCircleTextureAsset.Size() /2f, scale * 0.4f, SpriteEffects.None, 0);
        }
        
        private void DrawPixelatedTomeTrail(GraphicsDevice graphicsDevice)
        {
            var shader = MagicNormalShader.Instance;
            shader.Speed = 1;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, offset: Projectile.Size / 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedAura, DrawLayer.OverNPCs);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTomeTrail, DrawLayer.OverNPCsAdditive);
            DrawTomeSprite(ref lightColor);
            return false;
        }
    }
}
