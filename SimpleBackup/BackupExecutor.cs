using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SimpleBackup
{
    class BackupExecutor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string destinationRootPath;

        private readonly BackupDefinition definition;

        public BackupExecutor(string destinationRootPath, BackupDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(destinationRootPath)) throw new ArgumentNullException("destinationRootPath");
            if (definition == null) throw new ArgumentNullException("definition");

            this.destinationRootPath = destinationRootPath;
            this.definition = definition;
        }

        public int NumberOfFilesWhichFailedToCopy { get; private set; }

        public int NumberOfFoldersAccessDenied { get; private set; }

        public void ExecuteBackup()
        {
            // Reset the failure counts.
            this.NumberOfFilesWhichFailedToCopy = 0;
            this.NumberOfFoldersAccessDenied = 0;

            foreach (var folderDefinition in this.definition.FolderDefinitions)
            {
                // Create the destination folder.
                string destinationFolderPath = Path.Combine(this.destinationRootPath, folderDefinition.DestinationFolderName);
                Directory.CreateDirectory(destinationFolderPath);

                // Copy the files from the specified source into the created destination.
                CopyFolder(folderDefinition.SourceFolderPath, destinationFolderPath);
            }
        }

        public IList<BackupExecutionFailure> GetExecutionFailures()
        {
            IList<BackupExecutionFailure> result = new List<BackupExecutionFailure>();

            if (NumberOfFoldersAccessDenied > 0)
            {
                result.Add(new BackupExecutionFailure(
                    BackupExecutionFailureType.FoldersAccessDenied,
                    NumberOfFoldersAccessDenied));
            }

            if (NumberOfFilesWhichFailedToCopy > 0)
                result.Add(new BackupExecutionFailure(
                    BackupExecutionFailureType.FilesFailedToCopy,
                    NumberOfFilesWhichFailedToCopy));

            return result;
        }

        private void CopyFolder(string sourceFolderPath, string destinationFolderPath)
        {
            var sourceFolder = new DirectoryInfo(sourceFolderPath);

            // Copy all files within the source folder.
            if (!CopyAllFilesInFolder(sourceFolder, destinationFolderPath)) return;

            // Get all subfolders.
            IList<DirectoryInfo> subfolders = sourceFolder.GetDirectories();

            // For each subdirectory, recurse.
            foreach (var subfolder in subfolders)
            {
                // Compose the source folder path.
                string sourceSubfolderPath = subfolder.FullName;

                // Compose the destination folder path.
                string destinationSubfolderPath = Path.Combine(destinationFolderPath, subfolder.Name);

                // Create the destination folder.
                try
                {
                    Directory.CreateDirectory(destinationSubfolderPath);
                }
                catch (Exception ex)
                {
                    FileObjectCopyFailed(sourceSubfolderPath, ex);
                }

                // Recurse.
                CopyFolder(sourceSubfolderPath, destinationSubfolderPath);
            }
        }

        private bool CopyAllFilesInFolder(DirectoryInfo sourceFolder, string destinationFolderPath)
        {
            // Get all files in the source folder.
            var getFilesResult = GetAllFilesInFolder(sourceFolder);
            if (getFilesResult.Failed) return false;

            // Copy all files from the source to the destination.
            foreach (var file in getFilesResult.Files)
            {
                // Compose the source path.
                string sourceFilePath = file.FullName;

                // Compose the destination path.
                string destinationFilePath = Path.Combine(destinationFolderPath, file.Name);

                try
                {
                    File.Copy(sourceFilePath, destinationFilePath);
                }
                catch (Exception ex)
                {
                    FileObjectCopyFailed(sourceFilePath, ex);
                }
            }

            return true;
        }

        private GetAllFilesInFolderResult GetAllFilesInFolder(DirectoryInfo sourceFolder)
        {
            IList<FileInfo> files;
            try
            {
                files = sourceFolder.GetFiles();
            }
            catch (UnauthorizedAccessException)
            {
                // Log the failure.
                FolderAccessDenied(sourceFolder.FullName);

                // Return the failure.
                return new GetAllFilesInFolderResult(null, true);
            }

            // We succeeded, so return the result.
            return new GetAllFilesInFolderResult(files, false);
        }

        private void FolderAccessDenied(string sourceFolderPath)
        {
            // Log the failure, and increment failure count.
            logger.Info("  {0}\\ [Reason: Access denied]", sourceFolderPath);
            this.NumberOfFoldersAccessDenied++;
        }

        private void FileObjectCopyFailed(string sourceFileObjectPath, Exception ex)
        {
            // Log the failure, and increment failure count.
            logger.Info("  {0} [Reason: {1}]", sourceFileObjectPath, ex.Message);
            this.NumberOfFilesWhichFailedToCopy++;
        }

        #region Private class GetAllFilesInFolderResult
        private sealed class GetAllFilesInFolderResult
        {
            public GetAllFilesInFolderResult(IList<FileInfo> files, bool failed)
            {
                Files = files;
                Failed = failed;
            }

            public IList<FileInfo> Files { get; private set; }

            public bool Failed { get; private set; }
        }
        #endregion // #region Private class GetAllFilesInFolderResult
    }
}
