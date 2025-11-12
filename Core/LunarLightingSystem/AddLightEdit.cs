using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class AddLightEdit : ModSystem
    {
        public override void Load()
        {
            On_Lighting.AddLight_int_int_float_float_float += NoAddLight;
            On_Lighting.AddLight_int_int_int_float += NoAddLight;
            On_Lighting.AddLight_Vector2_int += NoAddLight;
            On_Lighting.AddLight_Vector2_Vector3 += NoAddLight;
            On_Lighting.AddLight_Vector2_float_float_float += NoAddLight;
        }


        public override void Unload()
        {
            On_Lighting.AddLight_int_int_float_float_float -= NoAddLight;
            On_Lighting.AddLight_int_int_int_float -= NoAddLight;
            On_Lighting.AddLight_Vector2_int -= NoAddLight;
            On_Lighting.AddLight_Vector2_Vector3 -= NoAddLight;
            On_Lighting.AddLight_Vector2_float_float_float -= NoAddLight;
        }
        private void NoAddLight(On_Lighting.orig_AddLight_Vector2_float_float_float orig, Vector2 position, float r, float g, float b)
        {
            /*
            TileAmbientLight tileAmbientLight = new TileAmbientLight();
            tileAmbientLight.position = position;
            tileAmbientLight.color = new Color(r, g, b);
            AddAmbientLight(tileAmbientLight);*/
        }

        private void NoAddLight(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            /*
            TileAmbientLight tileAmbientLight = new TileAmbientLight();
            tileAmbientLight.position = new Vector2(i * 16, j * 16);
            tileAmbientLight.color = new Color(r, g, b);
            AddAmbientLight(tileAmbientLight);*/
        }
        private void NoAddLight(On_Lighting.orig_AddLight_Vector2_Vector3 orig, Vector2 position, Vector3 rgb)
        {
            /*
            TileAmbientLight tileAmbientLight = new TileAmbientLight();
            tileAmbientLight.position = position;
            tileAmbientLight.color = new Color(rgb);
            AddAmbientLight(tileAmbientLight);*/
        }

        private void NoAddLight(On_Lighting.orig_AddLight_Vector2_int orig, Vector2 position, int torchID)
        {

        }

        private void NoAddLight(On_Lighting.orig_AddLight_int_int_int_float orig, int i, int j, int torchID, float lightAmount)
        {

        }

    }
}
