using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    internal class SerpentSegment
    {
        public string TexturePath;
        public Texture2D Texture => ModContent.Request<Texture2D>(TexturePath).Value;
        public Texture2D GlowTexture => ModContent.Request<Texture2D>(TexturePath + "_Glow").Value;
        public Color GlowWhiteColor;
        public bool GlowWhite;
        public float GlowTimer;
        public Rectangle? Frame;
        public Vector2 Size => Texture.Size();
        public Vector2 Position;
        public Vector2 Center => Position + Size / 2;
        public Vector2 Velocity;
        public float Rotation;
        public float Scale = 1f;
        public bool Eaten;
        public int FrameCounter;
        public int FrameTick;
        public SerpentSegment(Projectile projectile)
        {
            Position = projectile.position;
            Rotation = 0;
            Velocity = Vector2.Zero;
            Eaten = false;
        }
    }

    internal class SerpentMinionProj : ModProjectile
    {

        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float SegmentStretch = 0.66f;
        private float ChargeTrailOpacity;
        private bool DrawChargeTrail;

        //Segments
        private SerpentSegment Head => Segments[0];
        private SerpentSegment[] Segments;
        private Vector2 HitboxFixer => new Vector2(Projectile.width, Projectile.height) / 2;


        private Projectile MainSerpentProjectile
        {
            get
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active)
                        continue;
                    if (p.type == Type && p.owner == Projectile.owner)
                        return p;
                }

                return Projectile;
            }
        }

        private bool IsMainSerpent
        {
            get
            {
                return MainSerpentProjectile == Projectile;
            }
        }

        private ref float Timer => ref Projectile.ai[1];
        private int SegmentCount
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = (int)value;
        }

        private Player Owner => Main.player[Projectile.owner];
        private int _originalDamage;
        private float ExtraScale;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 3;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;

            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.minionSlots = 1;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            InitSegments(3);
        }

        private void InitSegments(int numSegments)
        {          
            //Initialize Segments
            List<SerpentSegment> segments = new List<SerpentSegment>();
            //Set the textures

            //Head
            SerpentSegment segment = new SerpentSegment(Projectile);
            segment.TexturePath = $"{Texture}";
            segment.Position = Projectile.position + new Vector2(1, 0);
            segments.Add(segment);
            for (int i = 0; i < numSegments; i++)
            {
                segment = new SerpentSegment(Projectile);
                segment.TexturePath = $"{Texture}_Body";
                segment.Position = Projectile.position + new Vector2(1, 0);

                float p = i;
                float progress = p / (float)numSegments;
                progress = 1 - progress;
                segment.Scale = Math.Max(0.5f, progress);

                segments.Add(segment);
            }
    
            segment = new SerpentSegment(Projectile);
            segment.TexturePath = $"{Texture}_Tail";
            segment.Position = Projectile.position + new Vector2(1, 0);
            segments.Add(segment);
            Segments = segments.ToArray();
            SegmentCount = numSegments;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return IsMainSerpent;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return IsMainSerpent;
        }

        

        public override void AI()
        {
            //Whopliwooo
            if (!SummonHelper.CheckMinionActive<SerpentMinionBuff>(Owner, Projectile))
                return;

            if (!IsMainSerpent)
            {
                Projectile.position = MainSerpentProjectile.position;
                return;
            }

            if(_originalDamage == 0)
            {
                _originalDamage = Projectile.originalDamage;
            }
  
            int ownedCounts = Owner.ownedProjectileCounts[Type];
            Projectile.originalDamage = _originalDamage + ownedCounts * 15;
            ExtraScale = 1f + ((float)ownedCounts * 0.05f);

            int numBodySegments = (int)(ownedCounts * 10) + 1;
            if(SegmentCount != numBodySegments)
            {
                InitSegments(numBodySegments);
            }

            GlowWhite(1 / 60f);
            Timer++;
            if(Timer == 1)
            {
                _originalDamage = Projectile.originalDamage;
            }
  
            SummonHelper.SearchForTargetsThroughTiles(Owner, Projectile,
                 out bool foundTarget,
                 out float distanceFromTarget,
                 out Vector2 targetCenter);

            float orbitRadius = 384;
            Vector2 direction = Owner.Center.DirectionTo(Projectile.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / 180);
            Vector2 orbitCenter = Owner.Center + direction * orbitRadius;

            if (foundTarget && distanceFromTarget <= 1500)
            {
                if (distanceFromTarget > 256)
                {
                    AI_MoveToward(targetCenter, 16, 1);
                }
                else
                {
                    Projectile.velocity *= 0.8f;
                }


                StartSegmentGlow(Color.White);
                Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, directionToTarget.ToRotation(), 0.1f);
                if (Timer % 8 == 0)
                {
                    //Shoot fire breath
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, directionToTarget * 10,
                        ModContent.ProjectileType<SerpentMinionFireBreathProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            else
            {
                SummonHelper.CalculateIdleValuesWithOverlap(Owner, Projectile, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.1f);
                StopSegmentGlow();
            }

            MakeLikeWorm();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!IsMainSerpent)
                return false;

            //This damages everything in the trail
            float collisionPoint = 0;
            for (int i = 1; i < Segments.Length; i++)
            {
                Vector2 position = Segments[i].Position;
                Vector2 previousPosition = Segments[i - 1].Position;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 8, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        private void AI_MoveToward(Vector2 targetCenter, float speed = 8, float accel = 16)
        {
            //chase target
            Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(Projectile.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;

            if (Projectile.velocity.X < targetVelocity.X)
            {
                Projectile.velocity.X += accel;
                if (Projectile.velocity.X >= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }
            else if (Projectile.velocity.X > targetVelocity.X)
            {
                Projectile.velocity.X -= accel;
                if (Projectile.velocity.X <= targetVelocity.X)
                {
                    Projectile.velocity.X = targetVelocity.X;
                }
            }

            if (Projectile.velocity.Y < targetVelocity.Y)
            {
                Projectile.velocity.Y += accel;
                if (Projectile.velocity.Y >= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
            else if (Projectile.velocity.Y > targetVelocity.Y)
            {
                Projectile.velocity.Y -= accel;
                if (Projectile.velocity.Y <= targetVelocity.Y)
                {
                    Projectile.velocity.Y = targetVelocity.Y;
                }
            }
        }

        #region Draw Code

        private void GlowWhite(float speed)
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                SerpentSegment segment = Segments[i];
                if (segment.GlowWhite)
                {
                    segment.GlowTimer += speed;
                    if (segment.GlowTimer >= 1f)
                        segment.GlowTimer = 1f;
                }
                else
                {
                    segment.GlowTimer -= speed;
                    if (segment.GlowTimer <= 0)
                        segment.GlowTimer = 0;
                }
            }
        }

        private void MakeLikeWorm()
        {
            //Segments
            Head.Position = Projectile.position;
            Head.Rotation = Projectile.rotation;
            MoveSegmentsLikeWorm();
        }

        private void StartSegmentGlow(Color color)
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                StartSegmentGlow(i, color);
            }
        }

        private void StartSegmentGlow(int index, Color color)
        {
            SerpentSegment segment = Segments[index];
            segment.GlowWhiteColor = color;
            segment.GlowWhite = true;
        }

        private void StopSegmentGlow()
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                StopSegmentGlow(i);
            }
        }

        private void StopSegmentGlow(int index)
        {
            SerpentSegment segment = Segments[index];
            segment.GlowWhite = false;
        }

        private void ResetSegmentGlow()
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                ResetSegmentGlow(i);
            }
        }

        private void ResetSegmentGlow(int index)
        {
            SerpentSegment segment = Segments[index];
            segment.GlowWhite = false;
            segment.GlowTimer = 0;
        }

        private void MoveSegmentsLikeWorm()
        {
            for (int i = 1; i < Segments.Length; i++)
            {
                MoveSegmentLikeWorm(i);
            }
        }

        private void MoveSegmentLikeWorm(int index)
        {
            int inFrontIndex = index - 1;
            if (inFrontIndex < 0)
                return;
            
            ref var segment = ref Segments[index];
            ref var frontSegment = ref Segments[index - 1];

            // Follow behind the segment "in front" of this Projectile
            // Use the current Projectile.Center to calculate the direction towards the "parent Projectile" of this Projectile.
            float dirX = frontSegment.Position.X - segment.Position.X;
            float dirY = frontSegment.Position.Y - segment.Position.Y;

            // We then use Atan2 to get a correct rotation towards that parent Projectile.
            // Assumes the sprite for the Projectile points upward.  You might have to modify this line to properly account for your Projectile's orientation
            segment.Rotation = (float)Math.Atan2(dirY, dirX);
            // We also get the length of the direction vector.
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            if (length == 0)
                length = 1;

            // We calculate a new, correct distance.

            float fixer = 1;
            if (index == Segments.Length - 1)
            {
                fixer /= 1.75f;
            }

     
            float dist = (length - segment.Size.X * SegmentStretch * fixer) / length;

            float posX = dirX * dist;
            float posY = dirY * dist;

            //reset the velocity
            segment.Velocity = Vector2.Zero;


            // And set this Projectiles position accordingly to that of this Projectiles parent Projectile.
            segment.Position.X += posX;
            segment.Position.Y += posY;
        }


        public float WidthFunctionCharge(float completionRatio)
        {
            return Projectile.width * Projectile.scale * (1f - completionRatio) * 2f;
        }

        public Color ColorFunctionCharge(float completionRatio)
        {
            if (!DrawChargeTrail)
            {
                ChargeTrailOpacity -= 0.05f;
                if (ChargeTrailOpacity <= 0)
                    ChargeTrailOpacity = 0;
            }
            else
            {
                ChargeTrailOpacity += 0.05f;
                if (ChargeTrailOpacity >= 1)
                    ChargeTrailOpacity = 1;
            }

            return Color.Lerp(Color.Orange, Color.Orange, (1f - completionRatio)) * ChargeTrailOpacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!IsMainSerpent)
                return false;

            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunctionCharge, ColorFunctionCharge, GameShaders.Misc["VampKnives:BasicTrail"]);
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            Vector2 size = new Vector2(90, 90);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:BasicTrail"];
            TrailDrawer.DrawPrims(Projectile.oldPos, size * 0.5f - Main.screenPosition, 155);

            //Draw all the segments
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                SerpentSegment segment = Segments[i];
                if (segment.Eaten)
                    continue;

                Vector2 drawPosition = segment.Position - Main.screenPosition + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = Projectile.scale * segment.Scale * ExtraScale;
                Color drawColor = Color.White;
                spriteBatch.Draw(segment.Texture, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = Segments.Length - 1; i > -1; i--)
            {
                SerpentSegment segment = Segments[i];
                if (segment.Eaten)
                    continue;

                if (!ModContent.RequestIfExists<Texture2D>(segment.TexturePath + "_Glow", out var asset))
                    continue;

                Vector2 drawPosition = segment.Position - Main.screenPosition + HitboxFixer;
                float drawRotation = segment.Rotation;
                Vector2 drawOrigin = segment.Size / 2;
                float drawScale = Projectile.scale * segment.Scale * ExtraScale;
                Color drawGlowColor = Color.White;
                float osc = VectorHelper.Osc(0, 1);

                spriteBatch.Draw(asset.Value, drawPosition, null, drawGlowColor * osc, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

                for (float j = 0f; j < 1f; j += 0.25f)
                {
                    float radians = (j + osc) * MathHelper.TwoPi;
                    spriteBatch.Draw(segment.GlowTexture, drawPosition + new Vector2(0f, 8f).RotatedBy(radians) * osc,
                        null, Color.White * osc * 0.3f, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }

                if (segment.GlowTimer > 0 && ModContent.RequestIfExists<Texture2D>(segment.TexturePath + "_White", out var whiteAsset))
                {
                    spriteBatch.Draw(whiteAsset.Value, drawPosition, null, segment.GlowWhiteColor * segment.GlowTimer, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
            }
        }
        #endregion
    }
}
