using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    //So to get a proper moving platform
    //We need to move the player's position directly without affecting their velocity
    //Then have the bottom thing be treated has a solid object that can't be moved past
    //So it should act as moving ground since the y velocity is never changing

    public struct ElevatorBounds
    {
        public Rectangle rectangle;
        public Vector2 velocity;
    }
    public interface IElevator
    {
        ElevatorBounds GetElevatorBounds();
    }

    public class ElevatorSystem : ModSystem
    {
        private static List<ElevatorBounds> _elevators;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _elevators = new List<ElevatorBounds>();
            On_Player.SlopingCollision += ElevatorLogic;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Player.SlopingCollision -= ElevatorLogic;
        }

        public static void AddElevator(ElevatorBounds elevator)
        {
            _elevators.Add(elevator);
        }
        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
            _elevators.Clear();
        }

        private void ElevatorLogic(On_Player.orig_SlopingCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            foreach(ElevatorBounds elevator in _elevators)
            {
                Rectangle playerRectangle = self.getRect();
                playerRectangle.Height = 8;
                if (playerRectangle.Intersects(elevator.rectangle))
                {
                    if (!self.justJumped && self.velocity.Y >= 0)
                    {
                        self.velocity.Y = 0;
                        self.position.Y = (elevator.rectangle.Center().Y) - self.height / 2;
                        self.position.Y += elevator.velocity.Y;
                        if (elevator.velocity.Y > 0)
                        {
                            self.position.Y += self.height / 2;
                        }
                    }
                }
            }

            orig(self, fallThrough, ignorePlats);
        }
    }

    public class PunkerElevator : ModNPC
    {
        private enum AIState
        {
            Idle,
            Up,
            Down
        }
        private ref float Timer => ref NPC.ai[0];
        private Vector2 MoveTarget
        {
            get
            {
                Vector2 target = new Vector2();
                target.X = NPC.ai[1];
                target.Y = NPC.ai[2];
                return target;
            }
            set
            {
                NPC.ai[1] = value.X;
                NPC.ai[2] = value.Y;
            }
        }

        private AIState State
        {
            get
            {
                return (AIState)NPC.ai[3];
            }
            set
            {
                NPC.ai[3] = (float)value;
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.friendly = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.damage = 1;
            NPC.defense = 1;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 1f;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            NPC.velocity.Y = MathHelper.Lerp(0, 5, EasingFunction.InOutSine(Timer/120));
            if (Timer > 2)
            {
                ElevatorBounds elevatorBounds = new ElevatorBounds
                {
                    rectangle = GetPlatformRectangle(),
                    velocity = NPC.velocity
                };
                ElevatorSystem.AddElevator(elevatorBounds);
            }

            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Up:
                    AI_Up();
                    break;
                case AIState.Down:
                    AI_Down();
                    break;
            }
        }

        private Rectangle GetPlatformRectangle()
        {
            Rectangle rectangle = new Rectangle(0, 0, 96, 16);
            Vector2 rectangleStart = NPC.Center - new Vector2(rectangle.Width / 2f, 0);
            rectangle.X = (int)rectangleStart.X;
            rectangle.Y = (int)rectangleStart.Y;
            return rectangle;
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void AI_Idle()
        {

        }

        private void AI_Up()
        {
            Timer++;

        }

        private void AI_Down()
        {
            Timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Rectangle? frame = null;
            Vector2 drawOrigin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            Vector2 drawCenter = NPC.Center - screenPos;
            drawCenter.Y -= 4;
            spriteBatch.Draw(texture, drawCenter, null, drawColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
            return false;
        }
    }
}
