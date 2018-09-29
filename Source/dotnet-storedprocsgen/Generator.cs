using McMaster.Extensions.CommandLineUtils;
using StoredProcsGenerator.Database;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoredProcsGenerator
{
    [Command(
           Name = "dotnet storedprocsgen",
           FullName = "dotnet-storedprocsgen",
           Description = "Generates stored procedures",
           ExtendedHelpText = "This tool generates stored procedures.  Use -s to specify server name and -d for the database name.  SSPI will be used.")]
    [HelpOption]
    public partial class Generator
    {
        public const string ProcedureKinds = "update|delete|insert|search|getbyid|getbyparentid";
        private readonly IColumnInfoProvider columnInfoProvider;
        private readonly IStoredProcedureGenerator storedProcedureGenerator;

        public Generator(
            IColumnInfoProvider columnInfoProvider,
            IStoredProcedureGenerator storedProcedureGenerator)
        {
            this.columnInfoProvider = columnInfoProvider;
            this.storedProcedureGenerator = storedProcedureGenerator;
        }

        [Required(ErrorMessage = "You must specify server name / -s or --server option")]
        [Option("-s|--server", CommandOptionType.SingleValue, Description = "Server name", ShowInHelpText = true)]
        public string Server { get; }

        [Required(ErrorMessage = "You must specify database name / -d or --database option")]
        [Option("-d|--database", CommandOptionType.SingleValue, Description = "Database name", ShowInHelpText = true)]
        public string Database { get; }

        [Required(ErrorMessage = "You must specify table name / -t or --table option")]
        [Option("-t|--table", CommandOptionType.SingleValue, Description = "Table name", ShowInHelpText = true)]
        public string Table { get; }

        [Required(ErrorMessage = "You must specify procedure kind / -k or --kind option")]
        [Option("-k|--kind", CommandOptionType.SingleValue, Description = "Procedure kind: " + ProcedureKinds, ShowInHelpText = true)]
        public string Kind { get; }

        [Option("-p|--prefix", CommandOptionType.SingleValue, Description = "Stored procedure prefix", ShowInHelpText = true)]
        public string Prefix { get; } = "usp";

        [Option("--drop", CommandOptionType.NoValue, Description = "Generate drop statement", ShowInHelpText = true)]
        public bool Drop { get; } = false;

        [Option("-r|--rowVersionColumn", CommandOptionType.SingleValue, Description = "Row version column name", ShowInHelpText = true)]
        public string RowVersionColumn { get; } = "";

        [Option("-pc|--parentColumn", CommandOptionType.SingleValue, Description = "Parent column name", ShowInHelpText = true)]
        public string ParentColumn { get; } = "";

        [Option("-e|--search_columns", CommandOptionType.SingleValue, Description = "Search columns, separated by |", ShowInHelpText = true)]
        public string SearchColumns { get; } = "";

        [Option("-o|--order_by_columns", CommandOptionType.SingleValue, Description = "Order by columns, separated by |", ShowInHelpText = true)]
        public string OrderByColumns { get; } = "";

        public async Task<int> OnExecute(CommandLineApplication app, IConsole console)
        {
            if (ProcedureKinds.Contains(Kind.ToLower()))
            {
                if (Kind.ToLower() == "search")
                {
                    if (string.IsNullOrEmpty(SearchColumns) || string.IsNullOrEmpty(OrderByColumns))
                    {
                        Console.WriteLine($"Search and order by columns options are required");
                        return Program.EXCEPTION;
                    }
                }
                Console.WriteLine($"Connecting to server {Server} to database {Database}...");
                Console.WriteLine($"... to create procedure with prefix of {Prefix} of type {Kind} for table {Table}");

                using (var connection = CreateConnection())
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected...");
                    var columns = await columnInfoProvider.GetColumns(connection, Table, RowVersionColumn, ParentColumn);
                    Console.WriteLine($"Total number of columns is {columns.Count}");
                    var kind = (StoredProcedureKind)Enum.Parse(typeof(StoredProcedureKind), Kind, true);
                    var text = new StringBuilder();
                    text.Append(storedProcedureGenerator.GetHeader(kind, Prefix, Table, columns.First().SchemaName, Drop));
                    text.Append(storedProcedureGenerator.GetBody(kind, columns, OrderByColumns, SearchColumns));
                    File.WriteAllText(FileName(kind), text.ToString());
                }

                return Program.OK;
            }
            Console.WriteLine($"Invalid procedure kind. Allowed values are {ProcedureKinds}");
            return Program.EXCEPTION;
        }

        public string CreateConnectionString()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = Server;
            builder.InitialCatalog = Database;
            builder.IntegratedSecurity = true;
            return builder.ConnectionString;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(CreateConnectionString());
        }

        public string FileName(StoredProcedureKind kind)
        {
            return Path.Combine(Environment.CurrentDirectory, storedProcedureGenerator.GetProcedureName(kind, Prefix, Table) + ".sql");
        }
    }
}