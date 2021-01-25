using System;
using System.Collections.Generic;
using System.Linq;

namespace SemVer
{
    // определяет валидные версии
    public class Range : IEquatable<Range>
    {
        private readonly ComparatorSet[] _comparatorSets;

        private readonly string _rangeSpec;

        // новый ряд
        public Range(string rangeSpec)
        {
            _rangeSpec = rangeSpec;
            var comparatorSetSpecs = rangeSpec.Split(new[] { "||" }, StringSplitOptions.None);
            _comparatorSets = comparatorSetSpecs.Select(s => new ComparatorSet(s)).ToArray();
        }

        private Range(IEnumerable<ComparatorSet> comparatorSets)
        {
            _comparatorSets = comparatorSets.ToArray();
            _rangeSpec = string.Join(" || ", _comparatorSets.Select(cs => cs.ToString()).ToArray());
        }

        // существует ли указанная версия в ряду
        public bool Contains(Version version)
        {
            return _comparatorSets.Any(s => s.Containss(version));
        }

        // существует ли указанная версия в ряду
        public bool Contains(string versionString)
        {
            try
            {
                var version = new Version(versionString);
                return Contains(version);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public bool Contains(Range versions)
        {
            var allIntersections = _comparatorSets.SelectMany(
                thisCs => versions._comparatorSets.Select(thisCs.Intersect))
                .Where(cs => cs != null).ToList();

            if (allIntersections.Count > 0)
            {
                return true;
            }
            return false;
        }

        // набор версий удовлетворяющий ряду
        public IEnumerable<Version> Contains(IEnumerable<Version> versions)
        {
            return versions.Where(Contains);
        }

        // набор версий удовлетворяющий ряду
        public IEnumerable<string> Contains(IEnumerable<string> versions)
        {
            return versions.Where(v => Contains(v));
        }

        // максимальная версия удовлетворяющая ряду
        public Version MaxContains(IEnumerable<Version> versions)
        {
            return Contains(versions).Max();
        }

        // максимальная версия удовлетворяющая ряду
        public string MaxContains(IEnumerable<string> versionStrings)
        {
            var versions = ValidVersions(versionStrings);
            var maxVersion = MaxContains(versions);
            return maxVersion == null ? null : maxVersion.ToString();
        }

        // пересечение рядов
        public Range Intersect(Range other)
        {
            var allIntersections = _comparatorSets.SelectMany(
                thisCs => other._comparatorSets.Select(thisCs.Intersect))
                .Where(cs => cs != null).ToList();

            if (allIntersections.Count == 0)
            {
                return new Range("<0.0.0");
            }
            return new Range(allIntersections);
        }

        // строковое представление ряда
        public override string ToString()
        {
            return _rangeSpec;
        }

        // проверка эквивалетности ряда
        public bool Equals(Range other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            var thisSet = new HashSet<ComparatorSet>(_comparatorSets);
            return thisSet.SetEquals(other._comparatorSets);
        }

        // перегрузки ...
        public override bool Equals(object other)
        {
            return Equals(other as Range);
        }

        public static bool operator ==(Range a, Range b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            return a.Equals(b);
        }

        public static bool operator !=(Range a, Range b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {

            return _comparatorSets.Aggregate(0, (accum, next) => accum ^ next.GetHashCode());
        }


        public static bool Contains(string rangeSpec, string versionString)
        {
            var range = new Range(rangeSpec);
            return range.Contains(versionString);
        }


        public static IEnumerable<string> Contains(string rangeSpec, IEnumerable<string> versions)
        {
            var range = new Range(rangeSpec);
            return range.Contains(versions);
        }

        public static string MaxContains(string rangeSpec, IEnumerable<string> versionStrings)
        {
            var range = new Range(rangeSpec);
            return range.MaxContains(versionStrings);
        }


        public static Range Parse(string rangeSpec)
            => new Range(rangeSpec);

        public static bool TryParse(string rangeSpec, out Range result)
        {
            try
            {
                result = Parse(rangeSpec);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private IEnumerable<Version> ValidVersions(IEnumerable<string> versionStrings)
        {
            foreach (var v in versionStrings)
            {
                Version version = null;

                try
                {
                    version = new Version(v);
                }
                catch (ArgumentException) { }

                if (version != null)
                {
                    yield return version;
                }
            }
        }
    }
}