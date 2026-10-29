using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace RaceLog;

/// <summary>
/// Копии журнала в JSON и XML. Класс не обращается к форме и не выполняет SQL.
/// </summary>
public static class RaceTransfer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        // Кириллица остаётся читаемой в текстовом файле.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static void SaveJson(string path, List<RaceResult> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        string json = JsonSerializer.Serialize(rows, JsonOptions);
        File.WriteAllText(path, json);
    }

    public static void SaveXml(string path, List<RaceResult> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var serializer = new XmlSerializer(typeof(List<RaceResult>));
        using FileStream stream = File.Create(path);
        serializer.Serialize(stream, rows);
    }

    public static List<RaceResult> LoadJson(string path)
    {
        string json = File.ReadAllText(path);
        List<RaceResult>? rows = JsonSerializer.Deserialize<List<RaceResult>>(json);
        if (rows is null)
        {
            throw new ArgumentException("Файл JSON не содержит списка записей.");
        }

        return rows;
    }

    public static List<RaceResult> LoadXml(string path)
    {
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        };

        var serializer = new XmlSerializer(typeof(List<RaceResult>));
        using XmlReader reader = XmlReader.Create(path, settings);
        object? result = serializer.Deserialize(reader);
        if (result is not List<RaceResult> rows)
        {
            throw new ArgumentException("Файл XML не содержит списка записей.");
        }

        return rows;
    }
}
