using System.Collections.Generic;

namespace Genus.Migrator.Model
{
    public interface IView:IDbObject, ISqlBody, IModelNamedObject
    {
        IModel Model { get; }
    }
}