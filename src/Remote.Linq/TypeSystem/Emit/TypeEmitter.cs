// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Remote.Linq.TypeSystem.Emit
{
    internal sealed partial class TypeEmitter
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

        internal Type EmitType(TypeInfo typeInfo)
        {
            return EmitType(typeInfo.Properties);
        }

        public Type EmitType(params string[] properties)
        {
            return EmitType((IEnumerable<string>)properties);
        }

        public Type EmitType(IEnumerable<string> properties)
        {
            // the call to InternalEmitType is intercepted by a cache, to serve an existing type if available
            return _typeCache.GetOrCreate(properties, InternalEmitType);
        }

        private Type InternalEmitType(IEnumerable<string> properties)
        {
            var name = CreateUniqueClassName(properties);
            return InternalEmitType(name, properties);
        }

        private string CreateUniqueClassName(IEnumerable<string> properties)
        {
            var id = Interlocked.Increment(ref _classIndex);
            var name = string.Format("__EmittedType__{0}`{1}", id, properties.Count());
            return name;
        }

        private Type InternalEmitType(string name, IEnumerable<string> args)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Type name must not be empty", "name");
            if (ReferenceEquals(null, args) || !args.Any()) throw new ArgumentException("No properties specified", "properties");

            // define type
            var fullName = string.Format("{0}.{1}", _module.Name, name);
            var type = _module.DefineType(fullName, TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed, typeof(object));

            // define generic parameters
            var genericTypeParameterNames = args.Select((x, i) => string.Format("T{0}", i)).ToArray();
            var genericTypeParameters = type.DefineGenericParameters(genericTypeParameterNames);

            // define fields
            var fields = args
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
            var properties = args
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
    }
}
