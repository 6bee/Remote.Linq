// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remote.Linq.Dynamic
{
    partial class DynamicObject : IDynamicMetaObjectProvider
    {
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression expression)
        {
            return new MetaObject(expression, BindingRestrictions.GetInstanceRestriction(expression, this), this);
        }

        private sealed class MetaObject : DynamicMetaObject
        {
            private static readonly Type _dynamicObjectType;
            private static readonly MethodInfo _getMethod;
            private static readonly MethodInfo _setMethod;

            static MetaObject()
            {
                _dynamicObjectType = typeof(DynamicObject);
                _getMethod = _dynamicObjectType.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance);
                _setMethod = _dynamicObjectType.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance);
            }

            public MetaObject(Expression expression, BindingRestrictions restrictions, DynamicObject dynamicObject)
                : base(expression, restrictions, dynamicObject)
            {
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                var self = Expression;
                var dynObj = (DynamicObject)Value;
                var keyExpr = Expression.Constant(binder.Name);
                var target = Expression.Call(Expression.Convert(self, _dynamicObjectType), _getMethod, keyExpr);
                return new DynamicMetaObject(target, BindingRestrictions.GetTypeRestriction(self, _dynamicObjectType));
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                var self = Expression;
                var keyExpr = Expression.Constant(binder.Name);
                var valueExpr = Expression.Convert(value.Expression, typeof(object));
                var target = Expression.Call(Expression.Convert(self, _dynamicObjectType), _setMethod, keyExpr, valueExpr);
                return new DynamicMetaObject(target, BindingRestrictions.GetTypeRestriction(self, _dynamicObjectType));
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                var dynObj = (DynamicObject)Value;
                return dynObj.Keys.ToList();
            }
        }
    }
}
