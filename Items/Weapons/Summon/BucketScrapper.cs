using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Helpers;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using Stellamod.Projectiles;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Weapons.Summon
{
    internal class BucketScrapper : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.damage = 21;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 2;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.Item113;
            Item.buffType = ModContent.BuffType<BucketScrapperMinionBuff>();
            Item.shoot = ModContent.ProjectileType<BucketScrapperMinion>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wire, 100);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DriveConstruct>(), 20);
            recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }


    }


    internal class BucketScrapperMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BucketScrapperMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    internal class BucketScrapperMinion : ModProjectile
    {
        private static float _orbitingOffset;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
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
            Projectile.width = 68;
            Projectile.height = 70;

            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 1f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool? CanCutTiles()
        {
            return true;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        private Vector2 CalculateCirclePosition(Player owner)
        {
            //Get the index of this minion
            int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

            //Now we can calculate the circle position	
            int minionCount = owner.ownedProjectileCounts[ModContent.ProjectileType<BucketScrapperMinion>()];
            float degreesBetweenFirefly = 360 / (float)minionCount;
            float degrees = degreesBetweenFirefly * minionIndex;
            float circleDistance = 96f;
            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitingOffset));
            return circlePosition;
        }


        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<BucketScrapperMinionBuff>(owner, Projectile))
                return;

            SummonHelper.SearchForTargets(owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (!foundTarget)
            {
                Vector2 circlePosition = CalculateCirclePosition(owner);
                float speed = 48;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, VectorHelper.VelocitySlowdownTo(Projectile.Center, circlePosition, speed), 0.1f);
            } 
            else
            {
                float speed = 20;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, VectorHelper.VelocitySlowdownTo(Projectile.Center, targetCenter, speed), 0.1f);
            }
            
            Visuals();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            direction = direction.RotatedByRandom(MathHelper.ToRadians(30));
            Projectile.velocity = -direction * 71;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<NailKaboom>(), 0, 0, Projectile.owner);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2") with { PitchVariance = 0.1f });
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.DarkCyan, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            Player owner = Main.player[Projectile.owner];
            int minionCount = owner.ownedProjectileCounts[ModContent.ProjectileType<BucketScrapperMinion>()];

            _orbitingOffset+=0.3f;
            Projectile.rotation += MathHelper.ToRadians(2 + minionCount);
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;
            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
