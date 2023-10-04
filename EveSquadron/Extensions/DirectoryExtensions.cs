using System;
using System.IO;

namespace EveSquadron.Extensions;

public static class DirectoryHelper
{
    public static bool HasWriteAccess(this DirectoryInfo directory, bool throwIfFails = false)
    {
        try
        {
            using var _ = File.Create(Path.Combine(directory.FullName, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose);
            return true;
        }
        catch (Exception e)
        {
            if (throwIfFails)
                throw;
            return false;
        }
    }
}