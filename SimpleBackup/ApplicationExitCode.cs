using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackup
{
    enum ApplicationExitCode
    {
        Succeeded = 0,
        UncaughtException = 1,
        InvalidCmdLineArguments = 2,
        BackupDefinitionFileNotFound = 3,
        DestinationRootFolderAlreadyExists = 4,
        ErrorCreatingDestinationRootFolder = 5,
    }
}
