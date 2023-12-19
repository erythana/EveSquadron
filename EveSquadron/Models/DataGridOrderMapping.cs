namespace EveSquadron.Models;

public class DataGridOrderMapping
{
    public DataGridOrderMapping(int index, int displayIndex)
    {
        Index = index;
        DisplayIndex = displayIndex;
    }
    
    public int Index { get; }
    public int DisplayIndex { get; }
}