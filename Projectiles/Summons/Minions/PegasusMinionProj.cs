using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    internal class PegasusMinionProj : ModProjectile
    {
        private enum ActionState
        {
            Frost,
            Stars,
            Lightning
        }

        private ActionState State
        {
            get => (ActionState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private ref float Timer => ref Projectile.ai[1];
        private ref float RotTimer => ref Projectile.ai[2];
        private float WhiteTimer = 0f;
        private IEntitySource EntitySource => Projectile.GetSource_FromThis();
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.scale = 1f;
        }


        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return true;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            switch (State)
            {
                default:
                case ActionState.Frost:
                    return Color.Lerp(Color.LightCyan, Color.Transparent, completionRatio) * 0.7f;
                case ActionState.Stars:
                    return Color.Lerp(Color.Blue, Color.Transparent, completionRatio) * 0.7f;
                case ActionState.Lightning:
                    return Color.Lerp(Color.DarkGoldenrod, Color.Transparent, completionRatio) * 0.7f;
            }
        }

        private string GetTexturePath()
        {
            switch (State)
            {
                default:
                case ActionState.Frost:
                    return $"{Texture}_Frost";
                case ActionState.Lightning:
                    return $"{Texture}_Lightning";
                case ActionState.Stars:
                    return $"{Texture}_Stars";
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            string texturePath = GetTexturePath();
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            TrailDrawer.DrawPrims(Projectile.oldPos, texture.Size() * 0.5f - Main.screenPosition, 155);

            Vector2 drawPosition = Projectile.position + texture.Size() * 0.5f - Main.screenPosition;
            Rectangle? sourceRectangle = null;
            Color drawColor = Color.White;
            float drawRotation = Projectile.rotation;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            float drawScale = Projectile.scale;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition, sourceRectangle, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var shader = ShaderRegistry.MiscSilPixelShader;

            //The color to lerp to
            shader.UseColor(Color.White);

            //Should be between 0-1
            //1 being fully opaque
            //0 being the original color
            if (WhiteTimer <= 0)
                WhiteTimer = 0f;
            shader.UseSaturation(WhiteTimer);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            string texturePath = GetTexturePath();
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPosition = Projectile.position + (texture.Size() * 0.5f) - Main.screenPosition;
            Rectangle? sourceRectangle = null;
            Color drawColor = Color.White;
            float drawRotation = Projectile.rotation;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            float drawScale = Projectile.scale;

            spriteBatch.Draw(texture, drawPosition, sourceRectangle, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin();
        }

        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<PegasusMinionBuff>(Owner, Projectile))
                return;

            WhiteTimer -= 0.01f;
            AI_Movement();
            switch (State)
            {
                case ActionState.Frost:
                    AI_Frost();
                    break;
                case ActionState.Stars:
                    AI_Stars();
                    break;
                case ActionState.Lightning:
                    AI_Lightning();
                    break;
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        private void AI_Movement()
        {
            RotTimer++;
            float offset = (MathHelper.TwoPi / 3f) * (float)State;
            float circleDistance = 128;
            Vector2 circlePosition = Owner.Center + new Vector2(circleDistance, 0)
                .RotatedBy(offset + RotTimer * 0.02f);

            //Oscillate movement
           // float ySpeed = MathF.Sin(offset + RotTimer * 0.05f);
           // circlePosition.Y += ySpeed;
            Projectile.Center = circlePosition;
        }

        private void AI_Frost()
        {
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if (!foundTarget)
                return;
            Timer++;
            if(Timer >= 30)
            {
                WhiteTimer = 1f;
                Vector2 velocity = Projectile.Center.DirectionTo(targetCenter) * 24;
                Projectile.NewProjectile(EntitySource, Projectile.Center, velocity, 
                    ModContent.ProjectileType<PegasusMinionFrostBombProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                SoundStyle soundStyle = SoundRegistry.IceyWind;
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                Timer = 0;
            }
        }

        private void AI_Lightning()
        {
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if (!foundTarget)
                return;
            Timer++;
            if (Timer >= 240)
            {
                WhiteTimer = 1f;
                Vector2 velocity = Projectile.Center.DirectionTo(targetCenter) * 96;
                Projectile.NewProjectile(EntitySource, Projectile.Center, velocity,
                    ModContent.ProjectileType<PegasusMinionLightningProj>(), Projectile.damage * 8, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(EntitySource, Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4) * 0.5f,
                   ModContent.ProjectileType<PegasusMinionLightningProj>(), Projectile.damage * 8, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(EntitySource, Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4) * 0.5f,
                   ModContent.ProjectileType<PegasusMinionLightningProj>(), Projectile.damage * 8, Projectile.knockBack, Projectile.owner);
                
                SoundStyle soundStyle = SoundRegistry.Lightning2;
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                Timer = 0;
            }
        }

        private void AI_Stars()
        {
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if (!foundTarget)
                return;
            Timer++;
            if (Timer >= 60 && Timer % 8 == 0)
            {
                WhiteTimer = 1f;
                Vector2 velocity = Projectile.Center.DirectionTo(targetCenter) * 15;
                Projectile.NewProjectile(EntitySource, Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4 / 3),
                    ModContent.ProjectileType<PegasusMinionStarProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            if(Timer >= 120)
            {
                Timer = 0;
            }
        }
    }
}
