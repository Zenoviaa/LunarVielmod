using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.NPCs.Bosses.Jiitas.Projectiles;
using Stellamod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas
    {
        public float BombDropNumFlies
        {
            get
            {
                return 6;
            }
        }

        public float BombDropTimeBetweenFlies
        {
            get
            {
                return 10;
            }
        }

        public float BombDropStartupTime
        {
            get
            {
                return 30;
            }
        }

        public int BombDropDamage
        {
            get
            {
                return 20;
            }
        }

        public float BombDropLaughTime
        {
            get
            {
                return 90;
            }
        }

        private void AI_BombDrop()
        {
            switch (ActionStep)
            {
                case 0:
                    AI_BombDropStartup();
                    break;
                case 1:
                    AI_BombDropLaugh();
                    break;
            }
        }

        private void AI_BombDropStartup()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasSit;
                jiitasSit.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jiitasSit, NPC.position);
            }

            NPC.velocity *= 0.9f;
            NPC.rotation *= 0.9f;

            Warn();
            PlayAnimation(AnimationState.Situp);
            if(Timer % BombDropTimeBetweenFlies == 0)
            {
                SoundStyle bombThrow = AssetRegistry.Sounds.Jiitas.JiitasBombThrow;
                bombThrow.PitchVariance = 0.2f;
                SoundEngine.PlaySound(bombThrow, NPC.position);
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 spawnPos = Target.Center;
                    spawnPos.Y -= 600;
                    spawnPos.X += Main.rand.NextFloat(-500, 500);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, -Vector2.UnitY, ModContent.ProjectileType<JiitasBombString>(), BombDropDamage, 1, Main.myPlayer);
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 left = Target.Center - new Vector2(384, 0);
                        Vector2 right = Target.Center + new Vector2(384, 0);
                        Vector2 spot = Vector2.Lerp(left, right, Main.rand.NextFloat(0f, 1f));
                        if (MultiplayerHelper.IsHost)
                        {
                            spot.Y = Target.Center.Y - 500;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), spot, Vector2.Zero, ModContent.ProjectileType<JiitasBomb>(), BombDropDamage, 1, Main.myPlayer);
                        }
                    }
                }
                AttackCounter++;
            }
            if(AttackCounter >= BombDropNumFlies)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_BombDropLaugh()
        {
            Timer++;
            NPC.noGravity = true;
            NPC.velocity.Y -= 0.015f;
            NoWarn();
            PlayAnimation(AnimationState.Laugh);
            Empowered = false;
            if(Timer >= BombDropLaughTime)
            {

                SwitchState(ActionState.Idle);
            }
        }
    }
}
