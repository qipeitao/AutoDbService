using AutoDbService.Models;
using AutoDbService.Prism.Interfaces;
using AutoDbService.Prism.Models;
using System;

namespace AutoDbService.Prism
{
    public static class AutoDbServiceEngineExtends
    {
        public static AutoDbServiceEngine UsePrism(this AutoDbServiceEngine engine)
        {
            engine.AddType<IInfoManagerViewModel<EntityBase>, InfoManagerViewModel<EntityBase>>(false);

            return engine;
        }
    }
}
