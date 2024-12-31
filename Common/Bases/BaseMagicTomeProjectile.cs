using Stellamod.Systems.MiscellaneousMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public abstract class BaseMagicTomeProjectile : ModProjectile
    {
        private float _initialSpeed;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];

        public float AttackRate { get; set; }
        public float ManaConsumptionRate { get; set; }
        public float HoldDistance { get; set; }
        public float GlowRotationSpeed { get; set; }
        public float GlowDistanceOffset { get; set; }


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            // Signals to Terraria that this Projectile requires a unique identifier beyond its index in the Projectile array.
            // This prevents the issue with the vanilla Last Prism where the beams are invisible in multiplayer.
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
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
            Projectile.scale = 0.01f;

            //This thing is channeled so it'll last forever
            Projectile.timeLeft = int.MaxValue;


            AttackRate = 12;
            ManaConsumptionRate = 4;
            HoldDistance = 36;
            GlowDistanceOffset = 4;
            GlowRotationSpeed = 0.05f;
        }

        public override void AI()
        {
            base.AI();

            /*
            Alright so how does this magic tome thing work
            Well there are a few things we need to do here
            0. The tome is going to hover in front of you towards your cursor,
            1. All tomes are channeled :P
            2. So we probably want a base magic tome item too
            3. It'll glow, which we can do with post draw.
            4. I think a cool glowy outline effect would be interesting, we'd need a shader for that with additive drawing
            5. Glowy particles and maybe a magic circle? Not sure if we need the magic circle or not, but zemmie can help with it if we do
            6. Mana consumption would be on the base item, check example mod for how to setup channeling items
            

            */

            Timer++;

            //Scale in animation when summoining
            float scaleDuration = 60f;
            if (Timer <= scaleDuration)
            {
                float progress = Timer / scaleDuration;
                float easedScaleProgress = Easing.InOutCubic(progress);
                Projectile.scale = MathHelper.Lerp(0f, 1f, easedScaleProgress);
            }

            if (Timer == 1)
            {
                _initialSpeed = Projectile.velocity.Length();
            }
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
            UpdatePlayerVisuals(player, rrp);
            AI_MoveTowardsCursor();
            AI_Attack();
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
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, diffToPosition, 0.2f);

                //This should create very smooth movement of the tome
                float targetRotation = directionToMouse.ToRotation();
                float velocityRotationOffset = Projectile.velocity.Length() * 0.04f;
                Projectile.rotation = targetRotation + velocityRotationOffset;
                Projectile.netUpdate = true;
            }
        }

        private bool ShouldConsumeMana()
        {
            return Timer % ManaConsumptionRate == 0;
        }

        private void AI_Attack()
        {
            //here we handle calculating when to attack!
            if (Main.myPlayer != Projectile.owner)
                return;

            Player player = Owner;
            UpdateDamageForManaSickness(player);
            // player.CheckMana returns true if the mana cost can be paid. Since the second argument is true, the mana is actually consumed.
            // If mana shouldn't consumed this frame, the || operator short-circuits its evaluation player.CheckMana never executes.
            bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);

            // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
            // player.channel indicates whether the player is still holding down the mouse button to use the item.
            bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;

            // Spawn in the Prism's lasers on the first frame if the player is capable of using the item.
            if (stillInUse && Timer % AttackRate == 0)
            {
                Vector2 mouseWorld = Main.MouseWorld;
                Vector2 directionToMouse = (mouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                Vector2 velocity = directionToMouse * Owner.HeldItem.shootSpeed;
                Shoot(Owner, Projectile.GetSource_FromThis(), Projectile.Center, velocity, Projectile.damage, Projectile.knockBack);
            }

            // If the Prism cannot continue to be used, then destroy it immediately.
            else if (!stillInUse)
            {
                Projectile.Kill();
            }
        }
        private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {
            // The Prism is a holdout Projectile, so change the player's variables to reflect that.
            // Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
            int playerDir = Projectile.Center.X > player.Center.X ? 1 : -1;
            player.ChangeDir(playerDir);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // If you do not multiply by Projectile.direction, the player's hand will point the wrong direction while facing left.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        private void UpdateDamageForManaSickness(Player player)
        {
            Projectile.damage = (int)player.GetDamage(DamageClass.Magic).ApplyTo(player.HeldItem.damage);
        }
        protected virtual void Shoot(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int damage, float knockback)
        {

        }

        private float DefaultWidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(12, 0, completionRatio);
        }

        private Color DefaultColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p);
            return trailColor;
        }

        protected virtual void DrawTrail(ref Color lightColor)
        {
            //Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            LightningBolt2Shader lightningShader = LightningBolt2Shader.Instance;
            lightningShader.PrimaryColor = Color.White;
            lightningShader.NoiseColor = new Color(120, 215, 255);
            lightningShader.Speed = 5;
            TrailDrawer.Draw(spriteBatch, Projectile.oldPos, Projectile.oldRot, DefaultColorFunction, DefaultWidthFunction, lightningShader, offset: Projectile.Size / 2f);
        }

        protected virtual void DrawTomeSprite(ref Color lightColor)
        {
            Texture2D closeYourTomeTyrant = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-5, 5, MathUtil.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = closeYourTomeTyrant.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = Projectile.Center.X < Owner.Center.X ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float layerDepth = 0;


            //Draw Glow Effects
            //Let's do some additive glow
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (float f = 0; f < 1; f += 0.2f)
            {
                float rotation = (f * MathHelper.TwoPi) + Timer * GlowRotationSpeed;
                Vector2 velocityRot = rotation.ToRotationVector2();
                velocityRot *= GlowDistanceOffset;

                Vector2 glowDrawPos = drawPos + velocityRot;
                spriteBatch.Draw(closeYourTomeTyrant, glowDrawPos, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
            }
            spriteBatch.End();
            spriteBatch.Begin();


            //Actually draw it
            spriteBatch.Draw(closeYourTomeTyrant, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail(ref lightColor);
            DrawTomeSprite(ref lightColor);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }
    }
}
