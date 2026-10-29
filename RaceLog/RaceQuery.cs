using System;
using System.Collections.Generic;
using System.Linq;

namespace RaceLog;

/// <summary>
/// Поиск и фильтр по журналу. Метод не меняет исходный список, а возвращает новый.
/// </summary>
public static class RaceQuery
{
    public const string AllTracks = "Все трассы";

    /// <summary>
    /// Оставляет записи, имя которых содержит запрос без учёта регистра,
    /// и записи выбранной трассы. Пустой запрос и вариант «Все трассы» ничего не отсекают.
    /// </summary>
    public static List<RaceResult> Apply(List<RaceResult> all, string? text, string? track)
    {
        if (all is null)
        {
            throw new ArgumentNullException(nameof(all));
        }

        string query = (text ?? string.Empty).Trim();
        string trackFilter = track ?? AllTracks;

        IEnumerable<RaceResult> rows = all.Where(r => r is not null);

        if (query.Length > 0)
        {
            rows = rows.Where(r => r.Driver.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        if (trackFilter.Length > 0 && trackFilter != AllTracks)
        {
            rows = rows.Where(r => string.Equals(r.Track, trackFilter, StringComparison.Ordinal));
        }

        return rows.OrderBy(r => r.Id).ToList();
    }
}
