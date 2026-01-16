using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Projectiles.Thrown;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    public class HMArncharMinionRightProj : ModProjectile
    {
        private Vector2 _attackPosition;
        private enum AIState
        {
            Idle,
            Attack,
            Return
        }
        private ref float Timer => ref Projectile.ai[0];
        private ref float Side => ref Projectile.ai[1];

        private AIState State
        {
            get => (AIState)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        private Player Owner => Main.player[Projectile.owner];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_attackPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _attackPosition = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("HMArncharMinion");
            // Sets the amount of frames this minion has on its spritesheet
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 4;
            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 32;
            Projectile.height = 32;

            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;

            // Only determines the damage type
            Projectile.minion = false;

            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 0f;

            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() * 0.5f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<HMMinionBuff>(player, Projectile))
                return;

            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Attack:
                    AI_Attack();
                    break;
                case AIState.Return:
                    AI_Return();
                    break;
            }
            // So it will lean slightly towards the direction it's moving

            DrawHelper.AnimateTopToBottom(Projectile, 5);
            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }

        private void AI_Idle()
        {
            Timer++;
            Vector2 hoverPosition = Owner.Top + new Vector2(0, -16) + (Vector2.UnitX * Side * 64);
            Vector2 targetVelocity = (hoverPosition - Projectile.Center);
            Projectile.velocity = targetVelocity;
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
            NPC nearest = NPCHelper.FindClosestNPC(Projectile.position, 1000);
            if (nearest != null)
            {
                _attackPosition = nearest.Center;
                SwitchState(AIState.Attack);
            }
        }
        private void AI_Attack()
        {
            Timer++;
            if (Timer == 15)
            {
                Vector2 fireVelocity = (_attackPosition - Projectile.Center);
                fireVelocity = fireVelocity.SafeNormalize(Vector2.Zero);
                fireVelocity *= 15;
                Projectile.velocity = -fireVelocity * 0.15f;
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;
            if (Timer == 15 && this.OwnedByLocalClient())
            {
                Vector2 fireVelocity = (_attackPosition - Projectile.Center);
                fireVelocity = fireVelocity.SafeNormalize(Vector2.Zero);
                fireVelocity *= 21;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireVelocity,
                    ModContent.ProjectileType<GintzeSpearMini>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            if (Timer >= 30f)
            {
                SwitchState(AIState.Return);
            }
        }
        private void AI_Return()
        {
            Timer++;
            float returnTime = 60f;
            Vector2 hoverPosition = Owner.Top + new Vector2(0, -16) + (Vector2.UnitX * Side * 64);
            Vector2 targetVelocity = (hoverPosition - Projectile.Center);
            Projectile.velocity = Vector2.Lerp(Vector2.Zero, targetVelocity, EasingFunction.InOutSine(Timer / returnTime));
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Timer >= returnTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }

}

