using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Wolftaming
{
    public class AiTaskPlayFetch : AiTaskBase
    {
        public EntityItem dogToy { get; set; }

        private Entity owner { get; set; }

        public float moveSpeed = 0.03f;

        bool stuck = false;
        public AiTaskPlayFetch(EntityAgent entity, JsonObject taskConfig, JsonObject aiConfig) : base(entity, taskConfig, aiConfig)
        {
            if (taskConfig["movespeed"] != null)
            {
                moveSpeed = taskConfig["movespeed"].AsFloat(0.03f);
            }
        }

        public override bool ShouldExecute()
        {
            if (dogToy == null || !dogToy.Alive || dogToy.ShouldDespawn) return false;
            if (entity.ServerPos.SquareDistanceTo(dogToy.ServerPos) > 30 * 30)
            {
                dogToy = null;
            }
            return dogToy != null;
        }
        public override void StartExecute()
        {
            base.StartExecute();

            float size = dogToy.CollisionBox.XSize;
            pathTraverser.WalkTowards(dogToy.ServerPos.XYZ, moveSpeed, size + 0.2f, OnGoalReached, OnStuck);

            stuck = false;
        }

        public override bool ContinueExecute(float dt)
        {
            if (dogToy != null)
            {
                return getToy();
            }
            else
            {
                return bringToy();
            }
        }
        private bool getToy()
        {
            if (dogToy == null || !dogToy.Alive || stuck)
            {
                pathTraverser.Stop();
                return false;
            }

            double x = dogToy.ServerPos.X;
            double y = dogToy.ServerPos.Y;
            double z = dogToy.ServerPos.Z;

            pathTraverser.CurrentTarget.X = x;
            pathTraverser.CurrentTarget.Y = y;
            pathTraverser.CurrentTarget.Z = z;

            if (entity.ServerPos.SquareDistanceTo(x, y, z) < 2)
            {
                var slot = new DummySlot(dogToy.Itemstack);
                if (slot.TryPutInto(entity.World, entity.LeftHandItemSlot) > 0)
                {
                    dogToy.Die(EnumDespawnReason.PickedUp);
                }
                dogToy = null;
                increaseObedience();
                return true;
            }

            return pathTraverser.Active;
        }

        private void increaseObedience()
        {
            var domesticationStatus = entity.WatchedAttributes.GetOrAddTreeAttribute("domesticationstatus");
            domesticationStatus.SetFloat("obedience", domesticationStatus.GetFloat("obedience") + 0.03f);
            entity.WatchedAttributes.MarkPathDirty("domesticationstatus");
        }

        private bool bringToy()
        {
            if (owner == null || !owner.Alive || owner.ShouldDespawn)
            {
                owner = entity.World.GetNearestEntity(entity.ServerPos.XYZ, 50, 10, entity => entity is EntityPlayer);
                if (owner == null) return false;
            }

            double x = owner.ServerPos.X;
            double y = owner.ServerPos.Y;
            double z = owner.ServerPos.Z;

            pathTraverser.CurrentTarget.X = x;
            pathTraverser.CurrentTarget.Y = y;
            pathTraverser.CurrentTarget.Z = z;

            if (entity.ServerPos.SquareDistanceTo(x, y, z) < 2)
            {
                pathTraverser.Stop();
                owner = null;
                return false;
            }

            return owner.Alive && !stuck && pathTraverser.Active;
        }
        private void OnStuck()
        {
            stuck = true;
        }

        private void OnGoalReached()
        {
        }
    }
}