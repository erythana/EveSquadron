using System.Data;
using System.Reflection;

namespace EveSquadronKillmailImporter;

public static class EnumerableExtensions
{
    public static DataTable ToDataTable<T>(this IEnumerable<T> items)
    {
        var tb = new DataTable(typeof(T).Name);
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach(var prop in props)
            tb.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

        foreach (var item in items)
        {
            var values = new object[props.Length];
            for (var i=0; i<props.Length; i++)
                values[i] = props[i].GetValue(item, null) ?? DBNull.Value;

            tb.Rows.Add(values);
        }
        return tb;
    }
}