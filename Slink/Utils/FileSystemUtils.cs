using System;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;

namespace Slink
{
    public static class FileSystemUtils
    {
        async public static Task<IFile> CreateIFileAtPath(string pathWithFilename)
        {
            var startingString = pathWithFilename;
            startingString = startingString.Substring(FileSystem.Current.LocalStorage.Path.Length);

            // get hold of the file system
            IFolder folder = FileSystem.Current.LocalStorage;

            //check filesystem for folder
            var array = startingString.Split('/');
            for (int i = 0; i < array.Length; i++)
            {
                var target = array[i];
                if (String.IsNullOrEmpty(target)) continue;

                if (i == array.Length - 1) break;

                // create a folder, if one does not exist already
                folder = await folder.CreateFolderAsync(array[i], CreationCollisionOption.OpenIfExists);
            }

            //write a file given the filename
            var fileName = array.Last();
            IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            return file;
        }
    }
}