using System;
using System.Collections.Generic;
using System.Text;

namespace StoredProcsGenerator.Database
{
    public class ColumnInfo
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string SchemaName { get; set; }
        public string TypeName { get; set; }
        public int MaximumLength { get; set; }
        public int ColumnPrecision { get; set; }
        public int ColumnScale { get; set; }
        public int OrdinalPosition { get; set; }
        public string ColumnFullType { get; set; }
        public bool Computed { get; set; }
        public bool IdentityColumn { get; set; }
        public bool Nullable { get; set; }
        public int PrimaryKeyColumnPosition { get; set; }
        public bool IsRowVersion { get; set; }
        public bool IsCustomRowVersion { get; set; }
        public bool IsParentColumn { get; set; }
    }
}
