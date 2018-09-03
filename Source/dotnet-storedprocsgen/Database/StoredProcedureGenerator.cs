using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoredProcsGenerator.Database
{
    public class StoredProcedureGenerator : IStoredProcedureGenerator
    {
        public string GetHeader(StoredProcedureKind procedureKind, string prefix, string tableName, string schema, bool includeDrop)
        {
            var result = new StringBuilder();
            string fullName = GetProcedureName(procedureKind, prefix, tableName);
            if (includeDrop)
            {
                result.AppendLine($"IF EXISTS(SELECT * FROM sys.procedures WHERE name = '{fullName}')");
                result.AppendLine("BEGIN");
                result.AppendLine($"\tDROP PROCEDURE [{schema}].[{fullName}]");
                result.AppendLine("END");
                result.AppendLine("GO");
            }
            result.AppendLine($"CREATE PROCEDURE [{schema}].[{fullName}]");
            return result.ToString();
        }

        public string GetProcedureName(StoredProcedureKind procedureKind, string prefix, string tableName)
        {
            return $"{prefix}_{tableName}_{procedureKind}"; ;
        }

        public string GetBody(StoredProcedureKind procedureKind, List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            result.AppendLine("(");
            if (procedureKind == StoredProcedureKind.Insert)
            {
                result.Append(GetInsertStatementParameters(columns));
            }
            else if (procedureKind == StoredProcedureKind.Update)
            {
                result.Append(GetUpdateStatementParameters(columns));
            }
            else if (procedureKind == StoredProcedureKind.Delete)
            {
                result.Append(GetDeleteStatementParameters(columns));
            }
            result.AppendLine(")");
            result.AppendLine("AS");
            var firstColumn = columns.First();
            if (procedureKind == StoredProcedureKind.Insert)
            {
                result.Append($"INSERT INTO [{firstColumn.SchemaName}].[{firstColumn.TableName}]");
                result.AppendLine("(");
                result.Append(GetInsertStatementColumnsList(columns));
                result.AppendLine(")");
                result.AppendLine(GetInsertStatementOutput(columns));
                result.AppendLine("VALUES");
                result.AppendLine("(");
                result.Append(GetInsertStatementValues(columns));
                result.AppendLine(")");
            }
            else if (procedureKind == StoredProcedureKind.Update)
            {
                result.AppendLine($"UPDATE [{firstColumn.SchemaName}].[{firstColumn.TableName}] SET");
                result.Append(GetUpdateStatementColumnsList(columns));
                result.AppendLine(GetInsertStatementOutput(columns));
                result.AppendLine("WHERE");
                result.AppendLine(GetUpdateStatementWhere(columns));
            }
            else if (procedureKind == StoredProcedureKind.Delete)
            {
                result.AppendLine($"DELETE FROM [{firstColumn.SchemaName}].[{firstColumn.TableName}]");
                result.AppendLine("WHERE");
                result.AppendLine(GetUpdateStatementWhere(columns));
            }
            return result.ToString();
        }

        private string GetUpdateStatementParameters(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var filteredColumns = GetFilteredColumnsListForUpdate(columns);
            filteredColumns.ForEach(column =>
            {
                var comma = ",";
                if (filteredColumns.IndexOf(column) == filteredColumns.Count - 1)
                {
                    comma = "";
                }
                result.AppendLine($"\t@{column.ColumnName} {column.ColumnFullType}{comma}");
            });
            return result.ToString();
        }

        private string GetDeleteStatementParameters(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var keyColumns = columns.Where(c => c.PrimaryKeyColumnPosition > 0).ToList();
            var timeStamp = columns.FirstOrDefault(c => c.IsRowVersion);
            if (keyColumns.Any() && timeStamp != null)
            {
                keyColumns.ForEach(key =>
                {
                    var comma = ",";
                    result.AppendLine($"@{key.ColumnName} {key.ColumnFullType}{comma}");
                });
                result.AppendLine($"@{timeStamp.ColumnName} {timeStamp.ColumnFullType}");

            }
            else if (keyColumns.Any())
            {

                keyColumns.ForEach(key =>
                {
                    var comma = ",";
                    if (keyColumns.IndexOf(key) == keyColumns.Count - 1)
                    {
                        comma = "";
                    }
                    result.AppendLine($"@{key.ColumnName} {key.ColumnFullType}{comma}");
                });
            }

            return result.ToString();
        }

        private string GetInsertStatementParameters(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var filteredColumns = GetFilteredColumnsListForInsert(columns);
            filteredColumns.ForEach(column =>
            {
                var comma = ",";
                if (filteredColumns.IndexOf(column) == filteredColumns.Count - 1)
                {
                    comma = "";
                }
                result.AppendLine($"\t@{column.ColumnName} {column.ColumnFullType}{comma}");
            });
            return result.ToString();
        }

        private string GetInsertStatementValues(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var filteredColumns = GetFilteredColumnsListForInsert(columns);
            filteredColumns.ForEach(column =>
            {
                var comma = ",";
                if (filteredColumns.IndexOf(column) == filteredColumns.Count - 1)
                {
                    comma = "";
                }
                result.AppendLine($"\t@{column.ColumnName}{comma}");
            });
            return result.ToString();
        }



        private string GetInsertStatementColumnsList(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var filteredColumns = GetFilteredColumnsListForInsert(columns);
            filteredColumns.ForEach(column =>
            {
                var comma = ",";
                if (filteredColumns.IndexOf(column) == filteredColumns.Count - 1)
                {
                    comma = "";
                }
                result.AppendLine($"\t[{column.ColumnName}]{comma}");
            });
            return result.ToString();
        }

        private string GetUpdateStatementColumnsList(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var filteredColumns = GetFilteredColumnsListForInsert(columns);
            filteredColumns.ForEach(column =>
            {
                var comma = ",";
                if (filteredColumns.IndexOf(column) == filteredColumns.Count - 1)
                {
                    comma = "";
                }
                result.AppendLine($"\t[{column.ColumnName}] = @{column.ColumnName}{comma}");
            });
            return result.ToString();
        }

        private string GetUpdateStatementWhere(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var keyColumns = columns.Where(c => c.PrimaryKeyColumnPosition > 0).ToList();
            var timeStamp = columns.FirstOrDefault(c => c.IsRowVersion);
            if (keyColumns.Any() && timeStamp != null)
            {
                keyColumns.ForEach(key =>
                {
                    result.AppendLine($"\t[{key.ColumnName}] = @{key.ColumnName} AND ");
                });
                result.AppendLine($"\t[{timeStamp.ColumnName}]= @{timeStamp.ColumnName}");

            }
            else if (keyColumns.Any())
            {
                var and = "AND";
                keyColumns.ForEach(key =>
                {
                    if (keyColumns.IndexOf(key) == keyColumns.Count - 1)
                    {
                        and = "";
                    }
                    result.AppendLine($"\t[{key.ColumnName}] = @{key.ColumnName} {and} ");
                });
            }

            return result.ToString();
        }

        private string GetInsertStatementOutput(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var identity = columns.FirstOrDefault(c => c.IdentityColumn);
            var timeStamp = columns.FirstOrDefault(c => c.IsRowVersion);
            if (identity != null && timeStamp != null)
            {
                result.Append($"OUTPUT inserted.[{identity.ColumnName}], inserted.[{timeStamp.ColumnName}]");
            }
            else if (identity != null)
            {
                result.Append($"OUTPUT inserted.[{identity.ColumnName}]");
            }
            else if (timeStamp != null)
            {
                result.Append($"OUTPUT inserted.[{timeStamp.ColumnName}]");
            }
            return result.ToString();
        }


        private static List<ColumnInfo> GetFilteredColumnsListForInsert(List<ColumnInfo> columns)
        {
            return columns.Where(c => !c.IdentityColumn && !c.Computed && !c.IsRowVersion).ToList();
        }

        private static List<ColumnInfo> GetFilteredColumnsListForUpdate(List<ColumnInfo> columns)
        {
            return columns.Where(c => !c.Computed).ToList();
        }
    }
}
