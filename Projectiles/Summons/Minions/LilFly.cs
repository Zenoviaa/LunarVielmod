using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Minions;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class LilFly : ModProjectile
    {
        public enum AIState
        {
            Offense,
            Defense
        }


        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private ref float ShootTimer => ref Projectile.ai[2];
        private Player Owner => Main.player[Projectile.owner];
        private Vector2 HoverOffset;
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            //Only have contact damage in defense mode.
            return State == AIState.Defense;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(HoverOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            HoverOffset = reader.ReadVector2();
        }


        public override void AI()
        {
            base.AI();
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<FireflyMinionBuff>(owner, Projectile))
                return;

            ShootTimer--;
            if (ShootTimer <= 0)
            {
                ShootTimer = 0;
            }

            switch (State)
            {
                case AIState.Offense:
                    AI_Offense();
                    break;
                case AIState.Defense:
                    AI_Defense();
                    break;
            }
            if (Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;
            Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.rotation = Projectile.velocity.X * 0.15f;
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }


        private void AI_Offense()
        {
            Vector2 hoverPosition = Owner.Center + HoverOffset;
            Vector2 targetVelocity = (hoverPosition - Projectile.Center).SafeNormalize(Vector2.Zero);

            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, hoverPosition, 3);
            float distanceToOwner = Vector2.Distance(Projectile.Center, Owner.Center);
            if (distanceToOwner > 512)
            {
                Projectile.velocity *= 1.01f;
            }
            else
            {
                if (Projectile.velocity.Length() > 5)
                    Projectile.velocity *= 0.98f;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (Main.rand.NextBool(25))
                {
                    HoverOffset = Main.rand.NextVector2Circular(256, 256);
                    Projectile.netUpdate = true;
                }
            }

            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Timer--;
            if (Timer <= 0 && foundTarget)
            {
                //Fire Projectile
                Vector2 velocity = VectorHelper.VelocityDirectTo(Projectile.Center, targetCenter, 30);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile projectile = Projectile.NewProjectileDirect(Owner.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<FireflyBomb>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    projectile.DamageType = DamageClass.Summon;
                    //How many ticks between attacks?
                    Timer = 25 + Main.rand.Next(5, 30);
                    Projectile.netUpdate = true;
                }


                ShootTimer = 15;
                //_scaleOffset += 0.1f;
            }

            if (Main.rand.NextBool(12))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, Vector2.Zero);
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 0.78f);
        }

        private void AI_Defense()
        {
            Vector2 targetPosition = CalculateCirclePosition(Owner);
            Projectile.velocity = (targetPosition - Projectile.Center) * 0.1f;
            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Timer--;
            if (Timer <= 0 && foundTarget)
            {
                //Fire Projectile
                Vector2 velocity = VectorHelper.VelocityDirectTo(Projectile.Center, targetCenter, 30);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile projectile = Projectile.NewProjectileDirect(Owner.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<FireflyBomb>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    projectile.DamageType = DamageClass.Summon;
                }

                //How many ticks between attacks?
                Timer = 80;
                ShootTimer = 15;
            }


            if (Main.rand.NextBool(12))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, Vector2.Zero);
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.Aquamarine.ToVector3() * 0.78f);
        }

        private Vector2 CalculateCirclePosition(Player owner)
        {
            //Get the index of this minion
            int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

            //Now we can calculate the circle position	
            int fireflyCount = owner.ownedProjectileCounts[Type];
            float degreesBetweenFirefly = 360 / (float)fireflyCount;
            float degrees = degreesBetweenFirefly * minionIndex;
            float circleDistance = State == AIState.Defense ? 48f : 80f;

            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + Main.GlobalTimeWrappedHourly));
            switch (State)
            {
                case AIState.Offense:

                    break;
                case AIState.Defense:
                    //float factor =  / (float)fireflyCount;
                    float t = Main.GlobalTimeWrappedHourly + (minionIndex / (float)fireflyCount) * minionIndex;
                    circlePosition = owner.Center + VectorHelper.PointOnHeart(t, VectorHelper.Osc(5, 10));
                    break;
            }

            return circlePosition;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            switch (State)
            {
                default:
                case AIState.Offense:
                    break;
                case AIState.Defense:
                    texture = ModContent.Request<Texture2D>(Texture + "_Blue").Value;
                    break;
            }
            float p = ShootTimer / 15f;
            float ep = Easing.InOutCubic(p);

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos.Y += VectorHelper.Osc(-4f, 4f, speed: 1f, Projectile.whoAmI);
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = VectorHelper.Osc(0.75f, 1f, offset: Projectile.whoAmI);


            drawScale += MathHelper.Lerp(0.25f, 0.5f, ep);
            SpriteEffects dir = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, dir, 0);
            DrawGlow(ref lightColor);
            spriteBatch.Restart(blendState: BlendState.Additive);
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, dir, 0);
            spriteBatch.Draw(texture, drawPos, frame, drawColor * ep, drawRotation, drawOrigin, drawScale, dir, 0);
            spriteBatch.Draw(texture, drawPos, frame, drawColor * ep, drawRotation, drawOrigin, drawScale + 0.5f * ep, dir, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        private void DrawGlow(ref Color lightColor)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            bool isDefense = State == AIState.Defense;
            if (isDefense)
            {
                Color startInner = Color.LightBlue;
                Color startGlow = Color.Lerp(Color.Aquamarine, Color.DarkBlue, VectorHelper.Osc(0f, 1f, speed: 3f));
                Color startOuterGlow = Color.Lerp(Color.DarkBlue, Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));

                shader.InnerColor = startInner;
                shader.GlowColor = startGlow;
                shader.OuterGlowColor = startOuterGlow;
            }
            else
            {
                //Colors
                Color startInner = Color.Goldenrod;
                Color startGlow = Color.Lerp(Color.Red, Color.DarkRed, VectorHelper.Osc(0f, 1f, speed: 3f));
                Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));

                shader.InnerColor = startInner;
                shader.GlowColor = startGlow;
                shader.OuterGlowColor = startOuterGlow;
            }

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2f;

            //Radius of the circle
            shader.Size = VectorHelper.Osc(0.09f, 0.12f, offset: Projectile.whoAmI);

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = VectorHelper.Osc(2.5f, 3.5f, offset: Projectile.whoAmI);
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 1; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }

    }
}
