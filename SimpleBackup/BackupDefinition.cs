using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackup
{
    sealed class BackupDefinition
    {
        public BackupDefinition(string name, string destinationRootPath, IList<BackupFolderDefinition> folderDefinitions)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(destinationRootPath)) throw new ArgumentNullException("destinationRootPath");
            if (folderDefinitions == null) throw new ArgumentNullException("folderDefinitions");

            Name = name;
            DestinationRootPath = destinationRootPath;

            // Copy the folder definitions into a read only collection.
            FolderDefinitions = new ReadOnlyCollection<BackupFolderDefinition>(folderDefinitions);
        }

        public string Name { get; private set; }

        public string DestinationRootPath { get; private set; }

        public IReadOnlyCollection<BackupFolderDefinition> FolderDefinitions { get; private set; }
    }
}
