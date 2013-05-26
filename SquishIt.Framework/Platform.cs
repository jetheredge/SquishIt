using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SquishIt.Framework
{
    public class Platform
    {
        public static bool Unix
        {
            get { return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX; }
        }

        static bool _monoVersionChecked;
        static Version _monoVersion;
        public static Version MonoVersion
        {
            get
            {
                if (!_monoVersionChecked)
                {
                    var type = Type.GetType("Mono.Runtime");
                    if (type != null)
                    {
                        var displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                        if (displayName != null)
                        {
                            var versionString = displayName.Invoke(null, null).ToString();

                            var regex = new Regex(@"[0-9]+\.[0-9]+\.[0-9]+");

                            var versionName = regex.Matches(versionString)[0].Value;

                            _monoVersion = new Version(versionName);
                        }
                    }
                    _monoVersionChecked = true;
                }
                return _monoVersion;
            }
        }

        public static bool Mono
        {
            get { return MonoVersion != null; }
        }
    }
    public class Version
    {
        public Version(string versionName)
        {
            var numbers = versionName.Split('.');
            if (numbers.Length > 0)
            {
                Major = int.Parse(numbers[0]);
            }
            if (numbers.Length > 1)
            {
                Minor = int.Parse(numbers[1]);
            }
            if (numbers.Length > 2)
            {
                Build = int.Parse(numbers[2]);
            }
        }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }

        public static bool operator <=(Version x, Version y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException("Can't compare null versions");
            }

            if (x.Major == y.Major)
            {
                if (x.Minor == y.Minor)
                {
                    return x.Build <= y.Build;
                }
                return x.Minor <= y.Minor;
            }
            return x.Major <= y.Major;
        }

        public static bool operator >=(Version x, Version y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException("Can't compare null versions");
            }

            if (x.Major == y.Major)
            {
                if (x.Minor == y.Minor)
                {
                    return x.Build >= y.Build;
                }
                return x.Minor >= y.Minor;
            }
            return x.Major >= y.Major;
        }

        public static bool operator <(Version x, Version y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException("Can't compare null versions");
            }

            if (x.Major == y.Major)
            {
                if (x.Minor == y.Minor)
                {
                    return x.Build < y.Build;
                }
                return x.Minor < y.Minor;
            }
            return x.Major < y.Major;
        }

        public static bool operator >(Version x, Version y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException("Can't compare null versions");
            }

            if (x.Major == y.Major)
            {
                if (x.Minor == y.Minor)
                {
                    return x.Build > y.Build;
                }
                return x.Minor > y.Minor;
            }
            return x.Major > y.Major;
        }
    }
}

