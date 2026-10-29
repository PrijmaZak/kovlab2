using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Data.Sqlite;
using RaceLog;

namespace RaceLog.Checks;

/// <summary>
/// Автоматические проверки модели, поиска, SQLite, копий и отката.
/// Форма не запускается: проверяются классы, которые она использует.
/// Каждый сценарий работает со своей временной базой.
/// </summary>
internal static class Program
{
    private static int passed;
    private static readonly List<string> failures = new();

    private static int Main()
    {
        var scenarios = BuildScenarios();

        Console.WriteLine("Проверки RaceLog (ЛР 2)");
        Console.WriteLine(new string('-', 72));

        foreach ((string name, Action body) in scenarios)
        {
            try
            {
                body();
                passed++;
                Console.WriteLine($"  OK    {name}");
            }
            catch (Exception ex)
            {
                failures.Add(name);
                Console.WriteLine($"  ОШИБКА {name}");
                Console.WriteLine($"          {ex.GetType().Name}: {ex.Message}");
            }
        }

        Console.WriteLine(new string('-', 72));
        Console.WriteLine($"Пройдено {passed} из {scenarios.Count}.");

        if (failures.Count > 0)
        {
            Console.WriteLine("Не пройдены: " + string.Join("; ", failures));
            return 1;
        }

        Console.WriteLine("Все проверки пройдены.");
        return 0;
    }

    private static List<(string Name, Action Body)> BuildScenarios() => new()
    {
        ("01. Новая запись имеет значения по умолчанию", () =>
        {
            var row = new RaceResult();
            Equal(0L, row.Id);
            Equal(string.Empty, row.Driver);
            Equal(string.Empty, row.Car);
            Equal(string.Empty, row.Track);
            Equal(0, row.LapSeconds);
        }),

        ("02. Validate принимает корректную запись", () => RaceRules.Validate(Valid())),

        ("03. Validate отклоняет пустое имя", () =>
        {
            var row = Valid();
            row.Driver = "";
            Throws<ArgumentException>(() => RaceRules.Validate(row));
        }),

        ("04. Validate отклоняет имя из пробелов", () =>
        {
            var row = Valid();
            row.Driver = "   ";
            Throws<ArgumentException>(() => RaceRules.Validate(row));
        }),

        ("05. Validate отклоняет неизвестный автомобиль", () =>
        {
            var row = Valid();
            row.Car = "Truck";
            Throws<ArgumentException>(() => RaceRules.Validate(row));
        }),

        ("06. Validate отклоняет неизвестную трассу", () =>
        {
            var row = Valid();
            row.Track = "Пустыня";
            Throws<ArgumentException>(() => RaceRules.Validate(row));
        }),

        ("07. Validate принимает нижнюю границу 30", () =>
        {
            var row = Valid();
            row.LapSeconds = 30;
            RaceRules.Validate(row);
        }),

        ("08. Validate отклоняет 29", () =>
        {
            var row = Valid();
            row.LapSeconds = 29;
            Throws<ArgumentException>(() => RaceRules.Validate(row));
        }),

        ("09. Validate принимает верхнюю границу 600", () =>
        {
            var row = Valid();
            row.LapSeconds = 600;
            RaceRules.Validate(row);
        }),

        ("10. Validate отклоняет 601", () =>
        {
            var row = Valid();
            row.LapSeconds = 601;
            Throws<ArgumentException>(() => RaceRules.Validate(row));
        }),

        ("11. ValidateImport принимает шесть учебных записей",
            () => RaceRules.ValidateImport(SampleRows())),

        ("12. ValidateImport отклоняет null",
            () => Throws<ArgumentException>(() => RaceRules.ValidateImport(null!))),

        ("13. ValidateImport отклоняет повтор Id", () =>
        {
            var rows = SampleRows();
            rows[5].Id = rows[0].Id;
            Throws<ArgumentException>(() => RaceRules.ValidateImport(rows));
        }),

        ("14. ValidateImport отклоняет Id = 0", () =>
        {
            var rows = SampleRows();
            rows[2].Id = 0;
            Throws<ArgumentException>(() => RaceRules.ValidateImport(rows));
        }),

        ("15. ValidateImport отклоняет ошибку в последней записи", () =>
        {
            var rows = SampleRows();
            rows[^1].LapSeconds = 601;
            Throws<ArgumentException>(() => RaceRules.ValidateImport(rows));
        }),

        ("16. ValidateImport принимает пустой список",
            () => RaceRules.ValidateImport(new List<RaceResult>())),

        ("17. Apply без условий возвращает все записи по Id", () =>
        {
            List<RaceResult> view = RaceQuery.Apply(SampleRows(), "", RaceQuery.AllTracks);
            Equal(6, view.Count);
            Equal("1,2,3,4,5,6", string.Join(",", view.Select(r => r.Id)));
        }),

        ("18. Apply ищет без учёта регистра", () =>
        {
            Equal(2, RaceQuery.Apply(SampleRows(), "анна", RaceQuery.AllTracks).Count);
            Equal(2, RaceQuery.Apply(SampleRows(), "АННА", RaceQuery.AllTracks).Count);
        }),

        ("19. Apply объединяет поиск и трассу", () =>
        {
            List<RaceResult> view = RaceQuery.Apply(SampleRows(), "анна", "Спринт");
            Equal(1, view.Count);
            Equal(3L, view[0].Id);
        }),

        ("20. Apply возвращает пустой список без совпадений",
            () => Equal(0, RaceQuery.Apply(SampleRows(), "нет такого", RaceQuery.AllTracks).Count)),

        ("21. Apply не изменяет исходный список", () =>
        {
            List<RaceResult> all = SampleRows();
            string before = string.Join("|", all.Select(r => $"{r.Id}:{r.Driver}:{r.LapSeconds}"));
            RaceQuery.Apply(all, "анна", "Спринт");
            string after = string.Join("|", all.Select(r => $"{r.Id}:{r.Driver}:{r.LapSeconds}"));
            Equal(before, after);
        }),

        ("22. Apply сохраняет апостроф в запросе", () =>
        {
            var rows = SampleRows();
            rows.Add(new RaceResult { Id = 7, Driver = "О'Нил", Car = "Street", Track = "Спринт", LapSeconds = 90 });
            List<RaceResult> view = RaceQuery.Apply(rows, "о'н", RaceQuery.AllTracks);
            Equal(1, view.Count);
            Equal("О'Нил", view[0].Driver);
        }),

        ("23. Apply игнорирует пробелы по краям запроса",
            () => Equal(2, RaceQuery.Apply(SampleRows(), "  Анна  ", RaceQuery.AllTracks).Count)),

        ("24. Initialize создаёт пустую таблицу", () =>
        {
            using RaceRepository repository = NewRepository(out _);
            Equal(0, repository.GetAll().Count);
        }),

        ("25. Add назначает Id и сохраняет запись", () =>
        {
            using RaceRepository repository = NewRepository(out _);
            long id = repository.Add(Valid());
            Equal(1L, id);
            List<RaceResult> rows = repository.GetAll();
            Equal(1, rows.Count);
            Equal(1L, rows[0].Id);
            Equal("Анна", rows[0].Driver);
        }),

        ("26. Данные сохраняются между экземплярами репозитория", () =>
        {
            string path = NewPath();
            using (var first = new RaceRepository(path))
            {
                first.Initialize();
                first.Add(Valid());
            }

            using var second = new RaceRepository(path);
            second.Initialize();
            Equal(1, second.GetAll().Count);
        }),

        ("27. GetAll возвращает записи в порядке Id", () =>
        {
            using RaceRepository repository = NewRepository(out _);
            foreach (RaceResult row in SampleRows())
            {
                repository.Add(row);
            }

            List<RaceResult> rows = repository.GetAll();
            Equal("1,2,3,4,5,6", string.Join(",", rows.Select(r => r.Id)));
        }),

        ("28. Update меняет только выбранную запись", () =>
        {
            using RaceRepository repository = NewRepository(out _);
            repository.ReplaceAll(SampleRows());

            var changed = new RaceResult { Id = 3, Driver = "Анна", Car = "Prototype", Track = "Спринт", LapSeconds = 72 };
            repository.Update(changed);

            List<RaceResult> rows = repository.GetAll();
            Equal(72, rows.Single(r => r.Id == 3).LapSeconds);
            Equal(94, rows.Single(r => r.Id == 1).LapSeconds);
            Equal(6, rows.Count);
        }),

        ("29. Update отклоняет 601 и не меняет базу", () =>
        {
            using RaceRepository repository = NewRepository(out _);
            repository.ReplaceAll(SampleRows());

            var bad = new RaceResult { Id = 3, Driver = "Анна", Car = "Prototype", Track = "Спринт", LapSeconds = 601 };
            Throws<ArgumentException>(() => repository.Update(bad));
            Equal(73, repository.GetAll().Single(r => r.Id == 3).LapSeconds);
        }),

        ("30. Delete удаляет ровно выбранную запись", () =>
        {
            using RaceRepository repository = NewRepository(out _);
            repository.ReplaceAll(SampleRows());

            repository.Delete(2);

            List<RaceResult> rows = repository.GetAll();
            Equal(5, rows.Count);
            Equal("1,3,4,5,6", string.Join(",", rows.Select(r => r.Id)));
        }),

        ("31. Delete несуществующего Id сообщает об ошибке", () =>
        {
            using RaceRepository repository = NewRepository(out _);
            repository.ReplaceAll(SampleRows());
            Throws<InvalidOperationException>(() => repository.Delete(999));
        }),

        ("32. JSON: сохранение и чтение возвращают те же объекты", () =>
        {
            string path = TempFile("json");
            RaceTransfer.SaveJson(path, SampleRows());
            List<RaceResult> loaded = RaceTransfer.LoadJson(path);
            SameRows(SampleRows(), loaded);
        }),

        ("33. XML: сохранение и чтение возвращают те же объекты", () =>
        {
            string path = TempFile("xml");
            RaceTransfer.SaveXml(path, SampleRows());
            List<RaceResult> loaded = RaceTransfer.LoadXml(path);
            SameRows(SampleRows(), loaded);
        }),

        ("34. ReplaceAll заменяет журнал без дублей", () =>
        {
            using RaceRepository repository = NewRepository(out _);
            repository.ReplaceAll(SampleRows());
            repository.ReplaceAll(SampleRows());
            Equal(6, repository.GetAll().Count);
        }),

        ("35. ReplaceAll при ошибке вставки сохраняет прежние данные", () =>
        {
            string path = NewPath();
            using RaceRepository repository = new(path);
            repository.Initialize();
            repository.ReplaceAll(SampleRows());

            using (var connection = new SqliteConnection($"Data Source={path}"))
            {
                connection.Open();
                using SqliteCommand trigger = connection.CreateCommand();
                trigger.CommandText = @"
CREATE TRIGGER trg_fail BEFORE INSERT ON RaceResults
WHEN NEW.Id = 999
BEGIN
    SELECT RAISE(ABORT, 'проверка отката');
END;";
                trigger.ExecuteNonQuery();
            }

            var bad = SampleRows();
            bad.Add(new RaceResult { Id = 999, Driver = "Тест", Car = "Street", Track = "Спринт", LapSeconds = 60 });

            Throws<SqliteException>(() => repository.ReplaceAll(bad));
            Equal(6, repository.GetAll().Count);
        }),

        ("36. LoadJson отклоняет повреждённый файл", () =>
        {
            string path = TempFile("json");
            File.WriteAllText(path, "[ { \"Id\": 1, \"Driver\": ");
            Throws<System.Text.Json.JsonException>(() => RaceTransfer.LoadJson(path));
        }),

        ("37. LoadXml запрещает DTD", () =>
        {
            string path = TempFile("xml");
            File.WriteAllText(path, @"<?xml version=""1.0""?>
<!DOCTYPE foo [ <!ENTITY xxe SYSTEM ""file:///c:/windows/win.ini""> ]>
<ArrayOfRaceResult><RaceResult><Id>1</Id><Driver>&xxe;</Driver></RaceResult></ArrayOfRaceResult>");
            try
            {
                RaceTransfer.LoadXml(path);
            }
            catch (Exception ex) when (ex is XmlException || ex.InnerException is XmlException)
            {
                return;
            }

            throw new InvalidOperationException("Чтение XML с DTD не было отклонено.");
        })
    };

    // ---------- учебные данные ----------

    private static RaceResult Valid() => new()
    {
        Driver = "Анна",
        Car = "Street",
        Track = "Учебный овал",
        LapSeconds = 94
    };

    private static List<RaceResult> SampleRows() => new()
    {
        new RaceResult { Id = 1, Driver = "Анна", Car = "Street", Track = "Учебный овал", LapSeconds = 94 },
        new RaceResult { Id = 2, Driver = "Борис", Car = "Sport", Track = "Городское кольцо", LapSeconds = 126 },
        new RaceResult { Id = 3, Driver = "Анна", Car = "Prototype", Track = "Спринт", LapSeconds = 73 },
        new RaceResult { Id = 4, Driver = "Вера", Car = "Street", Track = "Учебный овал", LapSeconds = 101 },
        new RaceResult { Id = 5, Driver = "Глеб", Car = "Sport", Track = "Спринт", LapSeconds = 80 },
        new RaceResult { Id = 6, Driver = "Дина", Car = "Prototype", Track = "Городское кольцо", LapSeconds = 118 }
    };

    // ---------- вспомогательные методы ----------

    private static string NewPath()
    {
        string folder = Path.Combine(Path.GetTempPath(), "racelog-checks");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, Guid.NewGuid().ToString("N") + ".db");
    }

    private static RaceRepository NewRepository(out string path)
    {
        path = NewPath();
        var repository = new RaceRepository(path);
        repository.Initialize();
        return repository;
    }

    private static string TempFile(string extension)
    {
        string folder = Path.Combine(Path.GetTempPath(), "racelog-checks");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, Guid.NewGuid().ToString("N") + "." + extension);
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"Ожидалось «{expected}», получено «{actual}».");
        }
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try
        {
            action();
        }
        catch (T)
        {
            return;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Ожидалось исключение {typeof(T).Name}, получено {ex.GetType().Name}: {ex.Message}");
        }

        throw new InvalidOperationException($"Ожидалось исключение {typeof(T).Name}, но его не было.");
    }

    private static void SameRows(List<RaceResult> expected, List<RaceResult> actual)
    {
        Equal(expected.Count, actual.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Equal(expected[i].Id, actual[i].Id);
            Equal(expected[i].Driver, actual[i].Driver);
            Equal(expected[i].Car, actual[i].Car);
            Equal(expected[i].Track, actual[i].Track);
            Equal(expected[i].LapSeconds, actual[i].LapSeconds);
        }
    }
}
