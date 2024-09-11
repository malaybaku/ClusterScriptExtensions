using System;

namespace Baxter.ClusterScriptExtensions
{
    [Serializable]
    public struct FieldDefinedLocation
    {
        public int startLine;
        public int startColumn;
        public int endLine;
        public int endColumn;
    }
}