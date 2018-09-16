using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.IO;

namespace StoredProcsGeneratorTests
{
    [TestClass]
    public class GeneratorTests
    {
        [TestMethod]
        public void PersonUpdateScriptTest()
        {
            /*
                IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'usp_test_Person_Update')
                BEGIN
	                DROP PROCEDURE [dbo].[usp_test_Person_Update]
                END
                GO
                CREATE PROCEDURE [dbo].[usp_test_Person_Update]
                (
	                @PersonId INT,
	                @FirstName NVARCHAR(100),
	                @LastName NVARCHAR(100),
	                @BirthDate DATE,
	                @RowVersion TIMESTAMP
                )
                AS
                UPDATE [dbo].[Person] SET
	                [FirstName] = @FirstName,
	                [LastName] = @LastName,
	                [BirthDate] = @BirthDate
                OUTPUT inserted.[PersonId], inserted.[RowVersion]
                WHERE
	                [PersonId] = @PersonId AND 
	                [RowVersion]= @RowVersion
             */

            using(var connection = CreateConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = File.ReadAllText(FileName("usp_test_Person_Update.sql"));
                command.CommandType = System.Data.CommandType.Text;
                using (var reader = command.ExecuteReader())
                {

                }
            }
        }

        public string CreateConnectionString()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = "NIMISHA-PC";
            builder.InitialCatalog = "CodeGenTestDb";
            builder.IntegratedSecurity = true;
            return builder.ConnectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(CreateConnectionString());
        }

        public string FileName(string fileName)
        {
            return Path.Combine(Environment.CurrentDirectory, "GeneratedProcs", fileName);
        }
    }
}
