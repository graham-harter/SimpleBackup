using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackup
{
    sealed class BackupFolderDefinition
    {
        public BackupFolderDefinition(string sourceFolderPath, string destinationFolderName)
        {
            if (string.IsNullOrWhiteSpace(sourceFolderPath)) throw new ArgumentNullException("sourceFolderPath");
            if (string.IsNullOrWhiteSpace(destinationFolderName)) throw new ArgumentNullException("destinationFolderName");

            SourceFolderPath = sourceFolderPath;
            DestinationFolderName = destinationFolderName;
        }

        public string SourceFolderPath { get; private set; }

        public string DestinationFolderName { get; private set; }
    }
}
