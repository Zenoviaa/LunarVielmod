using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles.Magic;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.DelgrimsHammer
{
    public class DelgrimsHammerProj : ModProjectile
    {
        public static bool swung = false;

        private bool _initialized;
        private float _hitCount;
        private int timer;

        //Swing Stats
        public float SwingDistance;
        private int SwingTime => (int)((50 * Swing_Speed_Multiplier) / Owner.GetAttackSpeed(DamageClass.Melee));
        public float holdOffset = 60f;

        //Ending Swing Time so it doesn't immediately go away after the swing ends, makes it look cleaner I think
        public int EndSwingTime = 4 * Swing_Speed_Multiplier;

        //This is for smoothin the trail
        public const int Swing_Speed_Multiplier = 8;
        public int BounceTimer;
        public int BounceDelay;
        private Player Owner => Main.player[Projectile.owner];

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BounceTimer);
            writer.Write(BounceDelay);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BounceTimer = reader.ReadInt32();
            BounceDelay = reader.ReadInt32();
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 100;
            Projectile.width = 100;
            Projectile.friendly = true;
            Projectile.scale = 1f;

            //This extra updates thing basically lets us put more frames inside a single frame
            //Updates multiple times per frame
            //That's how the trail is smoother, it does affect other things though, so make sure to factor it in to timers
            Projectile.extraUpdates = Swing_Speed_Multiplier - 1;
            Projectile.usesLocalNPCImmunity = true;

            //Multiplying by the thing so it's still 10 ticks
            Projectile.localNPCHitCooldown = 10 * Swing_Speed_Multiplier;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public virtual float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }

        public override bool? CanDamage()
        {            
            //Get the swing progress
            float lerpValue = Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft, true);

            //Smooth it some more
            float swingProgress = Easing.InOutBack(lerpValue);
            return swingProgress > 0.33f && swingProgress < 0.75f;
        }
        public override void AI()
        {
            Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
            float multiplier = 0.2f;
            RGB *= multiplier;

            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            Swing();
        }

        private void Swing()
        {
            if(BounceDelay > 0)
            {
                BounceDelay--;
            }
            else
            {
                BounceTimer--;
                if (BounceTimer > 0)
                {
                    Projectile.timeLeft += 2;
                }
            }

            Player player = Main.player[Projectile.owner];
            int dir = Projectile.velocity.X < 0 ? -1 : 1;

            //Get the swing progress
            float lerpValue = Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft, true);

            //Smooth it some more
            float swingProgress = Easing.InOutBack(lerpValue);

            // the actual rotation it should have
            float defRot = Projectile.velocity.ToRotation();
            // starting rotation

            //How wide is the swing, in radians
            float swingRange = MathHelper.PiOver2 + MathHelper.PiOver4;
            float start = defRot - swingRange;

            // ending rotation
            float end = (defRot + swingRange);

            // current rotation obv
            // angle lerp causes some weird things here, so just use a normal lerp
            float rotation = dir == 1 ? MathHelper.Lerp(end, start, swingProgress) : MathHelper.Lerp(start, end, swingProgress);

            // offsetted cuz sword sprite
            Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
            position += rotation.ToRotationVector2() * holdOffset;
            Projectile.Center = position;
            Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = rotation * player.direction;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public override bool ShouldUpdatePosition() => false;
        public bool bounced = false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 oldMouseWorld = Main.MouseWorld;

            _hitCount++;
            float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
            SoundStyle jugglerHit = SoundRegistry.JugglerHit;
            jugglerHit.Pitch = pitch;
            jugglerHit.PitchVariance = 0.1f;
            jugglerHit.Volume = 0.5f;
            SoundEngine.PlaySound(jugglerHit, Projectile.position);

            if (_hitCount >= 7)
            {
                SoundStyle jugglerHitMax = SoundRegistry.JugglerHitMax;
                pitch = MathHelper.Clamp(_hitCount * 0.02f, 0f, 1f);
                jugglerHitMax.Pitch = pitch;
                jugglerHitMax.PitchVariance = 0.1f;
                SoundEngine.PlaySound(jugglerHitMax, Projectile.position);
            }

            for (int i = 0; i < 8; i++)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(target.Center, velocity, Color.DarkGoldenrod, randScale);
            }

            if (BounceTimer <= 0)
            {
                if(Main.myPlayer == player.whoAmI)
                {
                    player.velocity = Projectile.DirectionTo(oldMouseWorld) * -2f;
                }
 
                BounceTimer = 10 * Swing_Speed_Multiplier;
                BounceDelay = 2 * Swing_Speed_Multiplier;
                Projectile.netUpdate = true;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 16f);
            }
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 1.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Turquoise, Color.Transparent, completionRatio) * 0.7f;
        }

        public TrailRenderer SwordSlash;
        public TrailRenderer SwordSlash2;
        public TrailRenderer SwordSlash3;
        public TrailRenderer SwordSlash4;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();

            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SpikyTrail1").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/SpikyTrail2").Value;
            var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/StarTrail").Value;
            var TrailTex4 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/TerraTrail").Value;
            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);

            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, 
                    (p) => Vector2.Lerp(new Vector2(50), new Vector2(40), Easing.InCirc(p)), 
                    (p) => new Color(0, 2, 30, 50) * (1f - p));
                SwordSlash.drawOffset = Projectile.Size / 2f;
            }

            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass,
                   (p) => Vector2.Lerp(new Vector2(50), new Vector2(40), Easing.InCirc(p)), 
                    (p) => new Color(50, 15, 250, 4) * (1f - p));
                SwordSlash2.drawOffset = Projectile.Size / 2f;
            }

            if (SwordSlash3 == null)
            {
                SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass,
                    (p) => Vector2.Lerp(new Vector2(100), new Vector2(75), Easing.InCirc(p)), 
                    (p) => new Color(81, 21, 31, 100) * (1f - p));
                SwordSlash3.drawOffset = Projectile.Size / 2f;
            }

            if (SwordSlash4 == null)
            {
                SwordSlash4 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass,
                    (p) => Vector2.Lerp(new Vector2(80), new Vector2(60), Easing.InCirc(p)), 
                    (p) => new Color(105, 105, 105, 5) * (1f - p));
                SwordSlash4.drawOffset = Projectile.Size / 2f;

            }
            Main.spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);


            float[] rotation = new float[Projectile.oldRot.Length];
            for (int i = 0; i < rotation.Length; i++)
            {
                rotation[i] = Projectile.oldRot[i] - MathHelper.ToRadians(45);
            }


            SwordSlash.Draw(Projectile.oldPos, rotation);
            SwordSlash2.Draw(Projectile.oldPos, rotation);
            SwordSlash3.Draw(Projectile.oldPos, rotation);
            SwordSlash4.Draw(Projectile.oldPos, rotation);



            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);


            Main.EntitySpriteDraw(texture,
               Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
               sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itself
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture2 = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            Main.spriteBatch.End();

            Main.spriteBatch.Begin();


            return false;

        }

        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            float mult = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            Vector2 pos = player.Center + Projectile.velocity * (mult);

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            float rotation = Projectile.rotation;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 Dorigin = sourceRectangle.Size() / 2f;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Dorigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k / 0.2f));
                Main.EntitySpriteDraw(texture, drawPos, null, color, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return;
        }
    }
}