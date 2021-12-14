using Sam.FileTableFramework.Data;
using Sam.FileTableFramework.Extentions;
using System.Linq;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        internal string ConnectionString { get; private set; }
        public void UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
            AddRepositories();
        }
        private void AddRepositories()
        {
            foreach (var item in GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(IRepository).FullName)))
                GetType().GetProperty(item.Name).SetValue(this, new Repository(item.Name, ConnectionString));
        }
        public void Migrate()
        {
            this.MigrateDatabase();
        }
    }
}
