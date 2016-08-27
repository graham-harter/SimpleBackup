using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackup
{
    /// <summary>
    /// Enumeration of the different types of non-catastrophic failure
    /// which can occur on backup execution.
    /// </summary>
    enum BackupExecutionFailureType
    {
        /// <summary>
        /// Access was denied to a folder or folders.
        /// </summary>
        FoldersAccessDenied,
        /// <summary>
        /// A file or files failed to copy.
        /// </summary>
        FilesFailedToCopy,
    }
}
