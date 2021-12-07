using Sam.FileTableFramework.Extentions;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        private string ConnectionString { get; set; }
        public void UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public void Migrate()
         {
            this.GenerateDataBase(ConnectionString);
            this.GenerateTables(ConnectionString);
        }
    }
}
