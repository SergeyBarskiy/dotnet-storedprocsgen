using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoredProcsGenerator.Database
{
    public class StoredProcedureGenerator : IStoredProcedureGenerator
    {
        private readonly string selectQueryString = "SELECT ";
        private readonly string whereQueryString = "WHERE";
        private readonly string fromQueryString = " FROM ";
        private readonly string orderByQueryString = " ORDER BY ";
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

        public string GetBody(StoredProcedureKind procedureKind, List<ColumnInfo> columns, string sortByColumns, string searchColumns)
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
            else if (procedureKind == StoredProcedureKind.Search)
            {
                result.Append(GetSearchParameters());
            }
            else if (procedureKind == StoredProcedureKind.GetById)
            {
                result.Append(GetGetByIdStatementParameters(columns));
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
            else if (procedureKind == StoredProcedureKind.Search)
            {
                result.AppendLine(GetSearchRawData(columns, sortByColumns, searchColumns));
                result.AppendLine(GetSearchFinalSelect());
            }
            else if (procedureKind == StoredProcedureKind.GetById)
            {
                result.AppendLine(@"BEGIN");
                result.AppendLine(@"SET NOCOUNT ON;");
                result.Append(selectQueryString);
                result.Append(GetInsertStatementColumnsList(columns));
                result.AppendLine($"{fromQueryString}[{firstColumn.SchemaName}].[{firstColumn.TableName}]");
                result.AppendLine(whereQueryString);
                result.AppendLine(GetGetByIdStatementWhere(columns));

                result.AppendLine(@"END");
            }
            return result.ToString();
        }

        private string GetSearchRawData(List<ColumnInfo> columns, string sortByColumns, string searchColumns)
        {
            var sorts = sortByColumns.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var searches = searchColumns.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var selectable = sorts.Union(searches).Distinct().ToList();
            var firstColumn = columns.First();

            var finalOrderBy = "";
            sorts.ForEach(s =>
            {
                finalOrderBy = finalOrderBy + $"[{s}]";
                if (sorts.IndexOf(s) != sorts.Count - 1)
                {
                    finalOrderBy = finalOrderBy + ", ";
                }
            });

            var finalOrderByDescending = "";
            sorts.ForEach(s =>
            {
                finalOrderByDescending = finalOrderByDescending + $"[{s}] DESC";
                if (sorts.IndexOf(s) != sorts.Count - 1)
                {
                    finalOrderByDescending = finalOrderByDescending + ", ";
                }
            });

            var finalSearch = "";
            searches.ForEach(s =>
            {
                finalSearch = finalSearch + $"([{s}] LIKE '%'+ @SEARCH +'%')";
                if (searches.IndexOf(s) != searches.Count - 1)
                {
                    finalSearch = finalSearch + " OR ";
                }
            });
            columns.ForEach(column =>
            {
                if (column.PrimaryKeyColumnPosition > 0 && !selectable.Contains(column.ColumnName))
                {
                    selectable.Add(column.ColumnName);
                }
                if ((column.IsRowVersion || column.IsCustomRowVersion) && !selectable.Contains(column.ColumnName))
                {
                    selectable.Add(column.ColumnName);
                }
            });
            var result = new StringBuilder();
            result.AppendLine("WITH RAW_DATA AS (");
            result.AppendLine("\tSELECT");
            selectable.ForEach(column =>
            {
                result.AppendLine($"\t\t[{column}],");
            });
            result.AppendLine("\t\tCASE @IS_ASCENDING WHEN 1 THEN");
            result.AppendLine($"\t\t\tROW_NUMBER() OVER(ORDER BY {finalOrderBy})");
            result.AppendLine("\t\tELSE");
            result.AppendLine($"\t\t\tROW_NUMBER() OVER(ORDER BY {finalOrderByDescending})");
            result.AppendLine("\t\tEND AS RowNumber,");
            result.AppendLine("\t\tCOUNT(1) OVER() AS TotalRows");
            result.AppendLine($"\tFROM [{firstColumn.SchemaName}].[{firstColumn.TableName}]");

            result.AppendLine($"\tWHERE {finalSearch}");
            result.AppendLine(")");
            return result.ToString();
        }

        private string GetSearchParameters()
        {
            var result = new StringBuilder();
            result.AppendLine("\t@SEARCH AS NVARCHAR(10) = '',");
            result.AppendLine("\t@PAGE_NUMBER AS INT = 1,");
            result.AppendLine("\t@PAGE_SIZE AS INT = 10,");
            result.AppendLine("\t@IS_ASCENDING AS BIT = 1");
            return result.ToString();
        }

        private string GetSearchFinalSelect()
        {
            var result = new StringBuilder();
            result.AppendLine("SELECT * ");
            result.AppendLine("FROM RAW_DATA");
            result.AppendLine("WHERE");
            result.AppendLine("\tRowNumber > @PAGE_SIZE * (@PAGE_NUMBER - 1) AND ");
            result.AppendLine("\tRowNumber <= @PAGE_SIZE * @PAGE_NUMBER");
            result.AppendLine("ORDER BY ");
            result.AppendLine("\tRowNumber");
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
            var timeStamp = columns.FirstOrDefault(c => c.IsRowVersion || c.IsCustomRowVersion);
            if (keyColumns.Any() && timeStamp != null)
            {
                keyColumns.ForEach(key =>
                {
                    var comma = ",";
                    result.AppendLine($"\t@{key.ColumnName} {key.ColumnFullType}{comma}");
                });
                result.AppendLine($"\t@{timeStamp.ColumnName} {timeStamp.ColumnFullType}");

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


        private string GetGetByIdStatementParameters(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var keyColumns = columns.Where(c => c.PrimaryKeyColumnPosition > 0).ToList();
            if (keyColumns.Any())
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
                if (column.IsCustomRowVersion)
                {
                    result.AppendLine($"\t[{column.ColumnName}] = @{column.ColumnName} + 1{comma}");
                }
                else
                {
                    result.AppendLine($"\t[{column.ColumnName}] = @{column.ColumnName}{comma}");
                }

            });
            return result.ToString();
        }

        private string GetUpdateStatementWhere(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var keyColumns = columns.Where(c => c.PrimaryKeyColumnPosition > 0).ToList();
            var timeStamp = columns.FirstOrDefault(c => c.IsRowVersion || c.IsCustomRowVersion);
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

        private string GetGetByIdStatementWhere(List<ColumnInfo> columns)
        {
            var result = new StringBuilder();
            var keyColumns = columns.Where(c => c.PrimaryKeyColumnPosition > 0).ToList();
            if (keyColumns.Any())
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
            var timeStamp = columns.FirstOrDefault(c => c.IsRowVersion || c.IsCustomRowVersion);
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
        private static List<ColumnInfo> GetFilteredColumnsListForGetById(List<ColumnInfo> columns)
        {
            return columns.Where(c => !c.IdentityColumn && !c.Computed).ToList();
        }
    }
}
