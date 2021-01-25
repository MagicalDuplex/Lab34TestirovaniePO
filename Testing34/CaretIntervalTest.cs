using NUnit.Framework;
using SemVer;

namespace Testing34
{
    class CaretIntervalTest
    {

        [Test]
        public void TestCaretRangeContainVersion()
        {
            var range = new Range("^1.2.3");
            Assert.IsTrue(range.Contains("1.2.3"));
            Assert.IsTrue(range.Contains("1.9.9"));
            Assert.IsFalse(range.Contains("2.0.0"));
            Assert.IsFalse(range.Contains("1.2.2"));
        }

        [Test]
        public void TestCaretRangeContainRange()
        {
            var range = new Range("^1.2.3");
            var containRange = new Range(">=1.2.3 <2.0.0");
            var notcontainRange = new Range(">=0.0.0 <1.2.3");
            Assert.IsTrue(range.Contains(containRange));
            Assert.IsFalse(range.Contains(notcontainRange)); 
        }

        [Test]
        public void TestCaretrangeEqual1()
        {
            var range = new Range("^3.x");
            var equalRange = new Range(">=3.0 <4.0");
            Assert.IsTrue(range.Contains("3.2.9"));
            Assert.IsTrue(range.Contains("3.0.0"));
            Assert.IsFalse(range.Contains("2.9.0"));
            Assert.IsTrue(range.Equals(equalRange));
            Assert.IsTrue(range == equalRange);
            Assert.IsFalse(range != equalRange);
        }

        [Test]
        public void TestCaretRangeEqual2()
        {
            var range = new Range("^4");
            var equalRange = new Range(">=4.0.0 <5.0");
            Assert.IsTrue(range.Contains("4.1.4"));
            Assert.IsTrue(range.Contains("4.1.9"));
            Assert.IsFalse(range.Contains("1.2.4"));
            Assert.IsTrue(range.Equals(equalRange));
            Assert.IsTrue(range == equalRange);
            Assert.IsFalse(range != equalRange);
        }

        [Test]
        public void notEqualRanges()
        {
            var arange = new Range("^3");
            var brange = new Range("^4");
            Assert.IsFalse(arange.Equals(brange));
            Assert.IsFalse(arange == brange);
            Assert.IsTrue(arange != brange);
        }

    }
}
