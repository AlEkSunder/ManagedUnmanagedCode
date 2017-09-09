using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BLL.Interface;

namespace BLL.Concrete
{
    /// <summary>
    /// Provides functionality for getting the file size through the WIN32 API.
    /// </summary>
    /// <owner>Aleksey Beletsky</owner>
    /// <seealso cref="IFileSizeCounter" />
    public class FileSizeGetter : IFileSizeCounter
    {
        /// <summary>
        /// Stores the value which shows whether Dispose has been called.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        private bool disposed = false;

        /// <summary>
        /// Stores the pointer to an external unmanaged resource.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        private IntPtr handle;

        /// <summary>
        /// Stores the file names and sizes as internal managed resource.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        private IList<Tuple<string, long>> sizePerFiles = new List<Tuple<string, long>>();

        /// <summary>
        /// Finalizes an instance of the <see cref="FileSizeGetter"/> class.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        ~FileSizeGetter()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> 
        /// to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
                this.sizePerFiles = null;

            FileSizeGetter.CloseHandle(this.handle);
            this.handle = IntPtr.Zero;

            this.disposed = true;
        }

        /// <summary>
        /// Closes the handle.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="handle">The handle.</param>
        /// <returns>True if handle was closed, otherwise - false.</returns>
        [DllImport("Kernel32")]
        private extern static bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Provides external functionality to get the pointer for file.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="lpFileName">Name of the file.</param>
        /// <param name="dwDesiredAccess">The desired access.</param>
        /// <param name="dwShareMode">The share mode.</param>
        /// <param name="lpSecurityAttributes">The security attributes.</param>
        /// <param name="dwCreationDisposition">The creation disposition.</param>
        /// <param name="dwFlagsAndAttributes">The flags and attributes.</param>
        /// <param name="hTemplateFile">The template file.</param>
        /// <returns>The handle for file opening.</returns>
        [DllImport("Kernel32", SetLastError = true)]
        private extern static IntPtr CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode,
            IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

        /// <summary>
        /// Provides external functionality to get the file size.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="hFile">The handle for the file.</param>
        /// <param name="lpFileSize">Size of the specified file.</param>
        /// <returns>True if file size was got, otherwise - false.</returns>
        [DllImport("Kernel32")]
        private extern static bool GetFileSizeEx(IntPtr hFile, out long lpFileSize);

        /// <summary>
        /// Gets the size of processed files.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <returns>The size of processed files.</returns>
        public long GetSizes()
        {
            return this.sizePerFiles.Sum(sizePerFile => sizePerFile.Item2);
        }

        /// <summary>
        /// Initializes the handle for create file.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="fileName">The file name.</param>
        private void InitializeHandleForCreateFile(string fileName)
        {
            this.handle = FileSizeGetter.CreateFile(
                fileName,
                (uint)FileAccess.Read,
                (uint)FileShare.Read,
                IntPtr.Zero,
                (uint)FileMode.Open,
                (uint)FileAttributes.ReadOnly,
                IntPtr.Zero);
        }

        /// <summary>
        /// Processes the specified file.
        /// </summary>
        /// <owner>Aleksey Beletsky</owner>
        /// <param name="fileName">The filename.</param>
        public void ProcessFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            this.InitializeHandleForCreateFile(fileName);
            long fileSize;
            FileSizeGetter.GetFileSizeEx(handle, out fileSize);
            this.sizePerFiles.Add(new Tuple<string, long>(fileName, fileSize));
        }
    }
}