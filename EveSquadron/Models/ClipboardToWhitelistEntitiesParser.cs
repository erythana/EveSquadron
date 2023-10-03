using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EveSquadron.Models.Enums;
using EveSquadron.Models.Interfaces;

namespace EveSquadron.Models;

public class ClipboardToWhitelistEntitiesParser<T> : IClipboardToWhitelistEntitiesParser<T> where T : IWhitelistEntry, new()
{
    public IEnumerable<T> Parse(string clipboardContent)
    {
        var results = new List<T>();
        
        var clipboardSplit = Regex.Split(clipboardContent, "\r?\n") //This replaces Environment.NewLine and splits the newlines  because, for some reason, Windows used \n on a users system
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct();
        
        foreach (var entity in clipboardSplit)
        {
            var value = entity.Split(new[] { ',' }, 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var entityType = EntityTypeEnum.Character;
            if (value.Length > 1 && Enum.TryParse(value[0], out EntityTypeEnum result)) // separated by delimiter, need to parse type
                entityType = result;
            
            results.Add(new T{Type = entityType, Name = value.Last()});
        }

        return results;
    }
}