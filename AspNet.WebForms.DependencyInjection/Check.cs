//-----------------------------------------------------------------------
// <copyright file="Check.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.AspNet.WebForms.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Contains various check helper methods.
    /// </summary>
    internal static class Check
    {
        /// <summary>
        /// Checks if the specified <paramref name="object"/> is not <see langword="null"/>.
        /// </summary>
        /// <param name="object">Object to check the nullability.</param>
        /// <param name="argumentName">Name of the argument to check.</param>
        /// <exception cref="ArgumentNullException">If the specified <paramref name="object"/> is <see langword="null"/>.</exception>
        public static void IsNotNull(object @object, string argumentName)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
