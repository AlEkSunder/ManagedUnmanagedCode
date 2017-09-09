using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Interface;

namespace BLL.Concrete
{
    /// <summary>
    /// Provides functionality for getting the size of several files.
    /// </summary>
    /// <owner>Aleksey Beletsky</owner>
    /// <seealso cref="IDisposable" />
    public class FileSizeCollectionGetter : IDisposable
    {
        /// <summary>
        /// Holds the file size getter.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        private IFileSizeCounter fileSizeGetter;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        public void Dispose()
        {
            this.fileSizeGetter?.Dispose();
        }

        /// <summary>
        /// Gets the file size getter.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <value>
        /// The file size getter.
        /// </value>
        private IFileSizeCounter FileSizeGetter
        {
            get
            {
                if (this.fileSizeGetter == null)
                    this.fileSizeGetter = new FileSizeGetter();

                return this.fileSizeGetter;
            }
        }

        /// <summary>
        /// Gets the size of processed files.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="fileNames">The file names.</param>
        /// <returns>the size of processed files.</returns>
        public long GetFilesSize(IEnumerable<string> fileNames)
        {
            this.ValidateParam(fileNames);

            this.ProcessFiles(fileNames);

            long sizeOfFiles;

            sizeOfFiles = this.FileSizeGetter.GetSizes();

            this.FileSizeGetter.Dispose();

            return sizeOfFiles;
        }

        /// <summary>
        /// Determines is any file in the specified list of file names.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="fileNames">The file names.</param>
        /// <returns>
        ///   <c>true</c> if there is any file in the specified list of file names; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAnyFileInList(IEnumerable<string> fileNames)
        {
            return fileNames != null || fileNames.Any();
        }

        /// <summary>
        /// Processes the specified files.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="fileNames">The file names.</param>
        private void ProcessFiles(IEnumerable<string> fileNames)
        {
            foreach (var fileName in fileNames)
                this.FileSizeGetter.ProcessFile(fileName);
        }

        /// <summary>
        /// Validates the parameter.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="fileNames">The file names.</param>
        private void ValidateParam(IEnumerable<string> fileNames)
        {
            if (this.IsAnyFileInList(fileNames))
                throw new ArgumentNullException(nameof(fileNames));
        }
    }
}