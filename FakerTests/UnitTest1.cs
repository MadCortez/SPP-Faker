using System.Linq.Expressions;
using System.Reflection;
using FakerLibrary.Generators;
using FakerLibrary;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FakerTests
{
    [TestClass]
    public class UnitTest1
    {
        Faker faker = new Faker();

        [TestMethod]
        public void ValueTypes()
        {
            GeneratorContext generatorContext = new GeneratorContext();
            BoolGenerator boolGenerator = new BoolGenerator();
            IntegerGenerator integerGenerator = new IntegerGenerator();
            StringGenerator stringGenerator = new StringGenerator();
            bool condition = boolGenerator.CanGenerate(typeof(bool))
                && integerGenerator.CanGenerate(typeof(int))
                && stringGenerator.CanGenerate(typeof(string))
                && boolGenerator.Generate(typeof(bool), generatorContext) is bool
                && integerGenerator.Generate(typeof(int), generatorContext) is int
                && stringGenerator.Generate(typeof(string), generatorContext) is string;
            Assert.IsTrue(condition);
        }

        public class TestClass
        {
            public int a;
            public int b;
            public int c;

            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public TestClass(int a, int x)
            {
                this.a = a;
                X = x;
            }
        }

        [TestMethod]
        public void CheckFieldsAndPropsFilling()
        {
            TestClass testObj = faker.Create<TestClass>();
            Assert.IsNotNull(testObj);
            Assert.IsNotNull(testObj.a);
            Assert.IsNotNull(testObj.b);
            Assert.IsNotNull(testObj.c);
            Assert.IsNotNull(testObj.X);
            Assert.IsNotNull(testObj.Y);
            Assert.IsNotNull(testObj.Z);
        }

        public class CyclingTestClass : TestClass
        {
            public List<CyclingTestClass> testList;
            public CyclingTestClass innerObj;
            public CyclingTestClass(int a, int x) : base(a, x) { }
        }

        [TestMethod]
        public void TestCyclingClass()
        {
            CyclingTestClass cyclingTestClass = faker.Create<CyclingTestClass>();
            Assert.IsNotNull(cyclingTestClass);
            Assert.IsNotNull(cyclingTestClass.testList);
            Assert.IsNull(cyclingTestClass.testList[0]);
            Assert.IsNull(cyclingTestClass.innerObj);
        }

        public class A
        {
            public int a; 
        }
        public class B
        {
            public A objA;
            public int b;
        }
        public class C
        {
            public B objB;
            public int c;
        }

        [TestMethod]
        public void TestNestedClasses()
        {
            C objC = faker.Create<C>();
            Assert.IsNotNull(objC);
            Assert.IsNotNull(objC.c);
            Assert.IsNotNull(objC.objB);
            Assert.IsNotNull(objC.objB.b);
            Assert.IsNotNull(objC.objB.objA);
            Assert.IsNotNull(objC.objB.objA.a);
        }

        public class CoDependentClass1
        {
            public CoDependentClass2 cl2;

            public CoDependentClass1(CoDependentClass2 cl2)
            {
                this.cl2 = cl2;
            }
            public CoDependentClass1() { }
        }

        public class CoDependentClass2
        {
            public CoDependentClass1 cl1;
            public CoDependentClass2(CoDependentClass1 cl1)
            {
                this.cl1 = cl1;
            }
            public CoDependentClass2() { }
        }

        [TestMethod]
        public void CoDependentClassesTest()
        {
            var cl1 = faker.Create<CoDependentClass1>();
            var cl2 = faker.Create<CoDependentClass2>();

            Assert.IsNotNull(cl1);
            Assert.IsNotNull(cl2);

            Assert.IsNotNull(cl1.cl2);
            Assert.IsNotNull(cl2.cl1);

            Assert.IsNull(cl1.cl2.cl1);
            Assert.IsNull(cl2.cl1.cl2);
        }
    }
}