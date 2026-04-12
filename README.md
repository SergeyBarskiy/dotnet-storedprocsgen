# Stored procedures generator for SQL Server written in .NET Core tools
You can learn more about global tools here: 
https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools

Usage: dotnet storedprocsgen [options]

Options:
  -?|-h|--help           Show help information

  -s|--server            Server name

  --database             Database name

  -t|--table             Table name

  -sc|--schema           Schema name

  -k|--kind              Procedure kind: update|delete|insert|search|getbyid

  -p|--prefix            Stored procedure prefix

  --drop                 Generate drop statement

  -r|--rowVersionColumn  Row version column name
  
  -pc|--parentColumn	 Parent column name

  -e|--search_columns    Search columns, separated by |

  -o|--order_by_columns  Order by columns, separated by |
  
  --upper                Uppercase procedure name


This tool generates stored procedures.  Use -s to specify server name and -d for the database name.
Use --schema to target a specific schema, otherwise the tool keeps using the schema returned from table metadata.
 SSPI will be used.

Example for search generation:
 dotnet storedprocsgen  -s . -d ContactManager -t Company -k search --drop -e City -o City

Example for schema-specific generation:
 dotnet storedprocsgen -s . -d ContactManager -t Company -sc sales -k getbyid


To install run 
# dotnet tool install dotnet-storedprocsgen -g
