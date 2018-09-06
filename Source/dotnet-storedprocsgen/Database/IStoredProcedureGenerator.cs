using System.Collections.Generic;

namespace StoredProcsGenerator.Database
{
    public interface IStoredProcedureGenerator
    {
        string GetHeader(StoredProcedureKind procedureKind, string prefix, string tableName, string schema, bool includeDrop);
        string GetProcedureName(StoredProcedureKind procedureKind, string prefix, string tableName);
        string GetBody(StoredProcedureKind procedureKind, List<ColumnInfo> columns, string sortByColumns, string searchColumns);
    }
}