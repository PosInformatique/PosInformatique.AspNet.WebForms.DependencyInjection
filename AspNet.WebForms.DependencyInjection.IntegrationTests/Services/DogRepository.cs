//-----------------------------------------------------------------------
// <copyright file="DogRepository.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class DogRepository : IDogRepository
    {
        public DogRepository()
        {
            InstanceCount++;
        }

        public static int InstanceCount
        {
            get;
            private set;
        }

        public IReadOnlyList<Dog> GetDogs()
        {
            return new[]
            {
                new Dog() { Name = "Cachou", TatooNumber = "Cac-YS-1234" },
                new Dog() { Name = "Iris", TatooNumber = "Iri-YS-8484" },
                new Dog() { Name = "Lucifer", TatooNumber = "Luc-BR-7551" },
            };
        }
    }
}
