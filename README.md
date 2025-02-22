## Get started

1. Restore dependencies

   ```bash
   dotnet restore
   ```

2. Insert required environment variables to connect to database

   ```bash
   DB_CONNECTION="Host=your_host;Port=5432;Database=your_db;Username=your_user;Password=your_password" #PostgreSQL
   D_CONNECTION="Data Source=localdatabase.db" #Sqlite
   ADDRESS="http://0.0.0.0:8080" #Use actual website address if deploying online
   ```

3. OPTIONAL: Replace the PostgreSQL with a Sqlite local connection

  Replace: 
   ```bash
   builder.Services.AddDbContext<StoreContext>(options => 
   {
        var dbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION");
    
        if (string.IsNullOrEmpty(dbConnection))
        {
            throw new InvalidOperationException("FOR ADMIN: Please config ENV file! Connection missing!");
        }
    
        options.UseNpgsql(dbConnection);
   });
   ```

  With:
   ```bash
  builder.Services.AddDbContext<StoreContext>(options => 
  {
      options.UseSqlite(builder.Configuration.GetConnectionString("D-Connection"));
  });
   ```

4. Run database migration

   ```bash
   dotnet ef migrations add InitialCreate -o Data/Migrations
   ```

5. Update database

   ```bash
   dotnet ef database update
   ```

6. Run the app

   ```bash
   dotnet run 
   ```

