using Stellamod.Buffs.Minions;
using Stellamod.Common.ArmorRework;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.HeavyMetal
{
    public class HMMinionBuff : MinionBuff<HMArncharMinionRightProj> { }

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

    public class GintzeSpearMini : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.JavelinFriendly);
            AIType = ProjectileID.JavelinFriendly;
            Projectile.penetrate = 1;
            Projectile.scale = 0.65f;
            Projectile.alpha = 80;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                var particle = Particle<DustParticle>.Spawn(Projectile.Center, Projectile.oldVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                particle.gravity = 0;
                particle.dampening = 0.05f;
            }
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }
    }

    public class HeavyMetalPlayer : ModPlayer
    {
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSetBonus = false;
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            if (hasSetBonus && Player.ownedProjectileCounts[ModContent.ProjectileType<HMArncharMinionRightProj>()] == 0 && Main.myPlayer == Player.whoAmI)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone3"), Player.position);
                var EntitySource = Player.GetSource_FromThis();

                int damage = 17;
                Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0,
                    ModContent.ProjectileType<HMArncharMinionRightProj>(), damage, 1, Player.whoAmI, 0, ai1: 1);
                Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0,
                    ModContent.ProjectileType<HMArncharMinionRightProj>(), damage, 1, Player.whoAmI, 0, ai1: -1);
                Player.AddBuff(ModContent.BuffType<HMMinionBuff>(), 99999);
            }
            else if (!hasSetBonus)
            {
                Player.ClearBuff(ModContent.BuffType<HMMinionBuff>());
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class HeavyMetalHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<HeavyMetalHead, HeavyMetalBody, HeavyMetalLegs>(ArmorGroup.Act_I);
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HeavyMetalBody>() && legs.type == ModContent.ItemType<HeavyMetalLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            var stats = player.GetStats();
            stats.summonCastTime -= 0.5f;
            stats.defenseBonus += 5;
            stats.accessorySlots += 1;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<HeavyMetalPlayer>().hasSetBonus = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class HeavyMetalBody : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            var stats = player.GetStats();
            stats.defenseBonus += 5;
            stats.minionSlots += 2;
            stats.accessorySlots += 2;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class HeavyMetalLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 3;
            stats.minionSummonHealth += 0.5f;
            stats.accessorySlots += 1;
        }
    }
}
