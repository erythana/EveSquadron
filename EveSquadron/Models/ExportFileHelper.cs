using System.IO;
using EveSquadron.Extensions;

namespace EveSquadron.Models;

public static class ExportFileHelper
{
    public static bool IsValidExportFile(string file)
    {
        var fileInfo = new FileInfo(file);
        return fileInfo.Directory?.HasWriteAccess(throwIfFails: true) == true && fileInfo.Extension == ".csv";
    }
}