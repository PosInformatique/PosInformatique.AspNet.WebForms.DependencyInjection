//-----------------------------------------------------------------------
// <copyright file="IDogRepository.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public interface IDogManager
    {
        IReadOnlyList<Dog> GetDogs();
    }
}
