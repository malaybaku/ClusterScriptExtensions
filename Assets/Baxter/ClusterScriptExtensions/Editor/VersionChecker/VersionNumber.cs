using System;

namespace Baxter.ClusterScriptExtensions.Editor.VersionChecker
{
    /// <summary>
    /// ClusterScriptExtensionsのバージョン値。
    /// GitHubのリリースタグとpackage.jsonの2箇所で、リリースごとに共通の値が適用される。
    /// </summary>
    public readonly struct VersionNumber : IEquatable<VersionNumber>, IComparable<VersionNumber>
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Revision;

        public VersionNumber(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public static readonly VersionNumber Zero = new(0, 0, 0);
        
        public override string ToString() => $"{Major}.{Minor}.{Revision}";

        public bool Equals(VersionNumber other) 
            => Major == other.Major && Minor == other.Minor && Revision == other.Revision;

        public override bool Equals(object obj) 
            => obj is VersionNumber other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Major, Minor, Revision);

        public int CompareTo(VersionNumber other)
        {
            if (Major > other.Major) return 1;
            if (Major < other.Major) return -1;

            if (Minor > other.Minor) return 1;
            if (Minor < other.Minor) return -1;

            if (Revision > other.Revision) return 1;
            if (Revision < other.Revision) return -1;

            return 0;
        }

        public static bool TryParse(string value, out VersionNumber result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = Zero;
                return false;
            }
            
            var numbers = value.TrimStart('v').Split('.');
            if (numbers.Length == 3 &&
                int.TryParse(numbers[0], out var major) &&
                int.TryParse(numbers[1], out var minor) &&
                int.TryParse(numbers[2], out var revision)
                )
            {
                result = new VersionNumber(major, minor, revision);
                return true;
            }

            result = Zero;
            return false;
        }
    }


}
