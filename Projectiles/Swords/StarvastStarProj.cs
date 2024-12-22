using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    internal class StarvastStarProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        private ref float ai_Counter => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        bool foundTarget;
        internal PrimitiveTrail BeamDrawer;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player

            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
            }
        }

        public override void AI()
        {
            Visuals();
            ai_Counter++;
            Player owner = Main.player[Projectile.owner];

            NPC npc = NPCHelper.FindClosestNPC(Projectile.position, 700);
            if (npc != null)
            {
                foundTarget = true;
                AI_Movement(npc.Center, 15);
            }
            else
            {
                foundTarget = false;
                Timer += 0.02f;
                Vector2 orbitCenter = MovementHelper.OrbitAround(owner.Center, Vector2.UnitY, 64, Timer);
                Vector2 targetVel = (orbitCenter - Projectile.Center);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, 0.02f);
                //      Projectile.Center = Vector2.Lerp(Projectile.Center, orbitCenter, 0.8f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            //Charged Sound thingy

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }

            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Blue, duration: 25, baseSize: 0.06f);
        }


        public float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(16, 0, Easing.InExpo(completionRatio));
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Lerp(Color.LightCyan, Color.Blue, completionRatio), Color.Transparent, completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.RestartDefaults();
            Vector2 drawOffset = -Main.screenPosition + Projectile.Size / 2f;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);
            TrailDrawer.DrawPrims(Projectile.oldPos, drawOffset, 252);

            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, drawOffset, 155);

            spriteBatch.RestartDefaults();
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(44, 84, 94), Color.Transparent, ref lightColor);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float rotation = Projectile.rotation;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawScale = Projectile.scale;
            spriteBatch.Draw(texture, drawPos, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        private void Visuals()
        {
            if (ai_Counter == 0)
            {
                //Charged Sound thingy

            }

            if (foundTarget)
            {
                if (ai_Counter % 8 == 0)
                {

                }
            }



            // So it will lean slightly towards the direction it's moving
            float rotation = MathHelper.ToRadians(ai_Counter * 5);
            Projectile.rotation = rotation;

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }


        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {

        }
    }
}
