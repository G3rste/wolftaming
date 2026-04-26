using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace WolfTaming
{
    public class AiTaskPlayFetch : AiTaskBase
    {
        public EntityItem DogToy { get; set; }

        private Entity Owner { get; set; }

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
            if (DogToy == null || !DogToy.Alive || DogToy.ShouldDespawn) return false;
            if (entity.Pos.SquareDistanceTo(DogToy.Pos) > 30 * 30)
            {
                DogToy = null;
            }
            return DogToy != null;
        }
        public override void StartExecute()
        {
            base.StartExecute();

            float size = DogToy.CollisionBox.XSize;
            pathTraverser.WalkTowards(DogToy.Pos.XYZ, moveSpeed, size + 0.2f, OnGoalReached, OnStuck);

            stuck = false;
        }

        public override bool ContinueExecute(float dt)
        {
            if (DogToy != null)
            {
                return GetToy();
            }
            else
            {
                return BringToy();
            }
        }
        private bool GetToy()
        {
            if (DogToy == null || !DogToy.Alive || stuck)
            {
                pathTraverser.Stop();
                return false;
            }

            double x = DogToy.Pos.X;
            double y = DogToy.Pos.Y;
            double z = DogToy.Pos.Z;

            pathTraverser.CurrentTarget.X = x;
            pathTraverser.CurrentTarget.Y = y;
            pathTraverser.CurrentTarget.Z = z;

            if (entity.Pos.SquareDistanceTo(x, y, z) < 2)
            {
                var slot = new DummySlot(DogToy.Itemstack);
                if (slot.TryPutInto(entity.World, entity.LeftHandItemSlot) > 0)
                {
                    DogToy.Die(EnumDespawnReason.PickedUp);
                }
                DogToy = null;
                IncreaseObedience();
                return true;
            }

            return pathTraverser.Active;
        }

        private void IncreaseObedience()
        {
            var domesticationStatus = entity.WatchedAttributes.GetOrAddTreeAttribute("domesticationstatus");
            domesticationStatus.SetFloat("obedience", domesticationStatus.GetFloat("obedience") + 0.03f);
            entity.WatchedAttributes.MarkPathDirty("domesticationstatus");
        }

        private bool BringToy()
        {
            if (Owner == null || !Owner.Alive || Owner.ShouldDespawn)
            {
                Owner = entity.World.GetNearestEntity(entity.Pos.XYZ, 50, 10, entity => entity is EntityPlayer);
                if (Owner == null) return false;
            }

            double x = Owner.Pos.X;
            double y = Owner.Pos.Y;
            double z = Owner.Pos.Z;

            pathTraverser.CurrentTarget.X = x;
            pathTraverser.CurrentTarget.Y = y;
            pathTraverser.CurrentTarget.Z = z;

            if (entity.Pos.SquareDistanceTo(x, y, z) < 2)
            {
                pathTraverser.Stop();
                Owner = null;
                return false;
            }

            return Owner.Alive && !stuck && pathTraverser.Active;
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
