using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Wolftaming
{
    public class AiTaskStayCloseToShepherd : AiTaskStayCloseToEntity
    {
        public Entity shepherd;
        private long lastCheck = 0;
        public AiTaskStayCloseToShepherd(EntityAgent entity, JsonObject taskConfig, JsonObject aiConfig) : base(entity, taskConfig, aiConfig)
        {
        }

        public override bool ShouldExecute()
        {
            if (lastCheck + 20000 < entity.Api.World.ElapsedMilliseconds && entity.WatchedAttributes.GetInt("generation", 0) > 0)
            {
                lastCheck = entity.World.ElapsedMilliseconds;
                var candidate = entity.World.GetNearestEntity(entity.Pos.XYZ, range, 2, e => e?.Properties?.Attributes?["IsShepherd"].AsBool() == true);

                if (candidate != null)
                {
                    targetEntity = candidate;
                }
            }
            if (targetEntity != null)
            {
                double x = targetEntity.Pos.X;
                double y = targetEntity.Pos.Y;
                double z = targetEntity.Pos.Z;

                double dist = entity.Pos.SquareDistanceTo(x, y, z);

                if (dist > maxDistance * maxDistance * 4)
                {
                    targetEntity = null;
                    return false;
                }

                return dist > maxDistance * maxDistance;
            }
            return false;
        }
    }
}
