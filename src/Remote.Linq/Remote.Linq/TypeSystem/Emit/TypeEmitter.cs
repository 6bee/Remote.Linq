// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TypeSystem.Emit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using TypeInfo = Remote.Linq.TypeSystem.TypeInfo;

    public sealed partial class TypeEmitter
    {
        private static readonly TypeCache _typeCache = new TypeCache();

        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _module;
        private int _classIndex = -1;

        public TypeEmitter()
        {
            var appDomain = AppDomain.CurrentDomain;
            var assemblyName = new AssemblyName("Remote.Linq.TypeSystem.Emit.Types");
            var assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            var module = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            _assemblyBuilder = assemblyBuilder;
            _module = module;
        }

        public Type EmitType(TypeInfo typeInfo)
        {
            if (typeInfo.IsAnonymousType)
            {
                var properties = typeInfo.Properties.Select(x => x.Name).ToList();
                return _typeCache.GetOrCreate(properties, InternalEmitAnonymousType);
            }
            else
            {
                return _typeCache.GetOrCreate(typeInfo, InternalEmitType);
            }
        }

        private Type InternalEmitType(TypeInfo typeInfo)
        {
            var fullName = CreateUniqueClassName(typeInfo);

            var propertyInfos = typeInfo.Properties.ToArray();

            // define type
            var type = _module.DefineType(fullName, TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, typeof(object));

            // define fields
            var fields = propertyInfos
                .Select(x => type.DefineField(string.Format("_{0}", x.Name), x.PropertyType.Type, FieldAttributes.Private))
                .ToArray();

            // define default constructor
            var constructor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            var il = constructor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, objectCtor);
            il.Emit(OpCodes.Ret);

            // define properties
            var properties = propertyInfos
                .Select((x, i) =>
                {
                    var property = type.DefineProperty(x.Name, PropertyAttributes.HasDefault, x.PropertyType.Type, null);

                    var propertyGetter = type.DefineMethod(string.Format("get_{0}", x.Name), MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, x.PropertyType.Type, null);
                    var getterPil = propertyGetter.GetILGenerator();
                    getterPil.Emit(OpCodes.Ldarg_0);
                    getterPil.Emit(OpCodes.Ldfld, fields[i]);
                    getterPil.Emit(OpCodes.Ret);

                    property.SetGetMethod(propertyGetter);

                    var propertySetter = type.DefineMethod(string.Format("set_{0}", x.Name), MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { x.PropertyType.Type });
                    var setterPil = propertySetter.GetILGenerator();
                    setterPil.Emit(OpCodes.Ldarg_0);
                    setterPil.Emit(OpCodes.Ldarg_1);
                    setterPil.Emit(OpCodes.Stfld, fields[i]);
                    setterPil.Emit(OpCodes.Ret);

                    property.SetSetMethod(propertySetter);

                    return property;
                })
                .ToArray();

            // create type
            var t1 = type.CreateType();
            return t1;
        }

        private Type InternalEmitAnonymousType(IEnumerable<string> propertyNames)
        {
            if (!propertyNames.Any())
            {
                throw new Exception("No properties specified");
            }

            var fullName = CreateUniqueClassNameForAnonymousType(propertyNames);

            // define type
            var type = _module.DefineType(fullName, TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, typeof(object));

            // define generic parameters
            var genericTypeParameterNames = propertyNames.Select((x, i) => string.Format("T{0}", i)).ToArray();
            var genericTypeParameters = type.DefineGenericParameters(genericTypeParameterNames);

            // define fields
            var fields = propertyNames
                .Select((x, i) => type.DefineField(string.Format("_{0}", x), genericTypeParameters[i], FieldAttributes.Private | FieldAttributes.InitOnly))
                .ToArray();

            // define constructor
            var constructor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, genericTypeParameters);
            var objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            var il = constructor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, objectCtor);
            for (int i = 0; i < fields.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg, i + 1);
                il.Emit(OpCodes.Stfld, fields[i]);
            }
            il.Emit(OpCodes.Ret);

            // define properties
            var properties = propertyNames
                .Select((x, i) =>
                {
                    var property = type.DefineProperty(x, PropertyAttributes.HasDefault, genericTypeParameters[i], null);

                    var propertyGetter = type.DefineMethod(string.Format("get_{0}", x), MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, genericTypeParameters[i], null);
                    var pil = propertyGetter.GetILGenerator();
                    pil.Emit(OpCodes.Ldarg_0);
                    pil.Emit(OpCodes.Ldfld, fields[i]);
                    pil.Emit(OpCodes.Ret);

                    property.SetGetMethod(propertyGetter);

                    return property;
                })
                .ToArray();

            // create type
            var t1 = type.CreateType();
            return t1;
        }

        private string CreateUniqueClassName(TypeInfo typeInfo)
        {
            var typeFullName = typeInfo.FullName;
            string suffix = null;

            var type = _module.GetType(typeFullName);
            if (!ReferenceEquals(null, type))
            {
                var id = Interlocked.Increment(ref _classIndex);
                suffix = string.Format("_{0}", id);
            }

            var name = string.Format("{1}{2}", _module.Name, typeFullName, suffix);
            return name;
        }

        private string CreateUniqueClassNameForAnonymousType(IEnumerable<string> properties)
        {
            var id = Interlocked.Increment(ref _classIndex);
            var fullName = string.Format("{0}.__EmittedType__{1}`{2}", _module.Name, id, properties.Count());
            return fullName;
        }
    }
}
