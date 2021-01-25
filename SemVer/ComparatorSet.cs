using System;
using System.Collections.Generic;
using System.Linq;

namespace SemVer
{
    public class ComparatorSet : IEquatable<ComparatorSet>
    {
        private readonly List<Comparator> _comparators;

        public ComparatorSet(string spec)
        {
            _comparators = new List<Comparator> { };

            spec = spec.Trim();
            if (spec == "")
            {
                spec = "*";
            }

            int position = 0;
            int end = spec.Length;

            while (position < end)
            {
                int iterStartPosition = position;

                foreach (var interval in new Func<string, Tuple<int, Comparator[]>>[] {
                        Interval.CaretRange
                        })
                {
                    var result = interval(spec.Substring(position));
                    if (result != null)
                    {
                        position += result.Item1;
                        _comparators.AddRange(result.Item2);
                    }
                }

                var comparatorResult = Comparator.TryParse(spec.Substring(position));
                if (comparatorResult != null)
                {
                    position += comparatorResult.Item1;
                    _comparators.Add(comparatorResult.Item2);
                }

                if (position == iterStartPosition)
                {
                    throw new ArgumentException(String.Format("Invalid range specification: \"{0}\"", spec));
                }
            }
        }

        private ComparatorSet(IEnumerable<Comparator> comparators)
        {
            _comparators = comparators.ToList();
        }

        public bool Containss(Version version)
        {
            bool satisfied = _comparators.All(c => c.Containss(version));
            if (version.PreRelease != null)
            {
                return satisfied && _comparators.Any(c =>
                        c.Version.PreRelease != null &&
                        c.Version.BaseVersion() == version.BaseVersion());
            }
            else
            {
                return satisfied;
            }
        }

        public ComparatorSet Intersect(ComparatorSet other)
        {
            Func<Comparator, bool> operatorIsGreaterThan = c =>
                c.ComparatorType == Comparator.Operator.GreaterThan ||
                c.ComparatorType == Comparator.Operator.GreaterThanOrEqual;
            Func<Comparator, bool> operatorIsLessThan = c =>
                c.ComparatorType == Comparator.Operator.LessThan ||
                c.ComparatorType == Comparator.Operator.LessThanOrEqual;
            var maxOfMins =
                _comparators.Concat(other._comparators)
                .Where(operatorIsGreaterThan)
                .OrderByDescending(c => c.Version).FirstOrDefault();
            var minOfMaxs =
                _comparators.Concat(other._comparators)
                .Where(operatorIsLessThan)
                .OrderBy(c => c.Version).FirstOrDefault();
            if (maxOfMins != null && minOfMaxs != null && !maxOfMins.Intersects(minOfMaxs))
            {
                return null;
            }

            var equalityVersions =
                _comparators.Concat(other._comparators)
                .Where(c => c.ComparatorType == Comparator.Operator.Equal)
                .Select(c => c.Version)
                .ToList();
            if (equalityVersions.Count > 1)
            {
                if (equalityVersions.Any(v => v != equalityVersions[0]))
                {
                    return null;
                }
            }
            if (equalityVersions.Count > 0)
            {
                if (maxOfMins != null && !maxOfMins.Containss(equalityVersions[0]))
                {
                    return null;
                }
                if (minOfMaxs != null && !minOfMaxs.Containss(equalityVersions[0]))
                {
                    return null;
                }
                return new ComparatorSet(
                    new List<Comparator>
                    {
                        new Comparator(Comparator.Operator.Equal, equalityVersions[0])
                    });
            }

            var comparators = new List<Comparator>();
            if (maxOfMins != null)
            {
                comparators.Add(maxOfMins);
            }
            if (minOfMaxs != null)
            {
                comparators.Add(minOfMaxs);
            }

            return comparators.Count > 0 ? new ComparatorSet(comparators) : null;
        }

        public bool Equals(ComparatorSet other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            var thisSet = new HashSet<Comparator>(_comparators);
            return thisSet.SetEquals(other._comparators);
        }

        public override bool Equals(object other)
        {
            return Equals(other as ComparatorSet);
        }

        public override string ToString()
        {
            return string.Join(" ", _comparators.Select(c => c.ToString()).ToArray());
        }

        public override int GetHashCode()
        {
            return _comparators.Aggregate(0, (accum, next) => accum ^ next.GetHashCode());
        }
    }
}