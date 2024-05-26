using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.SingularDive
{
    public class SingularDiveDive : ModProjectile
    {
        public float holdOffset = 60f;
        public int combowombo;

        private bool Bounced = false;
        private Player Owner => Main.player[Projectile.owner];

        //Swing Stats
        public float SwingDistance;
        public const int Swing_Speed_Multiplier = 8;
        private int SwingTime => (int)((360 * Swing_Speed_Multiplier) / Owner.GetAttackSpeed(DamageClass.Melee));

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
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
            Projectile.extraUpdates = Swing_Speed_Multiplier - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10 * Swing_Speed_Multiplier;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        int Grenber = 0;
        public virtual float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }

        public override void AI()
        {
            Grenber++;


            var EntitySource = Projectile.GetSource_Death();

            if (Grenber >= 60 * Swing_Speed_Multiplier)
             {
                for (int i = 0; i < 150; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, speed * 11, Scale: 3f);
                    d.noGravity = true;
                }

                Projectile.NewProjectile(EntitySource, Projectile.Center.X + Main.rand.Next(-10, 10), 
                    Projectile.Center.Y + Main.rand.Next(-10, 10), Main.rand.Next(-4, 5), Main.rand.Next(-4, 5), ModContent.ProjectileType<SingularOrb>(), Projectile.damage * 2, 1, 
                    Projectile.owner);
                Grenber = 0;
            }

            Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
            float multiplier = 0.2f;
            RGB *= multiplier;

            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);


            int dir = (int)Projectile.ai[1];
            float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft, true));
            // the actual rotation it should have
            float defRot = Projectile.velocity.ToRotation();
            // starting rotation
            float endSet = ((MathHelper.Pi) * 6 / 0.2f);
            float start = defRot - endSet;

            // ending rotation
            float end = (defRot + endSet);
            // current rotation obv
            float rotation = dir == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
            // offsetted cuz sword sprite
            Vector2 position = Owner.RotatedRelativePoint(Owner.MountedCenter);
            position += rotation.ToRotationVector2() * holdOffset;
            Projectile.Center = position;
            Projectile.rotation = (position - Owner.Center).ToRotation() + MathHelper.PiOver4;

            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            Owner.itemRotation = rotation * Owner.direction;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override bool ShouldUpdatePosition() => false;


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            Vector2 oldMouseWorld = Main.MouseWorld;
            if (!Bounced)
            {
                player.velocity = Projectile.DirectionTo(oldMouseWorld) * -10f;

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, DustID.IceTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                }

                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, DustID.IceTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                }

                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, DustID.IceTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
                }

                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, DustID.IceTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
                }

                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                for (int i = 0; i < 10; i++)
                {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch, 0f, -2f, 0, default, 1.1f);
                    Main.dust[num].noGravity = true;
                    Dust expr_62_cp_0 = Main.dust[num];
                    expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-30, 31) / 20 - 1.5f);
                    Dust expr_92_cp_0 = Main.dust[num];
                    expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-30, 31) / 20 - 1.5f);
                    if (Main.dust[num].position != Projectile.Center)
                    {
                        Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                    }
                }
                target.SimpleStrikeNPC(200, 1, crit: false, 1);
                Bounced = true;
               

            }
           
            if (target.lifeMax <= 800)
            {
                if (target.life < target.lifeMax / 2)
                {
                    target.SimpleStrikeNPC(9999, 1, crit: false, 1);
                }
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);
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

            var TrailTex = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhiteTrail").Value;
            var TrailTex2 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/WhiteTrail").Value;
            var TrailTex3 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CrystalTrail").Value;
            var TrailTex4 = ModContent.Request<Texture2D>("Stellamod/Effects/Primitives/Trails/CrystalTrail").Value;
            Color color = Color.Multiply(new(1.50f, 1.75f, 3.5f, 0), 200);



            if (SwordSlash == null)
            {
                SwordSlash = new TrailRenderer(TrailTex, TrailRenderer.DefaultPass, (p) => new Vector2(50f), (p) => new Color(110, 15, 250, 90) * (1f - p));
                SwordSlash.drawOffset = Projectile.Size / 1.8f;
            }
            if (SwordSlash2 == null)
            {
                SwordSlash2 = new TrailRenderer(TrailTex2, TrailRenderer.DefaultPass, (p) => new Vector2(90f), (p) => new Color(255, 255, 255, 100) * (1f - p));
                SwordSlash2.drawOffset = Projectile.Size / 1.9f;

            }
            if (SwordSlash3 == null)
            {
                SwordSlash3 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(100f), (p) => new Color(10, 105, 250, 90) * (1f - p));
                SwordSlash3.drawOffset = Projectile.Size / 2f;

            }

            if (SwordSlash4 == null)
            {
                SwordSlash4 = new TrailRenderer(TrailTex3, TrailRenderer.DefaultPass, (p) => new Vector2(90f), (p) => new Color(0, 0, 0, 110));
                SwordSlash4.drawOffset = Projectile.Size / 2.2f;

            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);



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