using Sam.FileTableFramework.Enums;
using System;
using System.IO;

namespace Sam.FileTableFramework.Context.Internall
{

    internal class ChangeTrack
    {
        private ChangeTrack()
        {
        }
        public Guid? Id { get; private set; }
        public string? Name { get; private set; }
        public EntityState State { get; private set; }
        public Stream? Stream { get; private set; }

        public static ChangeTrack Create(string name, Stream stream)
        {
            return new ChangeTrack()
            {
                State = EntityState.Create,
                Name = name,
                Stream = stream
            };
        }
        public static ChangeTrack Delete(Guid id)
        {
            return new ChangeTrack()
            {
                State = EntityState.Delete,
                Id = id
            };

        }
    }
}
