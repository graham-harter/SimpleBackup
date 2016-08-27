using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackup
{
    /// <summary>
    /// Holds information about a particular kind of non-catastrophic failure
    /// which occurred during backup execution.
    /// </summary>
    class BackupExecutionFailure
    {
        public BackupExecutionFailure(BackupExecutionFailureType type, int numberOfFailures)
        {
            Type = type;
            NumberOfFailures = numberOfFailures;
        }

        public BackupExecutionFailureType Type { get; private set; }

        public int NumberOfFailures { get; private set; }
    }
}
