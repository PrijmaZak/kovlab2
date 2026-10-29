using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace RaceLog;

/// <summary>
/// Журнал заездов: форма связывает поля, таблицу, поиск, SQLite и копии JSON/XML.
/// </summary>
public partial class Form1 : Form
{
    private readonly RaceRepository repository;
    private List<RaceResult> allResults = new();

    public Form1()
    {
        InitializeComponent();

        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RaceLogLab2");
        Directory.CreateDirectory(folder);
        string databasePath = Path.Combine(folder, "racelog.db");

        repository = new RaceRepository(databasePath);
        lblStorage.Text = "База данных: " + repository.DatabasePath;

        cmbCar.Items.AddRange(RaceRules.Cars);
        cmbTrack.Items.AddRange(RaceRules.Tracks);
        cmbTrackFilter.Items.Add(RaceQuery.AllTracks);
        cmbTrackFilter.Items.AddRange(RaceRules.Tracks);

        cmbCar.SelectedIndex = 0;
        cmbTrack.SelectedIndex = 0;
        cmbTrackFilter.SelectedIndex = 0;

        txtSeconds.KeyDown += txtSeconds_KeyDown;

        try
        {
            repository.Initialize();
            Reload();
        }
        catch (Exception ex)
        {
            ShowError("Не удалось открыть журнал: " + ex.Message);
        }
    }

    // ---------- состояние и сообщения ----------

    private void ShowError(string message)
    {
        lblStatus.ForeColor = Color.Firebrick;
        lblStatus.Text = message;
    }

    private void ShowStatus(string message)
    {
        lblStatus.ForeColor = Color.DarkGreen;
        lblStatus.Text = message;
    }

    private void Reload()
    {
        allResults = repository.GetAll();
        ApplyFilter();
    }

    private void ApplyFilter(bool updateStatus = true)
    {
        List<RaceResult> view = RaceQuery.Apply(
            allResults,
            txtSearch.Text,
            cmbTrackFilter.SelectedItem as string);

        source.DataSource = view;
        gridResults.ClearSelection();

        if (updateStatus)
        {
            ShowStatus($"Показано: {view.Count}. Всего: {allResults.Count}.");
        }
    }

    private RaceResult? GetSelectedRow()
    {
        if (gridResults.SelectedRows.Count != 1)
        {
            return null;
        }

        return gridResults.SelectedRows[0].DataBoundItem as RaceResult;
    }

    // ---------- ввод ----------

    private RaceResult ReadInput(long id)
    {
        var row = new RaceResult
        {
            Id = id,
            Driver = txtDriver.Text.Trim(),
            Car = cmbCar.SelectedItem as string ?? string.Empty,
            Track = cmbTrack.SelectedItem as string ?? string.Empty
        };

        if (!int.TryParse(txtSeconds.Text.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out int seconds))
        {
            throw new ArgumentException("Время должно быть целым числом секунд, например 94.");
        }

        row.LapSeconds = seconds;
        RaceRules.Validate(row);
        return row;
    }

    private void FillInput(RaceResult row)
    {
        txtDriver.Text = row.Driver;
        txtSeconds.Text = row.LapSeconds.ToString(CultureInfo.CurrentCulture);
        cmbCar.SelectedItem = row.Car;
        cmbTrack.SelectedItem = row.Track;
    }

    private void ClearInput()
    {
        txtDriver.Clear();
        txtSeconds.Clear();
        cmbCar.SelectedIndex = 0;
        cmbTrack.SelectedIndex = 0;
        gridResults.ClearSelection();
        txtDriver.Focus();
    }

    // ---------- добавление, правка, удаление ----------

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        try
        {
            RaceResult row = ReadInput(0);
            long id = repository.Add(row);
            Reload();
            ClearInput();
            ShowStatus($"Добавлено: Id {id}, {row.Driver}, {row.LapSeconds} с.");
        }
        catch (ArgumentException ex)
        {
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            ShowError("Не удалось добавить запись: " + ex.Message);
        }
    }

    private void btnUpdate_Click(object? sender, EventArgs e)
    {
        RaceResult? selected = GetSelectedRow();
        if (selected is null)
        {
            ShowError("Выберите запись в таблице для изменения.");
            return;
        }

        try
        {
            RaceResult row = ReadInput(selected.Id);
            repository.Update(row);
            Reload();
            ClearInput();
            ShowStatus($"Сохранено: Id {row.Id}, {row.Driver}, {row.LapSeconds} с.");
        }
        catch (ArgumentException ex)
        {
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            ShowError("Не удалось сохранить изменения: " + ex.Message);
        }
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        RaceResult? selected = GetSelectedRow();
        if (selected is null)
        {
            ShowError("Выберите запись в таблице для удаления.");
            return;
        }

        DialogResult answer = MessageBox.Show(
            $"Удалить заезд Id {selected.Id} пилота {selected.Driver}?",
            "Удаление записи",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (answer != DialogResult.Yes)
        {
            ShowStatus("Удаление отменено.");
            return;
        }

        try
        {
            repository.Delete(selected.Id);
            Reload();
            ClearInput();
            ShowStatus($"Удалено: Id {selected.Id}.");
        }
        catch (Exception ex)
        {
            ShowError("Не удалось удалить запись: " + ex.Message);
        }
    }

    private void btnClear_Click(object? sender, EventArgs e)
    {
        ClearInput();
        ShowStatus("Поля и выделение очищены. Записи не изменены.");
    }

    private void btnReload_Click(object? sender, EventArgs e)
    {
        try
        {
            Reload();
        }
        catch (Exception ex)
        {
            ShowError("Не удалось прочитать журнал: " + ex.Message);
        }
    }

    private void gridResults_SelectionChanged(object? sender, EventArgs e)
    {
        RaceResult? row = GetSelectedRow();
        if (row is null)
        {
            return;
        }

        FillInput(row);
    }

    private void txtSeconds_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            btnAdd.PerformClick();
        }
    }

    // ---------- поиск и фильтр ----------

    private void btnFilter_Click(object? sender, EventArgs e)
    {
        ApplyFilter();
    }

    private void btnResetFilter_Click(object? sender, EventArgs e)
    {
        txtSearch.Clear();
        cmbTrackFilter.SelectedIndex = 0;
        ApplyFilter();
    }

    // ---------- копии JSON и XML ----------

    private void btnSaveJson_Click(object? sender, EventArgs e)
    {
        SaveCopy("JSON-файл (*.json)|*.json|Все файлы (*.*)|*.*", RaceTransfer.SaveJson);
    }

    private void btnSaveXml_Click(object? sender, EventArgs e)
    {
        SaveCopy("XML-файл (*.xml)|*.xml|Все файлы (*.*)|*.*", RaceTransfer.SaveXml);
    }

    private void SaveCopy(string filter, Action<string, List<RaceResult>> save)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Сохранить копию журнала",
            Filter = filter,
            AddExtension = true,
            DefaultExt = filter.StartsWith("JSON", StringComparison.Ordinal) ? "json" : "xml",
            FileName = "Заезды"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (string.Equals(Path.GetFullPath(dialog.FileName), repository.DatabasePath, StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Нельзя перезаписать рабочую базу данных копией.");
            return;
        }

        try
        {
            List<RaceResult> rows = repository.GetAll();
            save(dialog.FileName, rows);
            ShowStatus($"Сохранено записей: {rows.Count}. Файл: {dialog.FileName}");
        }
        catch (Exception ex)
        {
            ShowError("Не удалось сохранить копию: " + ex.Message);
        }
    }

    private void btnLoadJson_Click(object? sender, EventArgs e)
    {
        LoadCopy("JSON-файл (*.json)|*.json|Все файлы (*.*)|*.*", RaceTransfer.LoadJson);
    }

    private void btnLoadXml_Click(object? sender, EventArgs e)
    {
        LoadCopy("XML-файл (*.xml)|*.xml|Все файлы (*.*)|*.*", RaceTransfer.LoadXml);
    }

    private void LoadCopy(string filter, Func<string, List<RaceResult>> load)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Восстановить журнал из копии",
            Filter = filter,
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        List<RaceResult> rows;
        try
        {
            rows = load(dialog.FileName);
            RaceRules.ValidateImport(rows);
        }
        catch (ArgumentException ex)
        {
            ShowError("Копия отклонена: " + ex.Message);
            return;
        }
        catch (Exception ex)
        {
            ShowError("Не удалось прочитать копию: " + ex.Message);
            return;
        }

        DialogResult answer = MessageBox.Show(
            $"Текущий журнал: {allResults.Count} записей.\nВ копии: {rows.Count} записей.\n\n" +
            "Весь текущий журнал будет заменён. Продолжить?",
            "Восстановление журнала",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (answer != DialogResult.Yes)
        {
            ShowStatus("Восстановление отменено. Журнал не изменён.");
            return;
        }

        try
        {
            repository.ReplaceAll(rows);
            txtSearch.Clear();
            cmbTrackFilter.SelectedIndex = 0;
            Reload();
            ClearInput();
            ShowStatus($"Журнал восстановлен. Записей: {rows.Count}.");
        }
        catch (Exception ex)
        {
            ShowError("Не удалось восстановить журнал: " + ex.Message);
        }
    }
}
