using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Common.WeaponTypes
{

    public class ManaSphereGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isManaSphere;
        public int heldProj;
        public int staminaProj;
        public int staminaCost;
        public override bool AltFunctionUse(Item item, Player player)
        {
            if (isManaSphere)
                return true;
            return base.AltFunctionUse(item, player);
        }

        public override bool CanShoot(Item item, Player player)
        {
            if (isManaSphere)
            {
                //Check if any balls are out
                //If a ball is out you can shoot
                //If there's none, we'll get an error :(
                bool hasAnyBallOut = false;
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != player.whoAmI)
                        continue;
                    if (proj.type != heldProj)
                        continue;

                    hasAnyBallOut = true;
                    break;
                }

                return base.CanShoot(item, player) && hasAnyBallOut;
            }
            return base.CanShoot(item, player);
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (isManaSphere)
            {
                List<Projectile> possibleSpawnPoints = new List<Projectile>();
                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != player.whoAmI)
                        continue;
                    if (proj.type != heldProj)
                        continue;

                    possibleSpawnPoints.Add(proj);
                }

                Projectile point = possibleSpawnPoints[Main.rand.Next(0, possibleSpawnPoints.Count)];
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Main.rand.NextVector2Circular(4, 4), heldProj, 1, 1, player.whoAmI);
                point.Kill();
                DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
                if (player.altFunctionUse == 2 && dashPlayer.CanConsume(staminaCost))
                {
                    dashPlayer.Consume(staminaCost);
                    player.GetModPlayer<DashPlayer>().StaminaEffects(player);
                    type = staminaProj;
                }

                Projectile.NewProjectile(player.GetSource_FromThis(), point.Center, velocity, type, damage, knockback, player.whoAmI);
                return false;
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }

    public class ManaSphereExpandingTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            ManaSphereGlobalItem manaSphere = item.GetGlobalItem<ManaSphereGlobalItem>();
            if (manaSphere.isManaSphere)
            {
                TooltipLine line;

                line = new TooltipLine(Mod, "BasicSlash", LangText.Common("BasicSlash", LangText.Item(item.ModItem, "BasicSlash")));
                lines.Add(line);

                line = new TooltipLine(Mod, "StaminaSlash", LangText.Common("StaminaSlash", LangText.Item(item.ModItem, "StaminaSlash")));
                lines.Add(line);

                line = new TooltipLine(Mod, "StaminaCost", LangText.Common("StaminaCost", manaSphere.staminaCost.ToString()));
                line.OverrideColor = Color.Goldenrod;
                lines.Add(line);


                TooltipLine manaSphereHelp = new TooltipLine(Mod, "ManaSphere", LangText.Common("ManaSphereHelp"));
                lines.Add(manaSphereHelp);
            }
        }
    }
    public abstract class AbstractManaSphereHold : ModProjectile
    {
        private UnifiedRandom _random;
        private Vector2 TargetHoldPosition;
        protected ref float Timer => ref Projectile.ai[0];
        protected int Seed => (int)Projectile.ai[2];
        protected Player Owner => Main.player[Projectile.owner];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(TargetHoldPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            TargetHoldPosition = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public sealed override void AI()
        {
            base.AI();

            Item item = Owner.HeldItem;
            if (item.IsAir || item == null)
                return;
            ManaSphereGlobalItem manaSphere = item.GetGlobalItem<ManaSphereGlobalItem>();
            if (manaSphere.heldProj != Type)
                return;
            Projectile.timeLeft = 2;
            Timer++;
            if (Timer == 1)
            {
                if (Main.myPlayer == Projectile.owner)
                {

                    Projectile.ai[2] = Main.rand.Next(0, int.MaxValue);

                    Projectile.netUpdate = true;
                }
            }
            DustEffects();
            if (Owner.controlUseItem)
            {
                AI_FireOut();

            }
            else
            {
                AI_OrbitPlayer();
            }

        }


        public virtual void DustEffects()
        {

        }
        public virtual void AI_FireOut()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                TargetHoldPosition = Owner.Center + (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 32;
                Projectile.netUpdate = true;
            }
            Vector2 idlePosition = TargetHoldPosition;

            _random ??= new UnifiedRandom();
            _random.SetSeed(Seed);
            idlePosition.X += _random.NextFloat(-32, 32);
            idlePosition.Y += _random.NextFloat(-32, 32);
            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();


            if (Main.myPlayer == Owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);


        }

        public virtual void AI_OrbitPlayer()
        {
            _random ??= new UnifiedRandom();
            _random.SetSeed(Seed);
            Vector2 idlePosition = Owner.Center;
            idlePosition.X += _random.NextFloat(-32, 32);
            idlePosition.Y += _random.NextFloat(-32, 32);

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();


            if (Main.myPlayer == Owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }

    public class ManaSpherePlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            Item item = Player.HeldItem;
            if (item.IsAir || item == null)
                return;
            ManaSphereGlobalItem manaSphere = item.GetGlobalItem<ManaSphereGlobalItem>();
            if (!manaSphere.isManaSphere)
                return;
            if (Player.whoAmI == Main.myPlayer && Player.ownedProjectileCounts[manaSphere.heldProj] == 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(12, 12);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, vel, manaSphere.heldProj, 1, 1, Player.whoAmI);
                }

            }
        }
    }
}
