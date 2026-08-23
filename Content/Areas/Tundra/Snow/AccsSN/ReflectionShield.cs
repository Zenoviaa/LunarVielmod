using Microsoft.Xna.Framework;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.AccsSN
{
    public class ReflectionShield : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToShield(ModContent.ProjectileType<ReflectionShieldHeld>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<WinterbornShard, BlankCard>();
        }
    }
    public class ReflectionShieldHeld : AbstractShieldProjectile
    {
        public override void OnBlockMovement(NPC npc)
        {
            base.OnBlockMovement(npc);

            Vector2 pushVelocity = (npc.Center - Owner.Center).SafeNormalize(Vector2.Zero);
            pushVelocity *= 22;
            for (float f = 0; f < 2; f++)
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams();
                spawnParams.outerColor = Color.Blue;
                DustParticle.Spawn(Projectile.Center, pushVelocity * Main.rand.NextFloat(0.2f, 0.5f) * 4f, spawnParams);
            }


            var hit = SoundID.NPCHit53;
            hit.PitchVariance = 0.3f;
            hit.Volume = 0.15f;
            SoundEngine.PlaySound(hit, npc.position);
            npc.velocity += pushVelocity;
        }
    }
}
