using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Trap.Sparn
{
	[AutoloadBossHead]
    internal class Sparn : ModNPC
	{
		private enum AttackState
        {
			Idle,
			Telegraph_Death_Skulls,
			Death_Skulls,
			Telegraph_Cage,
			Cage,
			Telegraph_Shockwave,
			Shockwave
        }

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 29;
			NPCID.Sets.TrailCacheLength[Type] = 60;
			NPCID.Sets.TrailingMode[Type] = 2;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
		}

		public override void SetDefaults()
        {
			NPC.Size = new Vector2(36, 42);
			NPC.damage = 1;
			NPC.defense = 26;
			NPC.lifeMax = 6000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 40);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.scale = 1f;
            NPC.BossBar = ModContent.GetInstance<MiniBossBar>();
            if (!Main.dedServ)
			{
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/CatacombsBoss");
			}
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        //Trailing
        #region Draw Code

        private int _frameCounter;
		private int _frameTick;
		private void PreDrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Vector2 drawPos = NPC.Center - screenPos;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Main.GlobalTimeWrappedHourly / 2f + time * 0.04f;
			float rotationOffset = VectorHelper.Osc(1f, 2f, 5);
			time %= 4f;
			time /= 2f;

			if (time >= 1f)
			{
				time = 2f - time;
			}

			time = time * 0.5f + 0.5f;
			SpriteEffects effects = SpriteEffects.None;
			Player player = Main.player[NPC.target];
			if (player.Center.X < NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			Color rotatedColor = new Color(60, 0, 118, 75);
			Vector2 origin = new Vector2(36 / 2, 48 / 2);
			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				Vector2 rotatedPos = drawPos + new Vector2(0f, 8f * rotationOffset).RotatedBy(radians) * time;
				spriteBatch.Draw(texture, rotatedPos,
					texture.AnimationFrame(ref _frameCounter, ref _frameTick, 2, 29, false),
				rotatedColor, 0f, origin, 1f, effects, 0f);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				Vector2 rotatedPos = drawPos + new Vector2(0f, 16f * rotationOffset).RotatedBy(radians) * time;
				spriteBatch.Draw(texture, rotatedPos,
					texture.AnimationFrame(ref _frameCounter, ref _frameTick, 2, 29, false),
					rotatedColor, 0f, origin, 1f, effects, 0f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			int width = 36;
			int height = 42;

			Player player = Main.player[NPC.target];
			SpriteEffects effects = SpriteEffects.None;
			if (player.Center.X < NPC.Center.X)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			Vector2 drawPosition = NPC.Center - screenPos;
			Vector2 origin = new Vector2(width / 2, height / 2);

			Texture2D texture = TextureAssets.Npc[NPC.type].Value;
			int frameCount = 29;
			int startFrame = 0;
			int speed = 1;

			PreDrawAfterImage(spriteBatch, screenPos, drawColor);
			Rectangle rect = new Rectangle(0, startFrame * height, width, frameCount * height);
			spriteBatch.Draw(texture, drawPosition,
				texture.AnimationFrame(ref _frameCounter, ref _frameTick, speed, frameCount, rect, true),
				drawColor, 0f, origin, 1f, effects, 0f);
			return false;
		}

		#endregion

		private Vector2 _targetCenter;
		private float ai_Counter;
		private float ai_State;
		private bool _resetState;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ai_State);
            writer.Write(_resetState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ai_State = reader.ReadSingle();
            _resetState = reader.ReadBoolean();
        }

        private void FinishResetState()
        {
            if (_resetState)
            {
                ai_Counter = 0;
                _resetState = false;
            }
        }

        private void SwitchState(AttackState attackState)
        {
            if (StellaMultiplayer.IsHost)
            {
                ai_State = (float)attackState;
                _resetState = true;
                NPC.netUpdate = true;
            }
        }

        public override void AI()
		{
			NPC.damage = 0;
			NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
				NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, -8), 0.2f);
				NPC.EncourageDespawn(120);
				return;
            }

			AttackState attackState = (AttackState)ai_State;
            FinishResetState();
            switch (attackState)
            {
				case AttackState.Idle:
					Idle();
					break;

				case AttackState.Telegraph_Cage:
					TelegraphCage();
					break;

                case AttackState.Cage:
					AttackCage();
					break;

				case AttackState.Telegraph_Death_Skulls:
					TelegraphDeathSkulls();
					break;

				case AttackState.Death_Skulls:
					AttackDeathSkulls();
					break;

				case AttackState.Telegraph_Shockwave:
					TelegraphShockwave();
					break;

				case AttackState.Shockwave:
					AttackShockwave();
					break;
            }
        }

		private void Idle()
		{
			Player target = Main.player[NPC.target];
			float y = VectorHelper.Osc(0, -8);

			_targetCenter = target.Center + new Vector2(0, -128 + y);
			Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(NPC.Center, _targetCenter, 5);
			NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
			
			ai_Counter++;
			if (ai_Counter > 60)
			{
				if (StellaMultiplayer.IsHost)
				{
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            SwitchState(AttackState.Telegraph_Cage);
                            break;
                        case 1:
                            SwitchState(AttackState.Telegraph_Death_Skulls);
                            break;
                        case 2:
                            SwitchState(AttackState.Telegraph_Shockwave);
                            break;
                    }
                }
			}
		}

		private void TelegraphCage()
		{
			ai_Counter++;
			float distance = 128;
			float particleSpeed = 8;

			Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
			Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
			var d = Dust.NewDustPerfect(position, DustID.Iron, speed, Scale: 2f);
			d.noGravity = true;

			if (ai_Counter > 60)
            {
				SwitchState(AttackState.Cage);
            }
        }

		private void AttackCage()
		{
			ai_Counter++;
			Player target = Main.player[NPC.target];

			float distance = 16 * 24;
			Vector2 nodeSpawnPosition1 = target.Center + new Vector2(-distance, -distance);
			Vector2 nodeSpawnPosition2 = target.Center + new Vector2(distance, -distance);
			Vector2 nodeSpawnPosition3 = target.Center + new Vector2(distance, distance);
			Vector2 nodeSpawnPosition4 = target.Center + new Vector2(-distance, distance);

			SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"));
			DustBurst(nodeSpawnPosition1);
			DustBurst(nodeSpawnPosition2);
			DustBurst(nodeSpawnPosition3);
			DustBurst(nodeSpawnPosition4);

			if (StellaMultiplayer.IsHost)
			{
                float x = target.Center.X;
                float y = target.Center.Y;
                var source = NPC.GetSource_FromThis();
				Projectile node1 = Projectile.NewProjectileDirect(source, nodeSpawnPosition1, Vector2.Zero,
					ModContent.ProjectileType<SparnCageNode>(), 40, 1, owner: Main.myPlayer, 0, x, y);
                Projectile node2 = Projectile.NewProjectileDirect(source, nodeSpawnPosition2, Vector2.Zero,
                    ModContent.ProjectileType<SparnCageNode>(), 40, 1, owner: Main.myPlayer, 1, x, y);
                Projectile node3 = Projectile.NewProjectileDirect(source, nodeSpawnPosition3, Vector2.Zero,
                    ModContent.ProjectileType<SparnCageNode>(), 40, 1, owner: Main.myPlayer, 2, x, y);
                Projectile node4 = Projectile.NewProjectileDirect(source, nodeSpawnPosition4, Vector2.Zero,
                    ModContent.ProjectileType<SparnCageNode>(), 40, 1, owner: Main.myPlayer, 3, x, y);
            }

            SwitchState(AttackState.Idle);
        }

		private void TelegraphDeathSkulls()
		{
			ai_Counter++;
			float distance = 128;
			float particleSpeed = 8;

			Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
			Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
			var d = Dust.NewDustPerfect(position, DustID.GemEmerald, speed, Scale: 2f);
			d.noGravity = true;

			float y = VectorHelper.Osc(0, -8);
			Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(NPC.Center, _targetCenter + new Vector2(0, y), 20);
			NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);

            if (ai_Counter > 60)
            {
				SwitchState(AttackState.Death_Skulls);
			}
		}


		private void DustBurst(Vector2 targetCenter)
        {
			for (int i = 0; i < 16; i++)
			{
				Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
				var d = Dust.NewDustPerfect(targetCenter, DustID.GemEmerald, speed, Scale: 1.5f);
				d.noGravity = true;
			}
		}

		private void AttackDeathSkulls()
		{
			Player target = Main.player[NPC.target];
			ai_Counter++;
			if(ai_Counter % 9 == 0)
            {
                if (StellaMultiplayer.IsHost)
                {
                    int numProjectiles = Main.rand.Next(1, 2);
                    for (int p = 0; p < numProjectiles; p++)
                    {
                        // Rotate the velocity randomly by 30 degrees at max.

                        Vector2 spawnPosition = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                        Vector2 velocity = VectorHelper.VelocityDirectTo(spawnPosition, target.Center, Main.rand.NextFloat(4, 12));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPosition, velocity,
                            ModContent.ProjectileType<SparnSkull>(), 34, 2, Owner: Main.myPlayer);

                        DustBurst(spawnPosition);
                        SoundEngine.PlaySound(SoundID.Item43, NPC.position);
                    }
                }

			} 
			else if (ai_Counter > 64)
            {
				SwitchState(AttackState.Idle);
			}
		}

		private void TelegraphShockwave()
        {
			float y = VectorHelper.Osc(0, -8);
			Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(NPC.Center, _targetCenter + new Vector2(0, y), 20);
			NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.2f);
			SwitchState(AttackState.Shockwave);
		}

		private void AttackShockwave()
        {
			SwitchState(AttackState.Idle);
		}


		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			base.ModifyNPCLoot(npcLoot);
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TreasureBoxTrap>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
		}

		public override void OnKill()
		{
			NPC.SetEventFlagCleared(ref DownedBossSystem.downedSparn, -1);
		}
	}
}
