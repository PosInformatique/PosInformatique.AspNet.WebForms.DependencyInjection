//-----------------------------------------------------------------------
// <copyright file="DogManager.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class DogManager : IDogManager
    {
        private readonly IDogRepository repository;

        public DogManager(IDogRepository repository)
        {
            InstanceCount++;
            this.repository = repository;
        }

        public static int InstanceCount
        {
            get;
            private set;
        }

        public IReadOnlyList<Dog> GetDogs()
        {
            return this.repository.GetDogs();
        }
    }
}
