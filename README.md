# dotnet-storedprocsgen
Dotnet command line tool for stored procedures generation. 

Usage: dotnet storedprocsgen [options]

Options:
  -?|-h|--help           Show help information

  -s|--server            Server name

  -d|--database          Database name

  -t|--table             Table name

  -k|--kind              Procedure kind: update|delete|insert

  -p|--prefix            Stored procedure prefix

  --drop                 Generate drop statement

  -r|--rowVersionColumn  Row version custom column name.  Assumes numeric

This tool generates stored procedures.  Use -s to specify server name and -d for the database name.
 SSPI will be used.

To install run 
# dotnet tool install dotnet-storedprocsgen -g
