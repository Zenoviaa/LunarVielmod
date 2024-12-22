using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Stellamod.Projectiles.Slashers.Swords;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.FrostBringer
{
    internal class FrostBringerSlash : BaseSwingProjectile
    {
        private bool _hasFired;
        public override string Texture => "Stellamod/Items/Weapons/Melee/FrostBringer";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 64;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            holdOffset = 40;
            trailStartOffset = 0.2f;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 38;
            Projectile.width = 38;
            Projectile.friendly = true;
            Projectile.scale = 1f;

            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        public override void SetSwingStyle(ref BaseSwingStyle swingStyle, int comboIndex)
        {
            base.SetSwingStyle(ref swingStyle, comboIndex);
            switch (comboIndex)
            {
                case 0:
                    SoundStyle swingSound = new SoundStyle("Stellamod/Assets/Sounds/NormalSwordSlash2");
                    swingSound.PitchVariance = 0.13f;
                    swingStyle = new OvalSwingStyle
                    {
                        swingTime = 18,
                        swingXRadius = 128 / 1.5f,
                        swingYRadius = 64 / 1.5f,
                        swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4,
                        easingFunc = (float lerpValue) => Easing.InOutExpo(lerpValue, 5),
                        swingSound = swingSound,
                        swingSoundLerpValue = 0.5f
                    };
                    break;
            }
        }

        public override void AI()
        {
            base.AI();
            if (_smoothedLerpValue >= 0.4f)
            {
                if (!_hasFired && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                        ModContent.ProjectileType<FrostWave>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    _hasFired = true;
                }
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float t = Timer / 60f;
            t = MathHelper.Clamp(t, 0f, 1f);
            return MathHelper.Lerp(0f, 312, Easing.SpikeOutCirc(completionRatio) * t);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.Lerp(Color.White, Color.Blue, Easing.SpikeInExpo(completionRatio)), Easing.SpikeOutCirc(completionRatio) * Timer / 60f);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected override void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {
            base.DrawSlashTrail(trailPoints, drawOffset);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.SimpleTrail);
            TrailDrawer.DrawPrims(trailPoints, drawOffset, 155);
        }
    }
}
