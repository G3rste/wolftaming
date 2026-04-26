using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace WolfTaming
{
    public class ItemDogToy : Item
    {
        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (!byEntity.Controls.TriesToMove && byEntity.Controls.Sprint)
            {
                return;
            }
            // Not ideal to code the aiming controls this way. Needs an elegant solution - maybe an event bus?
            byEntity.Attributes.SetInt("aiming", 1);
            byEntity.Attributes.SetInt("aimingCancel", 0);
            byEntity.StartAnimation("aim");

            handling = EnumHandHandling.PreventDefault;
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            return true;
        }


        public override bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            byEntity.Attributes.SetInt("aiming", 0);
            byEntity.StopAnimation("aim");

            if (cancelReason != EnumItemUseCancelReason.ReleasedMouse)
            {
                byEntity.Attributes.SetInt("aimingCancel", 1);
            }

            return true;
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (byEntity.Attributes.GetInt("aimingCancel") == 1) return;

            byEntity.Attributes.SetInt("aiming", 0);
            byEntity.StopAnimation("aim");

            if (secondsUsed < 0.35f) return;

            (api as ICoreClientAPI)?.World.AddCameraShake(0.17f);

            ItemStack stack = slot.TakeOut(1);
            slot.MarkDirty();

            IPlayer byPlayer = null;
            if (byEntity is EntityPlayer player) byPlayer = byEntity.World.PlayerByUid(player.PlayerUID);
            byEntity.World.PlaySoundAt(new AssetLocation("sounds/player/throw"), byEntity, byPlayer, false, 8);

            EntityProperties type = byEntity.World.GetEntityType(new AssetLocation("wolftaming:dogtoy"));


            float acc = (1 - byEntity.Attributes.GetFloat("aimingAccuracy", 0));
            double rndpitch = byEntity.WatchedAttributes.GetDouble("aimingRandPitch", 1) * acc * 0.75;
            double rndyaw = byEntity.WatchedAttributes.GetDouble("aimingRandYaw", 1) * acc * 0.75;

            Vec3d pos = byEntity.Pos.XYZ.Add(0, byEntity.LocalEyePos.Y - 0.2, 0);

            Vec3d aheadPos = pos.AheadCopy(1, byEntity.Pos.Pitch + rndpitch, byEntity.Pos.Yaw + rndyaw);
            Vec3d velocity = (aheadPos - pos) * 0.65;
            Vec3d spawnPos = byEntity.Pos.BehindCopy(0.21).XYZ.Add(byEntity.LocalEyePos.X, byEntity.LocalEyePos.Y - 0.2, byEntity.LocalEyePos.Z);

            byEntity.StartAnimation("throw");
            NotifyDogs(byEntity.World.SpawnItemEntity(new ItemStack(this), spawnPos, velocity) as EntityItem);

            if (byEntity is EntityPlayer) RefillSlotIfEmpty(slot, byEntity, (itemstack) => itemstack.Collectible is ItemDogToy);

            byPlayer.Entity.World.PlaySoundAt(new AssetLocation("sounds/player/strike"), byPlayer.Entity, byPlayer, 0.9f + (float)api.World.Rand.NextDouble() * 0.2f, 16, 0.5f);
        }

        private void NotifyDogs(EntityItem dogToy)
        {
            var dogs = dogToy?.World?.GetEntitiesAround(dogToy.Pos.XYZ, 20f, 5f);
            if (dogs == null) { return; }
            foreach (var dog in dogs)
            {
                var task = dog?.GetBehavior<EntityBehaviorTaskAI>()?.TaskManager?.GetTask<AiTaskPlayFetch>();
                if (task != null) { task.DogToy = dogToy; }
            }
        }

    }
}
