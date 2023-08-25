using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace Wolftaming
{
    public class AiTaskStayCloseToShepherd : AiTaskStayCloseToEntity
    {
        public Entity shepherd;
        private long lastCheck = 0;
        public AiTaskStayCloseToShepherd(EntityAgent entity) : base(entity)
        {
        }

        public override bool ShouldExecute()
        {
            if (lastCheck + 20000 < entity.Api.World.ElapsedMilliseconds && entity.WatchedAttributes.GetInt("generation", 0) > 0)
            {
                lastCheck = entity.World.ElapsedMilliseconds;
                var candidate = entity.World.GetNearestEntity(entity.ServerPos.XYZ, range, 2, (e) =>
                {
                    if (entityCode.EndsWith("*"))
                    {
                        return e.Code.Path.StartsWith(entityCode.Substring(0, entityCode.Length - 1));
                    }
                    else
                    {
                        return e.Code.Path.Equals(entityCode);
                    }
                });
                if (candidate != null)
                {
                    targetEntity = candidate;
                }
            }
            if (targetEntity != null)
            {
                double x = targetEntity.ServerPos.X;
                double y = targetEntity.ServerPos.Y;
                double z = targetEntity.ServerPos.Z;

                double dist = entity.ServerPos.SquareDistanceTo(x, y, z);

                if(dist > maxDistance * maxDistance * 4){
                    targetEntity = null;
                    return false;
                }

                return dist > maxDistance * maxDistance;
            }
            return false;
        }
    }
}