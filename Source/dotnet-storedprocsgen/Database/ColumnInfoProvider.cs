using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StoredProcsGenerator.Database
{
    public class ColumnInfoProvider : IColumnInfoProvider
    {
        public async Task<List<ColumnInfo>> GetColumns(SqlConnection connection, string tableName, string rowVersionColumn, string parentColumn = null)
        {
            var properties = typeof(ColumnInfo).GetProperties().Where(c => !c.Name.ToUpper().Contains("CUSTOM") && !c.Name.ToUpper().Contains("PARENT")).ToList();
            var result = new List<ColumnInfo>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = ColumnsSelectStatement;
                command.Parameters.AddWithValue("@tableName", tableName);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var column = new ColumnInfo();
                        properties.ForEach(prop =>
                        {
                            prop.SetValue(column, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                        });
                        result.Add(column);
                    };
                }
            }
            if (!string.IsNullOrEmpty(rowVersionColumn))
            {
                var column = result.FirstOrDefault(c => c.ColumnName == rowVersionColumn);
                if (column != null)
                {
                    column.IsCustomRowVersion = true;
                }
            }

            if (!string.IsNullOrEmpty(parentColumn))
            {
                var column = result.FirstOrDefault(c => c.ColumnName == parentColumn);
                if (column != null)
                {
                    column.IsParentColumn = true;
                }
            }
            return result;
        }

        public const string ColumnsSelectStatement = @"
Select 
	sys.tables.name As TableName, 
	sys.columns.name As ColumnName, 
	sys.schemas.name As SchemaName,
	sys.types.name As TypeName,
	sys.columns.max_length As MaximumLength,
	sys.columns.precision As ColumnPrecision,
	sys.columns.scale As ColumnScale,
	sys.columns.column_id As OrdinalPosition,
	Upper(sys.types.name) + 
	Case
		When sys.types.name LIKE '%TEXT' OR sys.types.name IN ('IMAGE', 'SQL_VARIANT' ,'XML') Then ''
		When sys.types.name LIKE '%CHAR' AND sys.columns.max_length = -1 Then '(MAX)'
		When sys.types.name LIKE '%CHAR' AND sys.columns.max_length > -1 Then '(' + CAST(sys.columns.max_length As nvarchar(11)) + ')'
		When sys.types.name LIKE '%BINARY' AND sys.columns.max_length = -1 Then '(MAX)'
		When sys.types.name LIKE '%BINARY' AND sys.columns.max_length > -1 Then  '(' + CAST(sys.columns.max_length As nvarchar(11)) + ')'
		When sys.types.name IN ('DECIMAL', 'NUMERIC') Then  '(' + CAST(sys.columns.precision AS nvarchar(11)) + ',' + CAST(sys.columns.scale As nvarchar(11)) + ')'
		When sys.types.name IN ('FLOAT') Then '(' + CAST(COALESCE(sys.columns.precision, 18) AS varchar(11)) + ')'
		When sys.types.name IN ('DATETIME2', 'DATETIMEOFFSET', 'TIME') Then  '(' + CAST(COALESCE(sys.columns.scale, 7) AS varchar(11)) + ')'
		Else ''
	End AS ColumnFullType,
	sys.columns.is_computed as Computed,
	sys.columns.is_identity as IdentityColumn,
	sys.columns.is_nullable as Nullable,
	Coalesce((Select sys.index_columns.key_ordinal
		From sys.index_columns 
		Inner Join sys.indexes on sys.indexes.object_id = sys.index_columns.object_id and sys.indexes.index_id = sys.index_columns.index_id 
		Where sys.index_columns.object_id = sys.columns.object_id and sys.index_columns.column_id = sys.columns.column_id and sys.indexes.is_primary_key=1), 0) As PrimaryKeyColumnPosition,
	Case When sys.types.name = 'timestamp' Then Cast(1 as Bit) Else Cast(0 as Bit) End As IsRowVersion
From sys.columns
Inner Join sys.tables On sys.tables.object_id = sys.columns.object_id
Inner Join sys.schemas On sys.schemas.schema_id = sys.tables.schema_id
Inner Join sys.types On sys.types.user_type_id = sys.columns.user_type_id
Where sys.tables.name=@tableName
Order By sys.columns.column_id
";
    }
}
