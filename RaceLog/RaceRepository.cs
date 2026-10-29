using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace RaceLog;

/// <summary>
/// Всё общение с SQLite. SQL находится здесь, а не в обработчиках формы.
/// Каждый метод открывает короткое соединение и освобождает его через using.
/// </summary>
public sealed class RaceRepository : IDisposable
{
    private readonly string connectionString;

    public RaceRepository(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath))
        {
            throw new ArgumentException("Не указан путь к базе данных.", nameof(databasePath));
        }

        DatabasePath = System.IO.Path.GetFullPath(databasePath);
        connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true
        }.ToString();
    }

    public string DatabasePath { get; }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>Создаёт таблицу при первом запуске. Существующие данные не трогает.</summary>
    public void Initialize()
    {
        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS RaceResults (
    Id INTEGER PRIMARY KEY,
    Driver TEXT NOT NULL,
    Car TEXT NOT NULL,
    Track TEXT NOT NULL,
    LapSeconds INTEGER NOT NULL
        CHECK(LapSeconds BETWEEN 30 AND 600)
);";
        command.ExecuteNonQuery();
    }

    /// <summary>Читает весь журнал в порядке Id.</summary>
    public List<RaceResult> GetAll()
    {
        var rows = new List<RaceResult>();

        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Driver, Car, Track, LapSeconds FROM RaceResults ORDER BY Id;";

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new RaceResult
            {
                Id = reader.GetInt64(0),
                Driver = reader.GetString(1),
                Car = reader.GetString(2),
                Track = reader.GetString(3),
                LapSeconds = reader.GetInt32(4)
            });
        }

        return rows;
    }

    /// <summary>Вставляет новую запись и возвращает назначенный базой Id.</summary>
    public long Add(RaceResult row)
    {
        ArgumentNullException.ThrowIfNull(row);
        row.Driver = row.Driver.Trim();
        RaceRules.Validate(row);

        using SqliteConnection connection = Open();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using SqliteCommand insert = connection.CreateCommand();
        insert.Transaction = transaction;
        insert.CommandText = @"
INSERT INTO RaceResults (Driver, Car, Track, LapSeconds)
VALUES ($driver, $car, $track, $seconds);";
        insert.Parameters.AddWithValue("$driver", row.Driver);
        insert.Parameters.AddWithValue("$car", row.Car);
        insert.Parameters.AddWithValue("$track", row.Track);
        insert.Parameters.AddWithValue("$seconds", row.LapSeconds);
        insert.ExecuteNonQuery();

        using SqliteCommand lastId = connection.CreateCommand();
        lastId.Transaction = transaction;
        lastId.CommandText = "SELECT last_insert_rowid();";
        long id = (long)(lastId.ExecuteScalar() ?? 0L);

        transaction.Commit();
        return id;
    }

    /// <summary>Меняет запись по Id. Должна быть затронута ровно одна строка.</summary>
    public void Update(RaceResult row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (row.Id <= 0)
        {
            throw new ArgumentException("Для изменения нужен положительный Id записи.");
        }

        row.Driver = row.Driver.Trim();
        RaceRules.Validate(row);

        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"
UPDATE RaceResults
SET Driver = $driver, Car = $car, Track = $track, LapSeconds = $seconds
WHERE Id = $id;";
        command.Parameters.AddWithValue("$driver", row.Driver);
        command.Parameters.AddWithValue("$car", row.Car);
        command.Parameters.AddWithValue("$track", row.Track);
        command.Parameters.AddWithValue("$seconds", row.LapSeconds);
        command.Parameters.AddWithValue("$id", row.Id);

        int affected = command.ExecuteNonQuery();
        if (affected != 1)
        {
            throw new InvalidOperationException($"Запись с Id {row.Id} не найдена.");
        }
    }

    /// <summary>Удаляет одну запись по Id.</summary>
    public void Delete(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Для удаления нужен положительный Id записи.");
        }

        using SqliteConnection connection = Open();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM RaceResults WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);

        int affected = command.ExecuteNonQuery();
        if (affected != 1)
        {
            throw new InvalidOperationException($"Запись с Id {id} не найдена.");
        }
    }

    /// <summary>
    /// Заменяет весь журнал одной транзакцией: удаление прежних строк и вставка новых
    /// с сохранением Id. При любой ошибке транзакция откатывается целиком.
    /// </summary>
    public void ReplaceAll(List<RaceResult> rows)
    {
        RaceRules.ValidateImport(rows);

        using SqliteConnection connection = Open();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using (SqliteCommand delete = connection.CreateCommand())
        {
            delete.Transaction = transaction;
            delete.CommandText = "DELETE FROM RaceResults;";
            delete.ExecuteNonQuery();
        }

        using (SqliteCommand insert = connection.CreateCommand())
        {
            insert.Transaction = transaction;
            insert.CommandText = @"
INSERT INTO RaceResults (Id, Driver, Car, Track, LapSeconds)
VALUES ($id, $driver, $car, $track, $seconds);";

            SqliteParameter id = insert.Parameters.Add("$id", SqliteType.Integer);
            SqliteParameter driver = insert.Parameters.Add("$driver", SqliteType.Text);
            SqliteParameter car = insert.Parameters.Add("$car", SqliteType.Text);
            SqliteParameter track = insert.Parameters.Add("$track", SqliteType.Text);
            SqliteParameter seconds = insert.Parameters.Add("$seconds", SqliteType.Integer);

            foreach (RaceResult row in rows)
            {
                id.Value = row.Id;
                driver.Value = row.Driver.Trim();
                car.Value = row.Car;
                track.Value = row.Track;
                seconds.Value = row.LapSeconds;
                insert.ExecuteNonQuery();
            }
        }

        transaction.Commit();
    }

    public void Dispose()
    {
        // Соединения открываются на время вызова, поэтому отдельных ресурсов нет.
        SqliteConnection.ClearAllPools();
    }
}
