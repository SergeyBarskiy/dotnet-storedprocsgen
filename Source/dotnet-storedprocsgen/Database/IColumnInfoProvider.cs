using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace StoredProcsGenerator.Database
{
    public interface IColumnInfoProvider
    {
        Task<List<ColumnInfo>> GetColumns(SqlConnection connection, string tableName);
    }
}