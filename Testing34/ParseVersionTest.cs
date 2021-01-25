using SemVer;
using NUnit.Framework;

namespace Testing34
{
    class ParseVersionTest
    {
        [Test]
        public void ParseMajorVersion()
        {
            var version = new Version("1.2.3");
            Assert.AreEqual(version.Major, 1);

            version = new Version(" 1.2.3 ");
            Assert.AreEqual(version.Major, 1);

            version = new Version(" 2.2.3-4 ");
            Assert.AreEqual(version.Major, 2);

            version = new Version(" 3.2.3-pre ");
            Assert.AreEqual(version.Major, 3);
        }


        [Test]
        public void ParseMinorVersion()
        {
            var version = new Version("1.2.3");
            Assert.AreEqual(version.Minor, 2);

            version = new Version(" 1.2.3 ");
            Assert.AreEqual(version.Minor, 2);

            version = new Version(" 3.2.3-pre ");
            Assert.AreEqual(version.Minor, 2);

            version = new Version(" 2.2.3-4 ");
            Assert.AreEqual(version.Minor, 2);
        }

        [Test]
        public void ParsePatchVersion()
        {
            var version = new Version("1.2.3");
            Assert.AreEqual(version.Patch, 3);

            version = new Version(" 1.2.3 ");
            Assert.AreEqual(version.Patch, 3);

            version = new Version(" 2.2.3-4 ");
            Assert.AreEqual(version.Patch, 3);

            version = new Version(" 3.2.3-pre ");
            Assert.AreEqual(version.Patch, 3);
        }


        [Test]
        public void BadVersion()
        {
            var ex = Assert.Throws<System.ArgumentException>(() => new Version("Not semver"));
            Assert.That(ex.Message, Is.EqualTo("Invalid version string: Not semver"));

            ex = Assert.Throws<System.ArgumentException>(() => new Version("666"));
            Assert.That(ex.Message, Is.EqualTo("Invalid version string: 666"));
        }

        [Test]
        public void BadVersionRange()
        {
            var success = Range.TryParse("Not semver range", out var range);
            Assert.AreEqual(false, success);
        }
    }
}
