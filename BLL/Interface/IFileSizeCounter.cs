using System;

namespace BLL.Interface
{
    /// <summary>
    /// Defines interface for getting the file size.
    /// </summary>
    /// <owner>Aleksey Beletsky</owner>
    /// <seealso cref="IDisposable" />
    public interface IFileSizeCounter : IDisposable
    {
        /// <summary>
        /// Gets the size of processed files.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <returns>The size of processed files.</returns>
        long GetSizes();

        /// <summary>
        /// Processes the specified file.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="fileName">The filename.</param>
        void ProcessFile(string fileName);
    }
}