using System;
using System.Data.SqlClient;
using System.IO;

namespace StoredProcsGeneratorTests
{
    public class BaseTests
    {

        protected int ExecuteDdlStatement(string sqlText)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = sqlText;
                command.CommandType = System.Data.CommandType.Text;
                return command.ExecuteNonQuery();
            }
        }

        protected string CreateConnectionString()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = "NIMISHA-PC";
            builder.InitialCatalog = "CodeGenTestDb";
            builder.IntegratedSecurity = true;
            return builder.ConnectionString;
        }

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(CreateConnectionString());
        }

        protected string FileName(string fileName)
        {
            return Path.Combine(Environment.CurrentDirectory, "GeneratedProcs", fileName);
        }

        protected string GetFileText(string fileName)
        {
            return File.ReadAllText(FileName(fileName));
        }

        protected string GetSqlScriptText(string procedureKind, string table)
        {
            var fileName = "usp_test_" + table + "_";
            switch (procedureKind)
            {
                case "update":
                    fileName += "Update";
                    break;
                case "insert":
                    fileName += "Insert";
                    break;
                case "delete":
                    fileName += "Delete";
                    break;
                case "search":
                    fileName += "Search";
                    break;
            }
            fileName += ".sql";
            return GetFileText(fileName);
        }
    }
}
