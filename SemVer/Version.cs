using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SemVer
{
    // семантическая модель
    public class Version : IComparable<Version>, IComparable, IEquatable<Version>
    {
        private readonly string _inputString;
        private readonly int _major;
        private readonly int _minor;
        private readonly int _patch;
        private readonly string _preRelease;
        private readonly string _build;

        public int Major { get { return _major; } }

        public int Minor { get { return _minor; } }

        public int Patch { get { return _patch; } }
        public string PreRelease { get { return _preRelease; } }


        public string Build { get { return _build; } }

        private static Regex strictRegex = new Regex(@"^
            \s*v?
            ([0-9]|[1-9][0-9]+)       # major version
            \.
            ([0-9]|[1-9][0-9]+)       # minor version
            \.
            ([0-9]|[1-9][0-9]+)       # patch version
            (\-([0-9A-Za-z\-\.]+))?   # pre-release version
            (\+([0-9A-Za-z\-\.]+))?   # build metadata
            \s*
            $",
            RegexOptions.IgnorePatternWhitespace);


        // перевод строки в экземпляр класса version
        public Version(string input)
        {
            _inputString = input;

            var regex = strictRegex;

            var match = regex.Match(input);
            if (!match.Success)
            {
                throw new ArgumentException(String.Format("Invalid version string: {0}", input));
            }

            _major = Int32.Parse(match.Groups[1].Value);

            _minor = Int32.Parse(match.Groups[2].Value);

            _patch = Int32.Parse(match.Groups[3].Value);

            if (match.Groups[4].Success)
            {
                var inputPreRelease = match.Groups[5].Value;
                var cleanedPreRelease = PreReleaseVersion.Clean(inputPreRelease);
                if (inputPreRelease != cleanedPreRelease)
                {
                    throw new ArgumentException(String.Format(
                                "Invalid pre-release version: {0}", inputPreRelease));
                }
                _preRelease = cleanedPreRelease;
            }

            if (match.Groups[6].Success)
            {
                _build = match.Groups[7].Value;
            }
        }

        // получение экземпляра класса version из его составляющих
        public Version(int major, int minor, int patch,
                string preRelease = null, string build = null)
        {
            _major = major;
            _minor = minor;
            _patch = patch;
            _preRelease = preRelease;
            _build = build;
        }

        // получение версии без патча и предрелизной версии
        public Version BaseVersion()
        {
            return new Version(Major, Minor, Patch);
        }


        // возвращает оригинальное строковое представление экземпляра класса version 
        public override string ToString()
        {
            return _inputString ?? Clean();
        }


        public string Clean()
        {
            var preReleaseString = PreRelease == null ? ""
                : String.Format("-{0}", PreReleaseVersion.Clean(PreRelease));
            var buildString = Build == null ? ""
                : String.Format("+{0}", Build);

            return String.Format("{0}.{1}.{2}{3}{4}",
                    Major, Minor, Patch, preReleaseString, buildString);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Major.GetHashCode();
                hash = hash * 23 + Minor.GetHashCode();
                hash = hash * 23 + Patch.GetHashCode();
                if (PreRelease != null)
                {
                    hash = hash * 23 + PreRelease.GetHashCode();
                }
                return hash;
            }
        }

        // проверка эквивалентности
        public bool Equals(Version other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return CompareTo(other) == 0;
        }

        // реализация IComparable
        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case null:
                    return 1;
                case Version v:
                    return CompareTo(v);
                default:
                    throw new ArgumentException("Object is not a Version");
            }
        }

        // реализация IComparable<Version>
        public int CompareTo(Version other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }

            foreach (var c in PartComparisons(other))
            {
                if (c != 0)
                {
                    return c;
                }
            }

            return PreReleaseVersion.Compare(this.PreRelease, other.PreRelease);
        }

        private IEnumerable<int> PartComparisons(Version other)
        {
            yield return Major.CompareTo(other.Major);
            yield return Minor.CompareTo(other.Minor);
            yield return Patch.CompareTo(other.Patch);
        }

        public override bool Equals(object other)
        {
            return Equals(other as Version);
        }


        public static Version Parse(string input)
            => new Version(input);

        public static bool TryParse(string input, out Version result)
        {
            try
            {
                result = Parse(input);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        // перегрузка операторов для работы с экземплярами класса version
        public static bool operator ==(Version a, Version b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            return a.Equals(b);
        }

        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }

        public static bool operator >(Version a, Version b)
        {
            if (ReferenceEquals(a, null))
            {
                return false;
            }
            return a.CompareTo(b) > 0;
        }

        public static bool operator >=(Version a, Version b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null) ? true : false;
            }
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <(Version a, Version b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null) ? false : true;
            }
            return a.CompareTo(b) < 0;
        }

        public static bool operator <=(Version a, Version b)
        {
            if (ReferenceEquals(a, null))
            {
                return true;
            }
            return a.CompareTo(b) <= 0;
        }
    }
}
