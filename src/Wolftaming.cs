using System;
using System.Net.NetworkInformation;
using System.Xml;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Wolftaming
{
    public class Wolftaming : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterItemClass("dogtoy", typeof(ItemDogToy));

            AiTaskRegistry.Register<AiTaskPlayFetch>("playfetch");
            AiTaskRegistry.Register<AiTaskStayCloseToShepherd>("stayclosetoshepherd");
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Event.PlayerCreate += player => giveSecretDoggo(api, player);
        }

        private void giveSecretDoggo(ICoreServerAPI api, IServerPlayer player)
        {
            // Peachwolf updated the models -> Peachwolf gets his very own peach wolf
            if (player.PlayerUID == "eJNd6SdFvvE028GvnRfJM2Yp")
            {
                player.InventoryManager.TryGiveItemstack(new ItemStack(api.World.GetItem(new AssetLocation("wolftaming:creature-dog-peach-female"))));
            }
        }
    }
}
