//-----------------------------------------------------------------------
// <copyright file="ActivatorUtilitiesConstructorTypeAdapterTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.AspNet.WebForms.DependencyInjection.Tests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class ActivatorUtilitiesConstructorTypeAdapterTest
    {
        [Fact]
        public void GetConstructors_NonPublicInstance()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            constructors.Should().HaveCount(2);
            constructors[0].GetParameters().Single(p => p.Name == "isPrivate1").Should().NotBeNull();
            constructors[1].GetParameters().Single(p => p.Name == "isPrivate2").Should().NotBeNull();

            type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Should().Equal(constructors);

            // Compare with the .NET behavior
            constructors = typeof(ConstructorTest).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            constructors.Should().HaveCount(2);
            constructors[0].GetParameters().Single(p => p.Name == "isPrivate1").Should().NotBeNull();
            constructors[1].GetParameters().Single(p => p.Name == "isPrivate2").Should().NotBeNull();
        }

        [Fact]
        public void GetConstructors_PublicInstance()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            constructors.Should().HaveCount(2);
            constructors[0].GetParameters().Single(p => p.Name == "isPublic1").Should().NotBeNull();
            constructors[1].GetParameters().Single(p => p.Name == "isPublic2").Should().NotBeNull();

            type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Should().Equal(constructors);

            // Compare with the .NET behavior
            constructors = typeof(ConstructorTest).GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            constructors.Should().HaveCount(2);
            constructors[0].GetParameters().Single(p => p.Name == "isPublic1").Should().NotBeNull();
            constructors[1].GetParameters().Single(p => p.Name == "isPublic2").Should().NotBeNull();
        }

        [Fact]
        public void GetConstructors_NonPublicStatic()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Static);

            constructors.Should().HaveCount(1);
            constructors[0].GetParameters().Should().BeEmpty();

            type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Static).Should().Equal(constructors);

            // Compare with the .NET behavior
            constructors = typeof(ConstructorTest).GetConstructors(BindingFlags.NonPublic | BindingFlags.Static);

            constructors.Should().HaveCount(1);
            constructors[0].GetParameters().Should().BeEmpty();
        }

        [Fact]
        public void GetConstructors_PublicStatic()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Static);

            constructors.Should().HaveCount(0);

            type.GetConstructors(BindingFlags.Public | BindingFlags.Static).Should().Equal(constructors);

            // Compare with the .NET behavior
            constructors = typeof(ConstructorTest).GetConstructors(BindingFlags.Public | BindingFlags.Static);

            constructors.Should().HaveCount(0);
        }

        [Fact]
        public void GetConstructors_NoMatchingVisibility()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorForNoMatchingVisibilityTest));

            var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Static);

            constructors.Should().HaveCount(0);

            type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Static).Should().Equal(constructors);

            // Compare with the .NET behavior
            constructors = typeof(ConstructorForNoMatchingVisibilityTest).GetConstructors(BindingFlags.NonPublic | BindingFlags.Static);

            constructors.Should().HaveCount(0);
        }

        [Fact]
        public void GetConstructors_ParameterlessInstance()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorParameterless));

            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            constructors.Should().HaveCount(1);
            constructors[0].IsPublic.Should().BeTrue();

            type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Should().Equal(constructors);

            // Compare with the .NET behavior
            constructors = typeof(ConstructorParameterless).GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            constructors.Should().HaveCount(1);
            constructors[0].IsPublic.Should().BeTrue();
        }

        [Fact]
        public void GetConstructors_ParameterlessStatic()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorParameterless));

            var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Static);

            constructors.Should().HaveCount(1);
            constructors[0].IsPrivate.Should().BeTrue();

            type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Static).Should().Equal(constructors);

            // Compare with the .NET behavior
            constructors = typeof(ConstructorParameterless).GetConstructors(BindingFlags.NonPublic | BindingFlags.Static);

            constructors.Should().HaveCount(1);
            constructors[0].IsPrivate.Should().BeTrue();
        }

        [Fact]
        public void ActivatorUtilitiesConstructorPropertiesRedirectedToWrappedConstructor()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
            var dotNetConstructor = typeof(ConstructorTest).GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];

            constructor.MethodHandle.Should().Be(dotNetConstructor.MethodHandle);
            constructor.Attributes.Should().Be(dotNetConstructor.Attributes);
            constructor.Name.Should().Be(".ctor");
            constructor.DeclaringType.Should().BeSameAs(typeof(ConstructorTest));
            constructor.ReflectedType.Should().BeSameAs(typeof(ConstructorTest));
            constructor.GetMethodImplementationFlags().Should().Be(dotNetConstructor.GetMethodImplementationFlags());
            constructor.GetParameters().Should().BeEquivalentTo(dotNetConstructor.GetParameters());
        }

        [Fact]
        public void GetCustomAttributes_WithInherit()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];

            var attributes = constructor.GetCustomAttributes(true);

            attributes.Should().HaveCount(2);
            attributes[0].Should().BeOfType(typeof(EditorAttribute));
            attributes[1].Should().BeOfType(typeof(ActivatorUtilitiesConstructorAttribute));

            var otherAttributes = constructor.GetCustomAttributes(true);
            otherAttributes.Should().HaveCount(2);
            otherAttributes[0].Should().NotBeSameAs(attributes[0]);
            otherAttributes[1].Should().NotBeSameAs(attributes[1]);
        }

        [Fact]
        public void GetCustomAttributes_WithoutInherit()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];

            var attributes = constructor.GetCustomAttributes(false);

            attributes.Should().HaveCount(2);
            attributes[0].Should().BeOfType(typeof(EditorAttribute));
            attributes[1].Should().BeOfType(typeof(ActivatorUtilitiesConstructorAttribute));

            var otherAttributes = constructor.GetCustomAttributes(false);
            otherAttributes.Should().HaveCount(2);
            otherAttributes[0].Should().NotBeSameAs(attributes[0]);
            otherAttributes[1].Should().NotBeSameAs(attributes[1]);
        }

        [Fact]
        public void GetCustomAttributes_WithType()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];

            var attributes = constructor.GetCustomAttributes(typeof(EditorAttribute), true);

            attributes.Should().HaveCount(1);
            attributes[0].Should().BeOfType(typeof(EditorAttribute));

            var otherAttributes = constructor.GetCustomAttributes(typeof(EditorAttribute), true);
            otherAttributes.Should().HaveCount(1);
            otherAttributes[0].Should().NotBeSameAs(attributes[0]);
        }

        [Fact]
        public void GetCustomAttributes_WithTypeActivatorUtilitiesConstructorAttribute()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];

            var attributes = constructor.GetCustomAttributes(typeof(ActivatorUtilitiesConstructorAttribute), true);

            attributes.Should().HaveCount(1);
            attributes[0].Should().BeOfType(typeof(ActivatorUtilitiesConstructorAttribute));

            var otherAttributes = constructor.GetCustomAttributes(typeof(ActivatorUtilitiesConstructorAttribute), true);
            otherAttributes.Should().HaveCount(1);
            otherAttributes[0].Should().NotBeSameAs(attributes[0]);
        }

        [Fact]
        public void IsDefined()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];

            constructor.IsDefined(typeof(ActivatorUtilitiesConstructorAttribute), true).Should().BeTrue();
            constructor.IsDefined(typeof(EditorAttribute), true).Should().BeTrue();
            constructor.IsDefined(typeof(BrowsableAttribute), true).Should().BeFalse();

            constructor.IsDefined(typeof(ActivatorUtilitiesConstructorAttribute), false).Should().BeTrue();
            constructor.IsDefined(typeof(EditorAttribute), false).Should().BeTrue();
            constructor.IsDefined(typeof(BrowsableAttribute), false).Should().BeFalse();
        }

        [Fact]
        public void Invoke_WithNoObject()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];

            var result = constructor.Invoke(BindingFlags.Public, null, new object[] { 1234 }, CultureInfo.InvariantCulture);

            result.Should().BeOfType<ConstructorTest>();
        }

        [Fact]
        public void Invoke_WithObject()
        {
            var type = new ActivatorUtilitiesConstructorTypeAdapter(typeof(ConstructorTest));

            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];

            var result = constructor.Invoke(new ConstructorTest(1), BindingFlags.Public, null, new object[] { 1234 }, CultureInfo.InvariantCulture);

            result.Should().BeNull();
        }

        private class ConstructorTest : ConstructorTestBase
        {
            [Editor("A", "B")]
            public ConstructorTest(int isPublic1)
            {
            }

            public ConstructorTest(short isPublic2)
            {
            }

            private ConstructorTest(bool isPrivate1)
            {
            }

            private ConstructorTest(string isPrivate2)
            {
            }

            static ConstructorTest()
            {
            }
        }

        private class ConstructorTestBase
        {
            public ConstructorTestBase()
            {
            }

            [EditorBrowsable]
            [ActivatorUtilitiesConstructor]
            public ConstructorTestBase(int baseConstructorPublic)
            {
            }

            private ConstructorTestBase(string baseConstructorPrivate)
            {
            }
        }

        private class ConstructorForNoMatchingVisibilityTest
        {
            public ConstructorForNoMatchingVisibilityTest()
            {
            }
        }

        private class ConstructorParameterless : ConstructorParameterlessBase
        {
            static ConstructorParameterless()
            {
            }

            public ConstructorParameterless()
            {
            }
        }

        private class ConstructorParameterlessBase
        {
            static ConstructorParameterlessBase()
            {
            }

            [ActivatorUtilitiesConstructor]
            public ConstructorParameterlessBase()
            {
            }
        }
    }
}