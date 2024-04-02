using Npgsql;

namespace Discount.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
    {
        int retryForAvailability = retry.Value;

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();

            try
            {
                logger.LogInformation("Migration postgres database");

                using var connection =
                    new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                connection.Open();

                using var command = new NpgsqlCommand
                {
                    Connection = connection,
                };

                var resetCouponTable = configuration.GetValue<bool>("DatabaseSettings:ResetCouponTable");
                if (resetCouponTable)
                {
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();
                }
                
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Coupon (
	                                        ID SERIAL PRIMARY KEY NOT NULL,
	                                        ProductName VARCHAR(24) NOT NULL,
	                                        Description TEXT,
	                                        Amount INT
                                        );";
                command.ExecuteNonQuery();
                
                command.CommandText = @"INSERT INTO Coupon (ProductName, Description, Amount)
                                        SELECT 'IPhone X', 'IPhone Discount', 150
                                        WHERE NOT EXISTS (
	                                        SELECT 1
	                                        FROM Coupon
	                                        WHERE ProductName = 'IPhone X' 
		                                        AND Description = 'IPhone Discount'
	                                            AND Amount = 150
                                        );";
                command.ExecuteNonQuery();
                
                command.CommandText = @"INSERT INTO Coupon (ProductName, Description, Amount)
                                        SELECT 'Samsung 10', 'Samsung Discount', 100
                                        WHERE NOT EXISTS (
	                                        SELECT 1
	                                        FROM Coupon
	                                        WHERE ProductName = 'Samsung 10'
		                                        AND Description = 'Samsung Discount'
	                                            AND Amount = 100
                                        );";
                command.ExecuteNonQuery();
                
                logger.LogInformation("Migration postgres database done");
            }
            catch (NpgsqlException e)
            {
                logger.LogError(
                        e, 
                        "An error occurred while migrating the postresql database"
                    );

                if (retryForAvailability < 50)
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    MigrateDatabase<TContext>(host, retryForAvailability);
                }
            }
            catch (Exception e)
            {
               logger.LogError(
                        e,
                        e.Message
                    ); 
               throw;
            }
        }

        return host;
    }
}