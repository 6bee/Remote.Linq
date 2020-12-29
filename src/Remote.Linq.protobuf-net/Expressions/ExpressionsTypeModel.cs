// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.Expressions
{
    using Aqua.EnumerableExtensions;
    using Aqua.ProtoBuf;
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class ExpressionsTypeModel
    {
        public static AquaTypeModel ConfigureRemoteLinqExpressionTypes(this AquaTypeModel typeModel)
            => typeModel
            .RegisterBaseAndSubtypes<MemberBinding>()
            .RegisterBaseAndSubtypes<Expression>()
            .AddTypeSurrogate<ConstantExpression, ConstantExpressionSurrogate>();

        /// <summary>Register a base class and it's subtypes from the same assebly, uncluding all their serializable fields and proeprties.</summary>
        private static AquaTypeModel RegisterBaseAndSubtypes<T>(this AquaTypeModel typeModel)
        {
            var resiteredTypes = new HashSet<Type>();
            var pendingTypes = new HashSet<Type>();

            void CollectType(Type t)
            {
                t = TypeHelper.GetElementType(t) ?? t;

                if (resiteredTypes.Contains(t) || pendingTypes.Contains(t))
                {
                    return;
                }

                pendingTypes.Add(t);
                t.GetDefaultPropertiesForSerialization().Select(x => x.PropertyType).ForEach(CollectType);
                t.GetDefaultFieldsForSerialization().Select(x => x.FieldType).ForEach(CollectType);
            }

            void RegisterBaseAndSubtypes(Type baseType)
            {
                if (resiteredTypes.Contains(baseType))
                {
                    return;
                }

                resiteredTypes.Add(baseType);

                var expressionTypes = baseType.Assembly
                    .GetTypes()
                    .Where(x => x.BaseType == baseType)
                    .OrderBy(x => x.FullName)
                    .ToArray();

                foreach (var t in expressionTypes)
                {
                    typeModel.AddSubType(baseType, t);
                    RegisterBaseAndSubtypes(t);
                    CollectType(t);
                }
            }

            RegisterBaseAndSubtypes(typeof(T));

            pendingTypes.ForEach(x => _ = typeModel.GetType(x));

            return typeModel;
        }
    }
}
