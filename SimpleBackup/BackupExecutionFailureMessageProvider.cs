using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBackup
{
    /// <summary>
    /// Responsible for providing backup failure messages for each type
    /// of failure.
    /// </summary>
    class BackupExecutionFailureMessageProvider
    {
        public BackupExecutionFailureMessageProvider()
        {
        }

        /// <summary>
        /// Gets the appropriate format string for reporting that failures
        /// of the supplied type occurred.
        /// <para>
        /// The returned format string may contain a placeholder {0}
        /// for the number of failures of this type.
        /// </para>
        /// </summary>
        /// <param name="failureType">A backup execution failure type</param>
        /// <returns>
        /// A format string appropriate for reporting that failures of the
        /// supplied type occurred. The returned string may contain a placeholder
        /// {0} for the number of failures of this type.
        /// </returns>
        public string GetFailureTypeFormatString(BackupExecutionFailureType failureType)
        {
            switch (failureType)
            {
                case BackupExecutionFailureType.FoldersAccessDenied:
                    return "Access was denied to {0} folders.";

                case BackupExecutionFailureType.FilesFailedToCopy:
                    return "{0} files failed to copy.";

                default:
                    throw new NotImplementedException(
                        $"There is no failure message defined for the {nameof(BackupExecutionFailureType)} value: {failureType}");
            }
        }
    }
}
