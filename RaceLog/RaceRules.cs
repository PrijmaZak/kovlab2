using System;
using System.Collections.Generic;

namespace RaceLog;

/// <summary>
/// Единые правила учебного задания: границы времени, допустимые варианты
/// автомобиля и трассы, проверка одной записи и проверка всей копии журнала.
/// </summary>
public static class RaceRules
{
    public const int MinLapSeconds = 30;
    public const int MaxLapSeconds = 600;

    public static readonly string[] Cars = { "Street", "Sport", "Prototype" };
    public static readonly string[] Tracks = { "Учебный овал", "Городское кольцо", "Спринт" };

    /// <summary>Проверяет одну запись и выбрасывает ArgumentException с понятной причиной.</summary>
    public static void Validate(RaceResult row)
    {
        if (row is null)
        {
            throw new ArgumentException("Запись не создана.");
        }

        string driver = (row.Driver ?? string.Empty).Trim();
        if (driver.Length == 0)
        {
            throw new ArgumentException("Укажите имя пилота.");
        }

        if (Array.IndexOf(Cars, row.Car) < 0)
        {
            throw new ArgumentException("Выберите автомобиль: Street, Sport или Prototype.");
        }

        if (Array.IndexOf(Tracks, row.Track) < 0)
        {
            throw new ArgumentException("Выберите трассу: Учебный овал, Городское кольцо или Спринт.");
        }

        if (row.LapSeconds < MinLapSeconds || row.LapSeconds > MaxLapSeconds)
        {
            throw new ArgumentException(
                $"Время должно быть целым числом от {MinLapSeconds} до {MaxLapSeconds} секунд.");
        }
    }

    /// <summary>
    /// Проверяет всю восстанавливаемую копию: каждая запись корректна, Id положителен
    /// и не повторяется. Ошибка в любой записи отклоняет весь импорт.
    /// </summary>
    public static void ValidateImport(List<RaceResult> rows)
    {
        if (rows is null)
        {
            throw new ArgumentException("Копия не содержит списка записей.");
        }

        var seen = new List<long>();
        foreach (RaceResult row in rows)
        {
            if (row is null)
            {
                throw new ArgumentException("Копия содержит пустую запись.");
            }

            if (row.Id <= 0)
            {
                throw new ArgumentException("В копии найден неположительный Id.");
            }

            if (seen.Contains(row.Id))
            {
                throw new ArgumentException($"В копии повторяется Id {row.Id}.");
            }

            Validate(row);
            seen.Add(row.Id);
        }
    }
}
