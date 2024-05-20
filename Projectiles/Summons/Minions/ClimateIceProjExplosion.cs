using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions
{
    internal class ClimateIceProjExplosion : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.aiStyle = 595;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            int tickDuration = Main.rand.Next(60, 240);
            target.AddBuff(BuffID.Frozen, tickDuration);
            target.AddBuff(BuffID.Frostburn, tickDuration);
        }
    }
}
