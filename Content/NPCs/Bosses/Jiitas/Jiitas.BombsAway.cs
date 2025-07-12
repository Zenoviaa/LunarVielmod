using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.NPCs.Bosses.Jiitas.Projectiles;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
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
        //    Bombs Away - Goes down and throws out bombs all around, will sometimes get pulled by the strings to mix up the movement, just back away to dodge it
        
        public float BombsAwayHandsUpTime
        {
            get
            {
                return 60;
            }
        }

        public float BombsAwayThrowNum
        {
            get
            {
                if (Empowered)
                {
                    return 24;
                }
                return 12;
            }
        }
        public float BombsAwayTimeBetweenThrows
        {
            get
            {
                return 12;
            }
        }

        public float BombsAwayThrowSpeed
        {
            get
            {
                return 6;
            }
        }

        public int BombsAwayDamage
        {
            get
            {
                return 20;
            }
        }
        private void AI_BombsAway()
        {
            switch (ActionStep)
            {
                case 0:
                    AI_Sitdown();
                    break;
                case 1:
                    AI_PutYourHandsUp();
                    break;
                case 2:
                    AI_ThrowBombs();
                    break;
                case 3:
                    AI_Situp();
                    break;
            }
        }

        private void AI_PutYourHandsUp()
        {
            Timer++;
            Warn();
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y -= MathHelper.Lerp(0.5f, 0.1f, Timer / BombsAwayHandsUpTime); NPC.rotation *= 0.9f;
            PlayAnimation(AnimationState.BombsAway);
            if(Timer >= BombsAwayHandsUpTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_ThrowBombs()
        {
            Timer++;
            //Wobble
            float rotationInterpolant = ExtraMath.Osc(0f, 1f);
            float targetRotation = MathHelper.Lerp(-0.05f, 0.05f, rotationInterpolant);
            NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);

            PlayAnimation(AnimationState.Bombing);
            if(Timer % BombsAwayTimeBetweenThrows == 0)
            {
                SoundStyle bombThrow = AssetRegistry.Sounds.Jiitas.JiitasBombThrow;
                bombThrow.PitchVariance = 0.2f;
                SoundEngine.PlaySound(bombThrow, NPC.position);

                if (MultiplayerHelper.IsHost)
                {
                    Vector2 upwardVelocity = -Vector2.UnitY * BombsAwayThrowSpeed;
                    upwardVelocity.X += Main.rand.NextFloat(-4, 4);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, upwardVelocity, ModContent.ProjectileType<JiitasBomb>(), BombsAwayDamage, 1, Main.myPlayer);
                }
                AttackCounter++;
            }


            if(AttackCounter >= BombsAwayThrowNum)
            {
                Timer = 0;
                ActionStep++;
            }

        }
    }
}
