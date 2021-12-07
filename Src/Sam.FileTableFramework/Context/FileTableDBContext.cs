using Sam.FileTableFramework.Extentions;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        private DatabaseOptions DatabaseOptions { get; set; }
        public void UseSqlServer(DatabaseOptions databaseOptions)
        {
            DatabaseOptions = databaseOptions;
        }
        public void Migrate()
         {
            this.GenerateDataBase(DatabaseOptions.ConnectionString);
            this.GenerateTables(DatabaseOptions.ConnectionString);
        }
    }
}
