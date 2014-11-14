// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Remote.Linq.Dynamic
{
    public interface IDynamicObjectMapper
    {
        /// <summary>
        /// Maps a <see cref="DynamicObject"/> into a collection of objects
        /// </summary>
        /// <param name="obj"><see cref="DynamicObject"/> to be mapped</param>
        /// <param name="targetType">Target type for mapping, set this parameter to null if type information included within <see cref="DynamicObject"/> should be used.</param>
        /// <returns>The object created based on the <see cref="DynamicObject"/> specified</returns>
        object Map(DynamicObject obj, Type targetType = null);

        /// <summary>
        /// Maps a <see cref="DynamicObject"/> into an instance of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The target type in which the <see cref="DynamicObject"/> have to be mapped to</typeparam>
        /// <param name="obj"><see cref="DynamicObject"/> to be mapped</param>
        /// <returns>The object created based on the <see cref="DynamicObject"/> specified</returns>
        T Map<T>(DynamicObject obj);

        /// <summary>
        /// Maps a collection of <see cref="DynamicObject"/>s into a collection of objects
        /// </summary>
        /// <param name="objects">Collection of <see cref="DynamicObject"/> to be mapped</param>
        /// <param name="targetType">Target type for mapping, set this parameter to null if type information included within individual <see cref="DynamicObject"/>s should be used.</param>
        /// <returns>Collection of objects created based on the <see cref="DynamicObject"/>s specified</returns>
        System.Collections.IEnumerable Map(IEnumerable<DynamicObject> objects, Type targetType = null);

        /// <summary>
        /// Maps a collection of <see cref="DynamicObject"/>s into a collection of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The target type in which the <see cref="DynamicObject"/> have to be mapped to</typeparam>
        /// <param name="objects">Collection of <see cref="DynamicObject"/>s to be mapped</param>
        /// <returns>Collection of <typeparamref name="T"/> created based on the <see cref="DynamicObject"/>s specified</returns>
        IEnumerable<T> Map<T>(IEnumerable<DynamicObject> objects);

        /// <summary>
        /// Mapps the specified instance into a <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="obj">The instance to be mapped</param>
        /// <param name="setTypeInformation">Set this parameter to true if type information should be included within the <see cref="DynamicObject"/>, set it to false otherwise.</param>
        /// <returns>An instance of <see cref="DynamicObject"/> representing the mapped instance</returns>
        DynamicObject MapObject(object obj, bool setTypeInformation = true);

        /// <summary>
        /// Maps a collection of objects into a collection of <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="objects">The objects to be mapped</param>
        /// <param name="setTypeInformation">Set this parameter to true if type information should be included within the <see cref="DynamicObject"/>s, set it to false otherwise.</param>
        /// <returns>A collection of <see cref="DynamicObject"/> representing the objects specified</returns>
        IEnumerable<DynamicObject> MapCollection(object objects, bool setTypeInformation = true);
    }
}
