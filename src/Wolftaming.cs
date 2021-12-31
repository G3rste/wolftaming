using Vintagestory.API.Common;
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
        }
    }
}
