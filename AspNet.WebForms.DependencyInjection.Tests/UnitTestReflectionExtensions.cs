//-----------------------------------------------------------------------
// <copyright file="UnitTestReflectionExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.AspNet.WebForms.DependencyInjection.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    internal static class UnitTestReflectionExtensions
    {
        public static T GetFieldValue<T>(this object instance, string name)
        {
            return (T)instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }

        public static T GetStaticValue<T>(this Type type, string name)
        {
            return (T)type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        }
    }
}
