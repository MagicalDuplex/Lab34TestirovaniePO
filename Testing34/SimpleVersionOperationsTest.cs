using SemVer;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Testing34
{
    class SimpleVersionOperationsTest
    {
        [Test]
        public void EqualVersions()
        {
            var a = new Version("1.2.3");
            var b = new Version("1.2.3");
            Assert.IsTrue(a == b);
            Assert.IsTrue(a.Equals(b));
            var obja = (object)a;
            Assert.IsTrue(obja.Equals(b));
        }

        [Test]
        public void GreaterThan()
        {
            var versionA = new Version("1.2.3");
            var versionB = new Version("1.2.2");
            Assert.IsTrue(versionA > versionB);

            versionA = new Version("1.2.3");
            versionB = new Version("1.1.3");
            Assert.IsTrue(versionA > versionB);

            versionA = new Version("1.2.3");
            versionB = new Version("0.2.3");
            Assert.IsTrue(versionA > versionB);
        }


        [Test]
        public void NotGreaterThan()
        {
            var versionA = new Version("1.2.3");
            var versionB = new Version("1.2.3");
            Assert.IsFalse(versionA > versionB);

            versionA = new Version("1.2.3");
            versionB = new Version("1.2.4");
            Assert.IsFalse(versionA > versionB);
        }


        [Test]
        public void GreaterThanOrEqual()
        {
            var versionA = new Version("1.2.3");
            var versionB = new Version("1.2.3");
            Assert.IsTrue(versionA >= versionB);

            versionA = new Version("1.2.3");
            versionB = new Version("1.2.2");
            Assert.IsTrue(versionA >= versionB);

            versionA = new Version("1.2.3");
            versionB = new Version("1.1.3");
            Assert.IsTrue(versionA >= versionB);
        }

        [Test]
        public void NotGreaterThanOrEqual()
        {
            var versionA = new Version("1.2.3");
            var versionB = new Version("1.2.4");
            Assert.IsFalse(versionA >= versionB);
        }

        [Test]
        public void LessThan()
        {
            var versionA = new Version("1.2.3");
            var versionB = new Version("1.2.4");
            Assert.IsTrue(versionA < versionB);
        }

        [Test]
        public void NotLessThan()
        {
            var versionA = new Version("1.2.3");
            var versionB = new Version("1.2.3");
            Assert.IsFalse(versionA < versionB);
        }

        [Test]
        public void LessThanOrEqual()
        {
            var versionA = new Version("1.2.3");
            var versionB = new Version("1.2.3");
            Assert.IsTrue(versionA <= versionB);
        }

        [Test]
        public void NotLessThanOrEqual()
        {
            var versionA = new Version("1.2.3");
            var versionB = new Version("1.2.2");
            Assert.IsFalse(versionA <= versionB);
        }

        [Test]
        public void CompareTo()
        {
            var actual =
                new[]
                {
                    "1.5.0",
                    "0.0.4",
                    "0.1.0",
                    "0.1.1",
                    "0.0.2",
                    "1.0.0"
                }
                .Select(v => new Version(v))
                .Cast<System.IComparable>()
                .OrderBy(x => x)
                .Select(x => x.ToString());

            var expected =
                new[]
                {
                    "0.0.2",
                    "0.0.4",
                    "0.1.0",
                    "0.1.1",
                    "1.0.0",
                    "1.5.0"
                };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CompareToNull()
        {
            var version = (System.IComparable)new Version("1.0.0");
            var actual = version.CompareTo(null);
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void CompareToDifferentType()
        {
            var version = (System.IComparable)new Version("1.0.0");
            Assert.Throws<System.ArgumentException>(() => version.CompareTo(new System.Object()));
        }

        [Test]
        public void EqualHashCode()
        {
            var a = new Version("1.2.3");
            var b = new Version("1.2.3");
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void DifferentHashCode()
        {
            var a = new Version("1.2.3");
            var b = new Version("1.2.4");
            Assert.IsFalse(a.GetHashCode() == b.GetHashCode());
        }
    }
}
