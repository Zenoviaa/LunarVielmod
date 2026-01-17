using Stellamod.Core.Grass;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public class TileSets : ModSystem
    {
        private static bool _lastSuccess;
        private static int _lastLookup = -1;
        private static GrassProfile _lastProfile;
        public override void SetupContent()
        {
            GrassyTiles = new Dictionary<int, GrassProfile>();
            base.SetupContent();
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            GrassyTiles.Clear();
            GrassyTiles = null;
            _lastProfile = null;
        }

        public static Dictionary<int, GrassProfile> GrassyTiles { get; private set; }
        public static void RegisterGrassyTile<T>(int type) where T : GrassProfile
        {
            GrassyTiles.Add(type, ModContent.GetInstance<T>());
        }
        public static bool GetGrassProfile(int type, out GrassProfile profile)
        {

            if (_lastLookup == type)
            {

                profile = _lastProfile;
                return _lastSuccess;
            }

      
            _lastLookup = type;

            bool success = GrassyTiles.TryGetValue(type, out profile);
            _lastSuccess = success;
            _lastProfile = profile;
            return success;
        }
    }
}
