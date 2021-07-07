// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeExtensions;
    using System;
    using System.Linq;
    using System.Reflection;

    internal static partial class MethodInfos
    {
        internal const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        internal const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        internal const BindingFlags AnyStatic = PublicStatic | PrivateStatic;

        internal const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
        internal const BindingFlags PrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance;
        internal const BindingFlags AnyInstance = PublicInstance | PrivateInstance;

        internal const BindingFlags Any = AnyInstance | AnyStatic;

        /// <summary>
        /// Get <see cref="MethodInfo"/> using reflection.
        /// </summary>
        /// <remarks>
        /// This method should be used for types and method controlled by this library only.
        /// For external types, overload should be used to specify exact method signature.
        /// </remarks>
        internal static MethodInfo GetMethod(Type declaringType, string name, BindingFlags bindingFlags = Any)
            => GetMethodCore(declaringType, name, x => true, bindingFlags);

        /// <summary>
        /// Get <see cref="MethodInfo"/> using reflection.
        /// </summary>
        internal static MethodInfo GetMethod(Type declaringType, string name, params Type[] parameters)
            => GetMethod(declaringType, name, Array.Empty<Type>(), parameters, Any);

        /// <summary>
        /// Get <see cref="MethodInfo"/> using reflection.
        /// </summary>
        internal static MethodInfo GetMethod(Type declaringType, string name, Type[] genericArguments, params Type[] parameters)
            => GetMethod(declaringType, name, genericArguments, parameters, Any);

        internal static MethodInfo GetMethod(Type declaringType, string name, Type[] genericArguments, Type[] parameters, BindingFlags bindingFlags)
        {
            try
            {
                return GetMethodCore(declaringType, name, x => ParametersMatch(x, genericArguments, parameters), bindingFlags);
            }
            catch (Exception ex)
            {
                static string TypesToSzring(Type[] types) => string.Join(", ", types.Select(x => x.PrintFriendlyName(false, false)));

                var genericArgumentString = TypesToSzring(genericArguments);
                if (!string.IsNullOrEmpty(genericArgumentString))
                {
                    genericArgumentString = $"<{genericArgumentString}>";
                }

                var parametersString = TypesToSzring(parameters);

                throw new InvalidOperationException($"Failed to get MethodInfo '{bindingFlags} {declaringType}.{name}{genericArgumentString}({parametersString})'", ex);
            }
        }

        private static MethodInfo GetMethodCore(Type declaringType, string name, Func<MethodInfo, bool> filter, BindingFlags bindingFlags)
            => declaringType
            .GetMethods(bindingFlags)
            .Single(x => string.Equals(x.Name, name, StringComparison.Ordinal) && filter(x));

        private static bool ParametersMatch(MethodInfo method, Type[] genericArgumentTypes, Type[] parameterTypes)
        {
            method.AssertNotNull(nameof(method));
            genericArgumentTypes.AssertItemsNotNull(nameof(genericArgumentTypes));
            parameterTypes.AssertItemsNotNull(nameof(parameterTypes));

            if (method.IsGenericMethod)
            {
                if (method.GetGenericArguments().Length != genericArgumentTypes.Length)
                {
                    return false;
                }

                method = method.MakeGenericMethod(genericArgumentTypes);
            }
            else if (genericArgumentTypes.Length > 0)
            {
                return false;
            }

            var parameters = method.GetParameters();
            if (parameters?.Length != parameterTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var expectedType = parameterTypes[i];
                if (parameterType != expectedType)
                {
                    return false;
                }
            }

            return true;
        }
    }
}