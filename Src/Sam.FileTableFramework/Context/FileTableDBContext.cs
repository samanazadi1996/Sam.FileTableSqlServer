using Sam.FileTableFramework.Extentions;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        internal string ConnectionString { get; private set; }
        public void UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public void Migrate()
        {
            this.MigrateDatabase();
        }
    }
}
