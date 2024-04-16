using System.Linq;

namespace Sam.FileTableFramework.Context
{
    public abstract class FileTableDBContext
    {
        internal string? ConnectionString { get; private set; }
        public void UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
            AddRepositories();
        }
        private void AddRepositories()
        {
            foreach (var item in GetType().GetProperties().Where(p => p.PropertyType.FullName.Equals(typeof(FtDbSet).FullName)))
                GetType().GetProperty(item.Name).SetValue(this, new FtDbSet(item.Name, ConnectionString!));
        }
    }
}
