using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Fenix
{
    public class AngelenthalProj2 : ModProjectile
    {
        public static bool swung = false;
        public int SwingTime = 10;
        public float holdOffset = 100f;
        public int combowombo;
        private bool _initialized;
        private int timer;
        private bool ParticleSpawned;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        private Player Owner => Main.player[Projectile.owner];

        public float SwingDistance;
        public float Curvature;

        public ref float AiState => ref Projectile.ai[1];
        private Vector2 returnPosOffset; //The position of the projectile when it starts returning to the player from being hooked
        private Vector2 npcHookOffset = Vector2.Zero; //Used to determine the offset from the hooked npc's center
        private float npcHookRotation; //Stores the projectile's rotation when hitting an npc
        private NPC hookNPC; //The npc the projectile is hooked into

        public const float THROW_RANGE = 320; //Peak distance from player when thrown out, in pixels
        public const float HOOK_MAXRANGE = 800; //Maximum distance between owner and hooked enemies before it automatically rips out
        public const int HOOK_HITTIME = 20; //Time between damage ticks while hooked in
        public const int RETURN_TIME = 6; //Time it takes for the projectile to return to the owner after being ripped out

        public bool Flip = false;
        public bool Slam = false;
        public bool PreSlam = false;

        public Vector2 CurrentBase = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 90;
            Projectile.width = 98;
            Projectile.friendly = true;
            Projectile.scale = 1f;
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

        public override void AI()
        {



            Player player = Main.player[Projectile.owner];
            if (!_initialized)
            {
                timer++;

                SwingTime = (int)(15 / player.GetAttackSpeed(DamageClass.Melee));
                Projectile.alpha = 255;
                Projectile.timeLeft = SwingTime;
                _initialized = true;
                Projectile.damage -= 9999;
                //Projectile.netUpdate = true;

            }
            else if (_initialized)
            {
                if (!player.active || player.dead || player.CCed || player.noItems)
                {
                    return;
                }
                Projectile.alpha = 0;
                if (timer == 1)
                {
                    Projectile.damage += 9999;
                    Projectile.damage *= 3;

                    timer++;
                }
                Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
                float multiplier = 0.2f;
                float max = 2.25f;
                float min = 1.0f;
                RGB *= multiplier;
                if (RGB.X > max)
                {
                    multiplier = 0.5f;
                }
                if (RGB.X < min)
                {
                    multiplier = 1.5f;
                }
                Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10000;
                for (int i = 0; i < 1; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.HeartCrystal, 0, 0, 100, Color.Violet, 1f);
                    dust.noGravity = true;
                    dust.velocity *= 2f;

                }
                int dir = (int)Projectile.ai[1];
                float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
                // the actual rotation it should have
                float defRot = Projectile.velocity.ToRotation();
                // starting rotation
                float endSet = ((MathHelper.PiOver2) / 0.2f);
                float start = defRot - endSet;

                // ending rotation
                float end = defRot + endSet;
                // current rotation obv
                float rotation = dir == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
                // offsetted cuz sword sprite
                Vector2 position = Main.MouseWorld;
                position += rotation.ToRotationVector2() * holdOffset;
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = rotation * player.direction;
                player.itemTime = 2;
                player.itemAnimation = 2;
                //Projectile.netUpdate = true;


                if (!ParticleSpawned)
                {


                    ParticleSpawned = true;
                }

            }
        }
        private Vector2 GetSwingPosition(float progress)
        {
            //Starts at owner center, goes to peak range, then returns to owner center
            float distance = MathHelper.Clamp(SwingDistance, THROW_RANGE * 0.1f, THROW_RANGE) * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.Pi), 1, 0.04f);
            distance = Math.Max(distance, 100); //Dont be too close to player

            float angleMaxDeviation = MathHelper.Pi / 1.2f;
            float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(-angleMaxDeviation, angleMaxDeviation, progress); //Moves clockwise if player is facing right, counterclockwise if facing left
            return Projectile.velocity.RotatedBy(angleOffset) * distance;
        }


        public override bool ShouldUpdatePosition() => false;


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            player.GetModPlayer<MyPlayer>().SwordCombo++;
            player.GetModPlayer<MyPlayer>().SwordComboR = 480;
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SwingTime);
            writer.Write(SwingDistance);
            writer.WriteVector2(returnPosOffset);
            writer.WriteVector2(npcHookOffset);
            writer.Write(npcHookRotation);
            writer.Write(Flip);
            writer.Write(Slam);
            writer.Write(Curvature);

            if (hookNPC == default(NPC)) //Write a -1 instead if the npc isnt set
                writer.Write(-1);
            else
                writer.Write(hookNPC.whoAmI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingTime = reader.ReadInt32();
            SwingDistance = reader.ReadSingle();
            returnPosOffset = reader.ReadVector2();
            npcHookOffset = reader.ReadVector2();
            npcHookRotation = reader.ReadSingle();
            Flip = reader.ReadBoolean();
            Slam = reader.ReadBoolean();
            Curvature = reader.ReadSingle();

            int whoAmI = reader.ReadInt32(); //Read the whoami value sent
            if (whoAmI == -1) //If its a -1, sync that the npc hasn't been set yet
                hookNPC = default;
            else
                hookNPC = Main.npc[whoAmI];
        }
    }
}