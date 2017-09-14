using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrodLib.Utils;
using FrodLib.IoC;
using System.Threading;
using IoCTestInstances1;
using System.Reflection;
using IoCTestInstances2;
using System.Linq;
using System.Collections.Generic;

namespace FrodLib.Tests
{
    [TestClass]
    public class IoCContainerTests
    {
        [TestMethod]
        [TestCategory("IoCContainer")]
        public void RegisterInterfaceAndGetClassTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register(typeof(I1), typeof(C1));
            });

            object i1Instance = container.GetInstance(typeof(I1));
            Assert.IsNotNull(i1Instance);
            Assert.IsInstanceOfType(i1Instance, typeof(I1));
            Assert.IsInstanceOfType(i1Instance, typeof(C1));

            container.Configure(c =>
            {
                c.Register<I2, C2>();
            });

            object i2Instance = container.GetInstance<I2>();
            Assert.IsNotNull(i2Instance);
            Assert.IsInstanceOfType(i2Instance, typeof(I2));
            Assert.IsInstanceOfType(i2Instance, typeof(C2));

            object i2Instance2 = container.GetInstance<I2>();
            Assert.IsNotNull(i2Instance2);
            Assert.IsInstanceOfType(i2Instance2, typeof(I2));
            Assert.IsInstanceOfType(i2Instance2, typeof(C2));
            Assert.AreNotSame(i2Instance, i2Instance2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void RegisterFactoryAndGetClassTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1>(() => new C1());

            });

            object i1Instance = container.GetInstance(typeof(I1));
            Assert.IsNotNull(i1Instance);
            Assert.IsInstanceOfType(i1Instance, typeof(I1));
            Assert.IsInstanceOfType(i1Instance, typeof(C1));

        }


        [TestMethod]
        [TestCategory("IoCContainer")]
        public void RegisterAndGetSingletonInstanceTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>().AsSingleInstance();
            });

            object i1Instance1 = container.GetInstance<I1>();
            object i1Instance2 = container.GetInstance<I1>();

            Assert.AreSame(i1Instance1, i1Instance2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void RegisterInstanceAndGetInstanceTest()
        {
            IoCContainer container = new IoCContainer();

            C1 c1 = new C1();

            container.Configure(c =>
            {
                c.RegisterInstance<I1>(c1);
            });

            object i1Instance = container.GetInstance(typeof(I1));
            Assert.IsNotNull(i1Instance);
            Assert.IsInstanceOfType(i1Instance, typeof(I1));
            Assert.IsInstanceOfType(i1Instance, typeof(C1));
            Assert.AreSame(c1, i1Instance);

            object i1Instance2 = container.GetInstance(typeof(I1));
            Assert.IsNotNull(i1Instance2);
            Assert.IsInstanceOfType(i1Instance2, typeof(I1));
            Assert.IsInstanceOfType(i1Instance2, typeof(C1));
            Assert.AreSame(c1, i1Instance2);
            Assert.AreSame(i1Instance, i1Instance2);

        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void RegisterClassAndGetInstanceTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<C1, C1>();
            });
            var c1Instance = container.GetInstance(typeof(C1));
            Assert.IsNotNull(c1Instance);
            Assert.IsInstanceOfType(c1Instance, typeof(C1));
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetInstanceWithLookupTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
                c.Register<I2, C2>();
            });

            var c5 = container.GetInstance<C5>();
            Assert.IsNotNull(c5);
            Assert.IsNotNull(c5.I2, "Constructor lookup of implementation of C2 failed");
            Assert.IsNotNull(c5.I2.I1, "Recursive Constructor lookup of implementation of C1 failed");
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetNoneMappedInstanceShouldThrow()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I2, C2>();
            });
            try
            {
                container.GetInstance<I1>();
                Assert.Fail("An exception should have been thrown");
            }
            catch (IoCException)
            {

            }
            catch (Exception)
            {
                Assert.Fail("An invalid operation exception should have been thrown");
            }


            try
            {
                container.GetInstance<C5>();
                Assert.Fail("An exception during recursive lookup should have been thrown");
            }
            catch (IoCException)
            {

            }
            catch (Exception)
            {
                Assert.Fail("An invalid operation exception should have been thrown");
            }
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetInstanceWithTryGetInstance()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
            });

            object objInstance;
            I1 i1Instance;
            if (container.TryGetInstance(typeof(I1), out objInstance) == false) Assert.Fail();
            if (container.TryGetInstance<I1>(out i1Instance) == false) Assert.Fail();

            I2 i2Instance;
            if (container.TryGetInstance(typeof(I2), out objInstance)) Assert.Fail();
            if (container.TryGetInstance<I2>(out i2Instance)) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetInstancePrimaryConstructorTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
                c.Register<I2, C2>();
            });

            var c3 = container.GetInstance<C3>();
            Assert.IsNull(c3.I1);
            Assert.IsNotNull(c3.I2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetInstancePrimaryConstructorRequiredTest()
        {
            IoCContainer container = new IoCContainer();

            container.Configure(c =>
            {
                c.DefaultConstructorSelectionRule = ConstructorSelectorRule.RequirePrimary;
                c.Register<I1, C1>();
                c.Register<I2, C2>();
            });

            var c3 = container.GetInstance<C3>();
            Assert.IsNull(c3.I1);
            Assert.IsNotNull(c3.I2);

            try
            {
                var c4 = container.GetInstance<C4>();
                Assert.Fail("Constructor found where should not have been found");
            }
            catch (IoCException)
            {
                // Throw here is the expected result
            }

        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetInstanceDefaultConstructorMostArgsTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.DefaultConstructorSelectionRule = ConstructorSelectorRule.MostArgs;
                c.Register<I1, C1>();
                c.Register<I2, C2>();
            });

            var c4 = container.GetInstance<C4>();
            Assert.IsNotNull(c4.I1);
            Assert.IsNotNull(c4.I2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetInstanceDefaultConstructorLeastArgsTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.DefaultConstructorSelectionRule = ConstructorSelectorRule.LeastArgs;
                c.Register<I1, C1>();
                c.Register<I2, C2>();
            });

            var c4 = container.GetInstance<C4>();
            Assert.IsNull(c4.I1);
            Assert.IsNull(c4.I2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void AddGetFromCustomRegistry()
        {
            TestRegistry reg = new TestRegistry();
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.AddRegistry(reg);
            });

            var c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.AreEqual(1, reg.CreateInstanceCallCount);
            Assert.AreEqual(1, reg.HasImplementationForTypeCallCount);

            c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.AreEqual(2, reg.CreateInstanceCallCount);
            Assert.AreEqual(2, reg.HasImplementationForTypeCallCount);

        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanAssembly()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanAssembly(typeof(IoCContainerTests).Assembly);
                });
            });

            var c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);

            var c2 = container.GetInstance<I2>();
            Assert.IsNotNull(c2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanCallingAssembly()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanCallingAssembly();
                });
            });

            var c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);

            var c2 = container.GetInstance<I2>();
            Assert.IsNotNull(c2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanAssemblyExcludeFilter()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanAssembly(typeof(IoCContainerTests).Assembly);
                    s.Exclude(f => !(f == typeof(C1)));
                });
            });

            var c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(C1));

            try
            {
                var c2 = container.GetInstance<I2>();
                Assert.Fail("Instance should not have been mapped");
            }
            catch (Exception)
            {

            }
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanAssemblyIncludeNameSpace()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanAssembly(typeof(IoCContainerTests).Assembly);
                    s.IncludeNamespace("IoCTestInstances1");
                });
            });

            var c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(C1));

            try
            {
                var c2 = container.GetInstance<I2>();
                Assert.Fail("Instance should not have been mapped");
            }
            catch (Exception)
            {

            }
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanAssemblyExcludeNameSpace()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanAssembly(typeof(IoCContainerTests).Assembly);
                    s.ExcludeNamespace("IoCTestInstances1");
                    s.ExcludeNamespace("FrodLib.Tests");
                });
            });

            var c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(C6));

            try
            {
                var c2 = container.GetInstance<I3>();
                Assert.Fail("Instance should not have been mapped");
            }
            catch (Exception)
            {

            }
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanAssemblyForRegistry()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanAssembly(typeof(IoCContainerTests).Assembly);
                    s.ScanForRegistires();
                });
            });

            var c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.AreEqual(1, TestRegistry.SCreateInstanceCallCount);
            Assert.AreEqual(1, TestRegistry.SHasImplementationForTypeCallCount);

            c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.AreEqual(2, TestRegistry.SCreateInstanceCallCount);
            Assert.AreEqual(2, TestRegistry.SHasImplementationForTypeCallCount);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanAssemblySingleImplementation()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanAssembly(typeof(IoCContainerTests).Assembly);
                    s.SingleImplementationOfInterface();
                });
            });

            try
            {
                var c1 = container.GetInstance<I1>();
                Assert.Fail("Instance should not have been mapped");
            }
            catch (Exception)
            {

            }

            var c2 = container.GetInstance<I3>();
            Assert.IsNotNull(c2);
            Assert.IsInstanceOfType(c2, typeof(C7));
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanAssemblyMapAsSingleInstance()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanAssembly(typeof(IoCContainerTests).Assembly);
                    s.RegisterAsSingleInstance();
                });
            });

            var c1 = container.GetInstance<I1>();
            var c2 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.IsNotNull(c2);
            Assert.AreSame(c1, c2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ScanAssemblyContainingType()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Scan(s =>
                {
                    s.ScanAssemblyContainingType<C7>();

                });
            });

            var c1 = container.GetInstance<I3>();
            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(C7));
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void RemoveCurrentSingleInstance()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>().AsSingleInstance();
            });

            var c1 = container.GetInstance<I1>();
            var c2 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);
            Assert.IsNotNull(c2);
            Assert.AreSame(c1, c2);

            bool removed = container.RemoveCurrentSingleInstance<I1>();
            Assert.IsTrue(removed);
            var c3 = container.GetInstance<I1>();
            Assert.IsNotNull(c3);
            Assert.AreNotSame(c1, c3);

        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void RemoveRegistredSingleInstance()
        {
            C1 c1 = new C1();
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.RegisterInstance<I1>(c1);
            });

            var c2 = container.GetInstance<I1>();
            Assert.IsNotNull(c2);
            Assert.AreSame(c1, c2);

            bool removed = container.RemoveCurrentSingleInstance<I1>();
            Assert.IsTrue(removed);
            I1 c3;
            Assert.IsFalse(container.TryGetInstance<I1>(out c3));

            removed = container.RemoveCurrentSingleInstance<I1>();
            Assert.IsFalse(removed);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void RegisterInstance()
        {
            C1 c1 = new C1();
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.RegisterInstance<I1>(c1);
            });

            var c2 = container.GetInstance<I1>();
            Assert.IsNotNull(c2);
            Assert.AreSame(c1, c2);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetStoredInstanceByKey()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
                c.Register<I3, C7>();
            });

            var c1 = container.GetInstance<I1>();
            var c2 = container.GetInstance<I1>();

            Assert.AreNotSame(c1, c2);

            var c3 = container.GetInstance<I1>("Test");
            var c4 = container.GetInstance<I1>("Test");
            Assert.AreSame(c3, c4);
            Assert.AreNotSame(c1, c3);
            Assert.AreNotSame(c1, c4);

            var c32 = container.GetInstance<I1>("Test1");
            var c42 = container.GetInstance<I1>("Test1");
            Assert.AreSame(c32, c42);
            Assert.AreNotSame(c1, c32);
            Assert.AreNotSame(c1, c42);
            Assert.AreNotSame(c3, c32);
            Assert.AreNotSame(c4, c42);

            var c5 = container.GetInstance<I3>();
            var c6 = container.GetInstance<I3>();

            Assert.AreNotSame(c5, c6);

            var c7 = container.GetInstance<I3>("Test");
            var c8 = container.GetInstance<I3>("Test");
            Assert.AreSame(c7, c8);
            Assert.AreNotSame(c5, c7);
            Assert.AreNotSame(c5, c8);

            Assert.AreNotSame(c3, c7);
            Assert.AreNotSame(c4, c8);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void TryGetStoredInstanceByKey()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
                c.Register<I3, C7>();
            });

            I1 c1;
            I1 c2;
            Assert.IsTrue(container.TryGetInstance<I1>(out c1));
            Assert.IsTrue(container.TryGetInstance<I1>(out c2));

            Assert.AreNotSame(c1, c2);

            I1 c3;
            I1 c4;
            Assert.IsTrue(container.TryGetInstance<I1>("Test", out c3));
            Assert.IsTrue(container.TryGetInstance<I1>("Test", out c4));
            Assert.AreSame(c3, c4);
            Assert.AreNotSame(c1, c3);
            Assert.AreNotSame(c1, c4);

            I1 c32;
            I1 c42;

            Assert.IsTrue(container.TryGetInstance<I1>("Test1", out c32));
            Assert.IsTrue(container.TryGetInstance<I1>("Test1", out c42));
            Assert.AreSame(c32, c42);
            Assert.AreNotSame(c1, c32);
            Assert.AreNotSame(c1, c42);
            Assert.AreNotSame(c3, c32);
            Assert.AreNotSame(c4, c42);

            I3 c5;
            I3 c6;

            Assert.IsTrue(container.TryGetInstance<I3>(out c5));
            Assert.IsTrue(container.TryGetInstance<I3>(out c6));

            Assert.AreNotSame(c5, c6);

            I3 c7;
            I3 c8;

            Assert.IsTrue(container.TryGetInstance<I3>("Test", out c7));
            Assert.IsTrue(container.TryGetInstance<I3>("Test", out c8));
            Assert.AreSame(c7, c8);
            Assert.AreNotSame(c5, c7);
            Assert.AreNotSame(c5, c8);
            Assert.AreNotSame(c3, c7);
            Assert.AreNotSame(c4, c8);

            I2 c9;
            Assert.IsFalse(container.TryGetInstance<I2>("Test", out c9));
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void UnregisterType()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>().AsSingleInstance();
            });

            var c1 = container.GetInstance<I1>();
            Assert.IsNotNull(c1);

            container.Configure(c =>
            {
                c.Unregister<I1>();
            });

            try
            {
                var c2 = container.GetInstance<I1>();
                Assert.Fail("Instance should no longer be registered here");
            }
            catch (IoCException)
            {
            }

        }

        [ThreadStatic]
        static int resolverCalls;

        [TestInitialize()]
        public void IoCTestInit()
        {
            resolverCalls = 0;
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void LookupWithAssemblyResolver()
        {
            IoCContainer container = new IoCContainer();
            var c8 = container.GetInstance<C8>(ArgResolver);

            Assert.AreEqual(3, resolverCalls);
            Assert.AreEqual(10, c8.Var1);
            Assert.AreEqual("TESTSTRING", c8.Var2);
            Assert.IsNull(c8.Var3);
        }

        private bool ArgResolver(int index, string name, Type type, out object value)
        {
            resolverCalls++;
            if (index == 0)
            {
                Assert.AreEqual(name, "v1");
                Assert.AreEqual(type, typeof(int));
                value = (int)10;
                return true;
            }
            else if (index == 1)
            {
                Assert.AreEqual(name, "v2");
                Assert.AreEqual(type, typeof(string));
                value = "TESTSTRING";
                return true;
            }
            else if (index == 2)
            {
                Assert.AreEqual(name, "v3");
                Assert.AreEqual(type, typeof(string));
                value = "SHOULD NOT BE SET";
                return false;
            }
            else
            {
                value = null;
                Assert.Fail("Should only be three calls");
                return false;
            }
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCDispose()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
                c.Register<I3, C7>();
            });

            var c1 = container.GetInstance<I1>("Test");
            var c2 = container.GetInstance<I1>("Test1");

            var i1Instances = container.GetAllInstances<I1>();
            Assert.AreEqual(2, i1Instances.Count);

            container.Dispose();

            i1Instances = container.GetAllInstances<I1>();
            Assert.AreEqual(0, i1Instances.Count);

            try
            {
                var c3 = container.GetInstance<I1>();
                Assert.Fail("No mapped instaces should be left");
            }
            catch (IoCException)
            {

            }

        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ClearAllStoredInstances()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
                c.Register<I3, C7>();
            });
            var c1 = container.GetInstance<I1>("Test");
            var c2 = container.GetInstance<I1>("Test1");

            var c3 = container.GetInstance<I3>("Test");
            var c4 = container.GetInstance<I3>("Test1");

            var i1Instances = container.GetAllInstances<I1>();
            var i3Instances = container.GetAllInstances<I3>();

            Assert.AreEqual(2, i1Instances.Count);
            Assert.AreEqual(2, i3Instances.Count);

            container.ClearAllMappedInstances();

            i1Instances = container.GetAllInstances<I1>();
            i3Instances = container.GetAllInstances<I3>();

            Assert.AreEqual(0, i1Instances.Count);
            Assert.AreEqual(0, i3Instances.Count);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void ClearAllStoredInstancesForType()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
                c.Register<I3, C7>();
            });
            var c1 = container.GetInstance<I1>("Test");
            var c2 = container.GetInstance<I1>("Test1");

            var c3 = container.GetInstance<I3>("Test");
            var c4 = container.GetInstance<I3>("Test1");

            var i1Instances = container.GetAllInstances<I1>();
            var i3Instances = container.GetAllInstances<I3>();

            Assert.AreEqual(2, i1Instances.Count);
            Assert.AreEqual(2, i3Instances.Count);

            container.ClearAllMappedInstancesForContract<I3>();

            i1Instances = container.GetAllInstances<I1>();
            i3Instances = container.GetAllInstances<I3>();

            Assert.AreEqual(2, i1Instances.Count);
            Assert.AreEqual(0, i3Instances.Count);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void GetAllInstancesOfContract()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
            });
            var c3 = container.GetInstance<I1>("Test");
            var c4 = container.GetInstance<I1>("Test1");

            Assert.AreNotSame(c3, c4);

            var i1Instances = container.GetAllInstances<I1>();
            Assert.AreEqual(2, i1Instances.Count);
            Assert.IsTrue(i1Instances.Contains(c3));
            Assert.IsTrue(i1Instances.Contains(c4));

            try
            {
                container.GetInstance<I2>("Test");
                Assert.Fail("Exception should have been thrown");
            }
            catch { }
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCFillTest()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
                c.Register<I2, C2>();
            });

            C9 c9 = new C9();
            container.Fill(c9);
            c9.ValidatePropertyFieldExistance();
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCRegisterImplementationForExistingContract()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C1>();
            });

            var c1 = container.GetInstance<I1>();

            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(C1));

            container.Configure(c =>
            {
                c.Register<I1, C6>();
            });

            var c6 = container.GetInstance<I1>();

            Assert.IsNotNull(c6);
            Assert.IsInstanceOfType(c6, typeof(C6));
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCResolvePrivateConstructors()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C10>();
                c.Register<I2, C11>();
            });

            var c1 = container.GetInstance<I1>();

            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(C10));

            var c2 = container.GetInstance<I2>();

            Assert.IsNotNull(c2);
            Assert.IsInstanceOfType(c2, typeof(C11));
            Assert.IsNotNull(c2.I1);
            Assert.IsInstanceOfType(c2.I1, typeof(C10));
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCInjectByMethod()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I1, C10>();
                c.Register<I2, C11>();
            });

            var c1 = container.GetInstance<C12>();

            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(C12));
            Assert.AreEqual(1, c1.I1CallCount);

            Assert.IsNotNull(c1.I1);
            Assert.IsInstanceOfType(c1.I1, typeof(C10));

            Assert.IsNull(c1.I2);
            Assert.AreEqual(0, c1.I2CallCount);
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCCircularReferencesConstructorPlusMethodSingleInstances()
        {
            IoCContainer container = new IoCContainer();
           
            container.Configure(c =>
            {
                c.Register<I4, CircularMethodi4>().AsSingleInstance();
                c.Register<I5, CircularConstructori5>().AsSingleInstance();
            });

            var c1 = container.GetInstance<I4>();

            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c1).CallCount);

            Assert.IsNotNull(c1.I5);
            Assert.IsInstanceOfType(c1.I5, typeof(CircularConstructori5));

            Assert.IsNotNull(c1.I5.I4);
            Assert.IsInstanceOfType(c1.I5.I4, typeof(CircularMethodi4));
            Assert.AreSame(c1.I5.I4, c1);

            Console.WriteLine("Get I4 Pass");

            var c3 = container.GetInstance<I4>();

            Assert.IsNotNull(c3);
            Assert.IsInstanceOfType(c3, typeof(CircularMethodi4));
           

            Assert.IsNotNull(c3.I5);
            Assert.IsInstanceOfType(c3.I5, typeof(CircularConstructori5));

            Assert.IsNotNull(c3.I5.I4);
            Assert.IsInstanceOfType(c3.I5.I4, typeof(CircularMethodi4));
            Assert.AreSame(c3.I5.I4, c3);

            Assert.AreSame(c1, c3);
            Assert.AreSame(c1.I5, c3.I5);
            Assert.AreEqual(1, ((CircularMethodi4)c3).CallCount);

            Console.WriteLine("Get I4 2 Pass");

            container.ClearAllMappedInstances();
            container.RemoveCurrentSingleInstance<I4>();
            container.RemoveCurrentSingleInstance<I5>();

            var c2 = container.GetInstance<I5>();

            Assert.IsNotNull(c2);
            Assert.IsInstanceOfType(c2, typeof(CircularConstructori5));

            Assert.IsNotNull(c2.I4);
            Assert.IsInstanceOfType(c2.I4, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c2.I4).CallCount);

            Assert.IsNotNull(c2.I4.I5);
            Assert.IsInstanceOfType(c2.I4.I5, typeof(CircularConstructori5));
            Assert.AreSame(c2.I4.I5, c2);

            Console.WriteLine("Get I5 Pass");

            var c4 = container.GetInstance<I5>();

            Assert.IsNotNull(c4);
            Assert.IsInstanceOfType(c4, typeof(CircularConstructori5));

            Assert.IsNotNull(c4.I4);
            Assert.IsInstanceOfType(c4.I4, typeof(CircularMethodi4));
           

            Assert.IsNotNull(c4.I4.I5);
            Assert.IsInstanceOfType(c2.I4.I5, typeof(CircularConstructori5));
            Assert.AreSame(c4.I4.I5, c4);

            Assert.AreSame(c2, c4);
            Assert.AreSame(c2.I4, c4.I4);
            Assert.AreEqual(1, ((CircularMethodi4)c4.I4).CallCount);

            Console.WriteLine("Get I5 2 Pass");
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCCircularReferencesMethodPlusMethodSingleInstances()
        {
            IoCContainer container = new IoCContainer();

            container.Configure(c =>
            {
                c.Register<I4, CircularMethodi4>().AsSingleInstance();
                c.Register<I5, CircularMethodi5>().AsSingleInstance();
            });

            var c1 = container.GetInstance<I4>();

            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c1).CallCount);

            Assert.IsNotNull(c1.I5);
            Assert.IsInstanceOfType(c1.I5, typeof(CircularMethodi5));
            Assert.AreEqual(1, ((CircularMethodi5)c1.I5).CallCount);

            Assert.IsNotNull(c1.I5.I4);
            Assert.IsInstanceOfType(c1.I5.I4, typeof(CircularMethodi4));
            Assert.AreSame(c1.I5.I4, c1);

            Console.WriteLine("Get I4 Pass");

            var c3 = container.GetInstance<I4>();

            Assert.IsNotNull(c3);
            Assert.IsInstanceOfType(c3, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c3).CallCount);

            Assert.IsNotNull(c3.I5);
            Assert.IsInstanceOfType(c3.I5, typeof(CircularMethodi5));
            Assert.AreEqual(1, ((CircularMethodi5)c3.I5).CallCount);

            Assert.IsNotNull(c3.I5.I4);
            Assert.IsInstanceOfType(c3.I5.I4, typeof(CircularMethodi4));
            Assert.AreSame(c3.I5.I4, c3);

            Assert.AreSame(c1, c3);
            Assert.AreSame(c1.I5, c3.I5);

            Console.WriteLine("Get I4 2 Pass");

            container.ClearAllMappedInstances();
            container.RemoveCurrentSingleInstance<I4>();
            container.RemoveCurrentSingleInstance<I5>();

            var c2 = container.GetInstance<I5>();

            Assert.IsNotNull(c2);
            Assert.IsInstanceOfType(c2, typeof(CircularMethodi5));
            Assert.AreEqual(1, ((CircularMethodi5)c2).CallCount);

            Assert.IsNotNull(c2.I4);
            Assert.IsInstanceOfType(c2.I4, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c2.I4).CallCount);

            Assert.IsNotNull(c2.I4.I5);
            Assert.IsInstanceOfType(c2.I4.I5, typeof(CircularMethodi5));
            Assert.AreSame(c2.I4.I5, c2);

            Console.WriteLine("Get I5 Pass");

            var c4 = container.GetInstance<I5>();

            Assert.IsNotNull(c4);
            Assert.IsInstanceOfType(c4, typeof(CircularMethodi5));
            Assert.AreEqual(1, ((CircularMethodi5)c4).CallCount);

            Assert.IsNotNull(c2.I4);
            Assert.IsInstanceOfType(c2.I4, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c4.I4).CallCount);

            Assert.IsNotNull(c4.I4.I5);
            Assert.IsInstanceOfType(c2.I4.I5, typeof(CircularMethodi5));
            Assert.AreSame(c4.I4.I5, c4);

            Assert.AreSame(c2, c4);
            Assert.AreSame(c2.I4, c4.I4);

            Console.WriteLine("Get I5 2 Pass");
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCCircularReferencesConstructorPlusMethod()
        {
            IoCContainer container = new IoCContainer();

            container.Configure(c =>
            {
                c.Register<I4, CircularMethodi4>();
                c.Register<I5, CircularConstructori5>();
            });

            var c1 = container.GetInstance<I4>();

            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c1).CallCount);

            Assert.IsNotNull(c1.I5);
            Assert.IsInstanceOfType(c1.I5, typeof(CircularConstructori5));

            Assert.IsNotNull(c1.I5.I4);
            Assert.IsInstanceOfType(c1.I5.I4, typeof(CircularMethodi4));
            Assert.AreSame(c1.I5.I4, c1);

            Console.WriteLine("Get I4 Pass");

            var c3 = container.GetInstance<I4>();

            Assert.IsNotNull(c3);
            Assert.IsInstanceOfType(c3, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c3).CallCount);

            Assert.IsNotNull(c3.I5);
            Assert.IsInstanceOfType(c3.I5, typeof(CircularConstructori5));

            Assert.IsNotNull(c3.I5.I4);
            Assert.IsInstanceOfType(c3.I5.I4, typeof(CircularMethodi4));
            Assert.AreSame(c3.I5.I4, c3);

            Assert.AreNotSame(c1, c3);
            Assert.AreNotSame(c1.I5, c3.I5);

            Console.WriteLine("Get I4 2 Pass");


            container.ClearAllMappedInstances();
            container.RemoveCurrentSingleInstance<I4>();
            container.RemoveCurrentSingleInstance<I5>();

            var c2 = container.GetInstance<I5>();

            Assert.IsNotNull(c2);
            Assert.IsInstanceOfType(c2, typeof(CircularConstructori5));

            Assert.IsNotNull(c2.I4);
            Assert.IsInstanceOfType(c2.I4, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c2.I4).CallCount);

            Assert.IsNotNull(c2.I4.I5);
            Assert.IsInstanceOfType(c2.I4.I5, typeof(CircularConstructori5));
            Assert.AreSame(c2.I4.I5, c2);

            Console.WriteLine("Get I5 Pass");

            var c4 = container.GetInstance<I5>();

            Assert.IsNotNull(c4);
            Assert.IsInstanceOfType(c4, typeof(CircularConstructori5));

            Assert.IsNotNull(c4.I4);
            Assert.IsInstanceOfType(c4.I4, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c4.I4).CallCount);

            Assert.IsNotNull(c4.I4.I5);
            Assert.IsInstanceOfType(c4.I4.I5, typeof(CircularConstructori5));
            Assert.AreSame(c4.I4.I5, c4);

            Assert.AreNotSame(c2, c4);
            Assert.AreNotSame(c2.I4, c4.I4);

            Console.WriteLine("Get I5 Pass");

        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCCircularReferencesMethodPlusMethod()
        {
            IoCContainer container = new IoCContainer();

            container.Configure(c =>
            {
                c.Register<I4, CircularMethodi4>();
                c.Register<I5, CircularMethodi5>();
            });

            var c2 = container.GetInstance<I5>();

            Assert.IsNotNull(c2);
            Assert.IsInstanceOfType(c2, typeof(CircularMethodi5));
            Assert.AreEqual(1, ((CircularMethodi5)c2).CallCount);

            Assert.IsNotNull(c2.I4);
            Assert.IsInstanceOfType(c2.I4, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c2.I4).CallCount);

            Assert.IsNotNull(c2.I4.I5);
            Assert.IsInstanceOfType(c2.I4.I5, typeof(CircularMethodi5));
            Assert.AreSame(c2.I4.I5, c2);

            Console.WriteLine("Get I5 Pass");

            var c4 = container.GetInstance<I5>();

            Assert.IsNotNull(c4);
            Assert.IsInstanceOfType(c4, typeof(CircularMethodi5));
            Assert.AreEqual(1, ((CircularMethodi5)c4).CallCount);

            Assert.IsNotNull(c4.I4);
            Assert.IsInstanceOfType(c4.I4, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c4.I4).CallCount);

            Assert.IsNotNull(c4.I4.I5);
            Assert.IsInstanceOfType(c4.I4.I5, typeof(CircularMethodi5));
            Assert.AreSame(c4.I4.I5, c4);

            Assert.AreNotSame(c2, c4);
            Assert.AreNotSame(c2.I4, c4.I4);

            Console.WriteLine("Get I5 2 Pass");

            container.ClearAllMappedInstances();
            container.RemoveCurrentSingleInstance<I4>();
            container.RemoveCurrentSingleInstance<I5>();

            var c1 = container.GetInstance<I4>();

            Assert.IsNotNull(c1);
            Assert.IsInstanceOfType(c1, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c1).CallCount);

            Assert.IsNotNull(c1.I5);
            Assert.IsInstanceOfType(c1.I5, typeof(CircularMethodi5));
            Assert.AreEqual(1, ((CircularMethodi5)c1.I5).CallCount);

            Assert.IsNotNull(c1.I5.I4);
            Assert.IsInstanceOfType(c1.I5.I4, typeof(CircularMethodi4));
            Assert.AreSame(c1.I5.I4, c1);

            Console.WriteLine("Get I4 Pass");

            var c3 = container.GetInstance<I4>();

            Assert.IsNotNull(c3);
            Assert.IsInstanceOfType(c3, typeof(CircularMethodi4));
            Assert.AreEqual(1, ((CircularMethodi4)c3).CallCount);

            Assert.IsNotNull(c3.I5);
            Assert.IsInstanceOfType(c3.I5, typeof(CircularMethodi5));
            Assert.AreEqual(1, ((CircularMethodi5)c3.I5).CallCount);

            Assert.IsNotNull(c3.I5.I4);
            Assert.IsInstanceOfType(c3.I5.I4, typeof(CircularMethodi4));
            Assert.AreSame(c3.I5.I4, c3);

            Assert.AreNotSame(c1, c3);
            Assert.AreNotSame(c1.I5, c3.I5);

            Console.WriteLine("Get I4 2 Pass");
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCCircularReferencesThrow()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c =>
            {
                c.Register<I4, CircularConstructori4>().AsSingleInstance();
                c.Register<I5, CircularConstructori5>().AsSingleInstance();
            });

            I4 c1 = null;
            try
            {

                c1 = container.GetInstance<I4>();
                Assert.Fail("Circular exception should have been detected");
            }
            catch (IoCCircularReferenceException)
            {

            }
          
        }

        [TestMethod]
        [TestCategory("IoCContainer")]
        public void IoCResolveMany()
        {
            IoCContainer container = new IoCContainer();
            container.Configure(c => {
                c.Register<I1, C1>();
                c.Register<I1, C10>();
                c.Register<I1, C6>();
            });

            CResolveMany rMany = new CResolveMany();
            container.Fill(rMany);
            var objs = rMany.m_allI1.ToArray();
            Assert.IsTrue(objs.Any(o => o is C1));
            Assert.IsTrue(objs.Any(o => o is C10));
            Assert.IsTrue(objs.Any(o => o is C6));

            var objs2 = rMany.m_allI2.ToArray();
            Assert.IsTrue(objs2.Any(o => o.Value is C1));
            Assert.IsTrue(objs2.Any(o => o.Value is C10));
            Assert.IsTrue(objs2.Any(o => o.Value is C6));
        }

        #region Instance Classes

        public interface I1
        {

        }

        public interface I2
        {
            I1 I1 { get; }
        }


        public interface I3
        {

        }

        public interface I4
        {
            I5 I5 { get; }
        }

        public interface I5
        {
            I4 I4 { get; }
        }

        public class C7 : I3
        {

        }

        public class C2 : I2
        {
            public I1 I1 { get; set; }

            public C2(I1 i)
            {
                I1 = i;
            }
        }

        public class C3
        {
            public I1 I1 { get; set; }
            public I2 I2 { get; set; }

            public C3(I1 i)
            {
                I1 = i;
            }

            [PrimaryConstructor]
            public C3(I2 i)
            {
                I2 = i;
            }

            public C3(I1 i, I2 i2)
            {
                I1 = i;
                I2 = i2;
            }
        }

        public class C4
        {
            public I1 I1 { get; set; }
            public I2 I2 { get; set; }

            public C4()
            {
            }

            public C4(I1 i)
            {
                I1 = i;
            }

            public C4(I1 i, I2 i2)
            {
                I1 = i;
                I2 = i2;
            }
        }

        public class C5
        {
            public I2 I2 { get; private set; }

            public C5(I2 i)
            {
                I2 = i;
            }

        }

        public class C8
        {
            public int Var1 { get; private set; }
            public string Var2 { get; private set; }
            public string Var3 { get; private set; }

            public C8(int v1, string v2, string v3)
            {
                this.Var1 = v1;
                this.Var2 = v2;
                this.Var3 = v3;
            }
        }

        public class C9
        {
            [IoCResolve]
            public I1 f11;
            [IoCResolve]
            private I1 f12;

            [ThreadStatic]
            [IoCResolve]
            private static I1 sf1;

            private I1 f13;

            [IoCResolve]
            public I2 f21;

            [IoCResolve]
            public I1 p11 { get; set; }
            [IoCResolve]
            private I1 p12 { get; set; }
            [IoCResolve]
            private static I1 sp1 { get; set; }
            [IoCResolve]
            private I1 p13 { get; }

            private I1 p14 { get; set; }

            [IoCResolve]
            public I2 p21 { get; set; }

            public C9()
            {
                sf1 = null;
            }

            internal void ValidatePropertyFieldExistance()
            {
                Assert.IsNotNull(this.f11);
                Assert.IsNotNull(this.f12);
                Assert.IsNotNull(this.f21);
                Assert.IsNull(sf1);
                Assert.IsNull(f13);

                Assert.IsNotNull(this.p11);
                Assert.IsNotNull(this.p12);
                Assert.IsNull(this.p14);
                Assert.IsNull(sp1);
                Assert.IsNull(p13);
                Assert.IsNotNull(this.p21);
            }
        }

        public class C10 : I1
        {
            private C10()
            {

            }
        }

        public class C11 : I2
        {
            private C11(I1 i1)
            {
                I1 = i1;
            }

            public I1 I1
            {
                get; private set;
            }
        }


        #region Circular references

        public class C12
        {
            private int i1CallOunt = 0;
            private int i2CallOunt = 0;

            public I1 I1
            {
                get; private set;
            }
            public int I1CallCount { get { return i1CallOunt; } }
            public I2 I2
            {
                get; private set;
            }
            public int I2CallCount { get { return i2CallOunt; } }

            [IoCInject]
            public void SetI1(I1 i1)
            {
                Interlocked.Increment(ref i1CallOunt);
                this.I1 = i1;
            }

            public void SetI2(I2 i2)
            {
                Interlocked.Increment(ref i2CallOunt);
                this.I2 = i2;
            }
        }


        public class CircularConstructori4 : I4
        {

            public I5 I5
            {
                get; private set;
            }

            public CircularConstructori4(I5 i5)
            {
                this.I5 = i5;
            }
        }

        public class CircularConstructori5 : I5
        {

            public I4 I4
            {
                get; private set;
            }

            public CircularConstructori5(I4 i4)
            {
                this.I4 = i4;
            }
        }

        public class CircularMethodi4 : I4
        {

            private int m_callCount = 0;

            public I5 I5
            {
                get; private set;
            }

            public int CallCount { get { return m_callCount; } }

            [IoCInject]
            public void SetI1(I5 i5)
            {
                Interlocked.Increment(ref m_callCount);
                this.I5 = i5;
            }
        }

        public class CircularMethodi5 : I5
        {
            private int m_callCount = 0;
            public I4 I4
            {
                get; private set;
            }


            public int CallCount { get { return m_callCount; } }

            [IoCInject]
            public void SetI1(I4 i4)
            {
                Interlocked.Increment(ref m_callCount);
                this.I4 = i4;
            }
        }

        #endregion

        public class CResolveMany
        {
            [IoCResolveMany]
            internal IEnumerable<I1> m_allI1;

            [IoCResolveMany]
            internal IEnumerable<Lazy<I1>> m_allI2;
        }

        internal class TestRegistry : IIoCRegistry
        {
            [ThreadStatic]
            private static int m_CreateInstanceCallCount;

            [ThreadStatic]
            private static int m_HasImplementationForTypeCallCount;

            internal int CreateInstanceCallCount { get { return m_CreateInstanceCallCount; } }

            internal int HasImplementationForTypeCallCount { get { return m_HasImplementationForTypeCallCount; } }

            internal static int SCreateInstanceCallCount { get { return m_CreateInstanceCallCount; } }

            internal static int SHasImplementationForTypeCallCount { get { return m_HasImplementationForTypeCallCount; } }

            public TestRegistry()
            {
                m_CreateInstanceCallCount = 0;
                m_HasImplementationForTypeCallCount = 0;
            }

            public object CreateInstance(Type interfaceType)
            {
                Interlocked.Increment(ref m_CreateInstanceCallCount);
                if (interfaceType == typeof(I1))
                {
                    return new C1();
                }
                else if (interfaceType == typeof(C1))
                {
                    return new C1();
                }
                else
                {
                    return null;
                }
            }

            public TInterface CreateInstance<TInterface>() where TInterface : class
            {
                return CreateInstance(typeof(TInterface)) as TInterface;
            }

            public void Dispose()
            {

            }

            public bool HasImplementationForType(Type type)
            {
                Interlocked.Increment(ref m_HasImplementationForTypeCallCount);
                return type == typeof(C1) || type == typeof(I1);
            }
        }

        #endregion
    }
}

namespace IoCTestInstances1
{


    internal class C1 : FrodLib.Tests.IoCContainerTests.I1
    {
        public C1()
        {

        }
    }

}

namespace IoCTestInstances2
{



    internal class C6 : FrodLib.Tests.IoCContainerTests.I1
    {

    }


}
