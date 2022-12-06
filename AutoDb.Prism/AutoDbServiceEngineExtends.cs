using AutoDbService.Models;
using AutoDbService.DbPrism.Interfaces;
using AutoDbService.DbPrism.Models;
using System;

namespace AutoDbService.DbPrism
{
    public static class AutoDbServiceEngineExtends
    {
        public static AutoDbServiceEngine UsePrism(this AutoDbServiceEngine engine)
        {
            engine.AddType<IInfoManagerViewModel<EntityBase>, InfoManagerViewModel<EntityBase>>(false);
            engine.AddType<IBuildDynamicType, BuildDynamicType>();
            engine.AddType<IDynamicTypeClear, DynamicTypeClear>();
            
            return engine;
        }
    }
}
