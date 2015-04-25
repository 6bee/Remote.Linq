namespace Remote.Linq.Dynamic
{
    using System.Linq;
    using ProtoBuf.Meta;

    internal static class DynamicObjectTypeModel
    {
        public static RuntimeTypeModel ConfigureDynamicObject(this RuntimeTypeModel typeModel)
        {
            var type = typeModel[typeof(DynamicObject.Property)];

            type.GetFields().Single(x => string.Equals(x.Name, "Value")).DynamicType = true;

            return typeModel;
        }
    }
}
