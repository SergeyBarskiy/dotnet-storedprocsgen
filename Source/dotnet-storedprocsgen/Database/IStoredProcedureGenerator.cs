using System.Collections.Generic;

namespace StoredProcsGenerator.Database
{
    public interface IStoredProcedureGenerator
    {
        string GetHeader(StoredProcedureKind procedureKind, string prefix, string tableName, string schema, bool includeDrop, bool uppercaseName);
        string GetProcedureName(StoredProcedureKind procedureKind, string prefix, string tableName, bool uppercaseName);
        string GetBody(StoredProcedureKind procedureKind, List<ColumnInfo> columns, string sortByColumns, string searchColumns);
    }
}