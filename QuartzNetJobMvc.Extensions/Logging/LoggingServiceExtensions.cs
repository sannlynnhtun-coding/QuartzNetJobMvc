namespace QuartzNetJobMvc.Extensions.Logging;

public static class LoggingServiceExtensions
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        // Configure MSSqlServerSinkOptions
        var sinkOptions = new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true
        };

        // Define optional column options
        //var columnOptions = new ColumnOptions
        //{
        //    AdditionalColumns = new[]
        //    {
        //        new SqlColumn { ColumnName = "CustomColumn1", DataType = SqlDbType.NVarChar, DataLength = 100 }
        //    }
        //};

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.MSSqlServer(
                connectionString: builder.Configuration.GetConnectionString("DbConnection"),
                sinkOptions: sinkOptions
                //columnOptions: columnOptions
            )
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}