namespace RaceLog;

/// <summary>
/// Одна запись журнала заездов. Модель не знает ни о форме, ни о SQLite.
/// </summary>
public class RaceResult
{
    /// <summary>Ключ SQLite. У новой записи равен 0 до вставки в базу.</summary>
    public long Id { get; set; }

    /// <summary>Имя пилота без пробелов по краям, не пустое.</summary>
    public string Driver { get; set; } = "";

    /// <summary>Street, Sport или Prototype.</summary>
    public string Car { get; set; } = "";

    /// <summary>Учебный овал, Городское кольцо или Спринт.</summary>
    public string Track { get; set; } = "";

    /// <summary>Время круга в целых секундах, от 30 до 600 включительно.</summary>
    public int LapSeconds { get; set; }
}
