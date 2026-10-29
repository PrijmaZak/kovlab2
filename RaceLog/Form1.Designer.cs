#nullable disable

namespace RaceLog;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            repository?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        grpInput = new GroupBox();
        lblDriver = new Label();
        txtDriver = new TextBox();
        lblCar = new Label();
        cmbCar = new ComboBox();
        lblTrack = new Label();
        cmbTrack = new ComboBox();
        lblSeconds = new Label();
        txtSeconds = new TextBox();
        btnAdd = new Button();
        btnUpdate = new Button();
        btnDelete = new Button();
        lblSearch = new Label();
        txtSearch = new TextBox();
        lblFilter = new Label();
        cmbTrackFilter = new ComboBox();
        btnFilter = new Button();
        btnResetFilter = new Button();
        btnReload = new Button();
        btnClear = new Button();
        gridResults = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colDriver = new DataGridViewTextBoxColumn();
        colCar = new DataGridViewTextBoxColumn();
        colTrack = new DataGridViewTextBoxColumn();
        colLapSeconds = new DataGridViewTextBoxColumn();
        source = new BindingSource(components);
        btnSaveJson = new Button();
        btnLoadJson = new Button();
        btnSaveXml = new Button();
        btnLoadXml = new Button();
        lblNote = new Label();
        lblStatus = new Label();
        lblStorage = new Label();
        grpInput.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridResults).BeginInit();
        ((System.ComponentModel.ISupportInitialize)source).BeginInit();
        SuspendLayout();
        // 
        // grpInput
        // 
        grpInput.Controls.Add(lblDriver);
        grpInput.Controls.Add(txtDriver);
        grpInput.Controls.Add(lblCar);
        grpInput.Controls.Add(cmbCar);
        grpInput.Controls.Add(lblTrack);
        grpInput.Controls.Add(cmbTrack);
        grpInput.Controls.Add(lblSeconds);
        grpInput.Controls.Add(txtSeconds);
        grpInput.Controls.Add(btnAdd);
        grpInput.Controls.Add(btnUpdate);
        grpInput.Controls.Add(btnDelete);
        grpInput.Location = new Point(12, 12);
        grpInput.Name = "grpInput";
        grpInput.Size = new Size(1176, 92);
        grpInput.TabIndex = 0;
        grpInput.TabStop = false;
        grpInput.Text = "Данные заезда";
        // 
        // lblDriver
        // 
        lblDriver.AutoSize = true;
        lblDriver.Location = new Point(14, 24);
        lblDriver.Name = "lblDriver";
        lblDriver.Size = new Size(42, 15);
        lblDriver.TabIndex = 0;
        lblDriver.Text = "Пилот";
        // 
        // txtDriver
        // 
        txtDriver.Location = new Point(14, 46);
        txtDriver.MaxLength = 60;
        txtDriver.Name = "txtDriver";
        txtDriver.Size = new Size(240, 23);
        txtDriver.TabIndex = 1;
        // 
        // lblCar
        // 
        lblCar.AutoSize = true;
        lblCar.Location = new Point(262, 24);
        lblCar.Name = "lblCar";
        lblCar.Size = new Size(78, 15);
        lblCar.TabIndex = 2;
        lblCar.Text = "Автомобиль";
        // 
        // cmbCar
        // 
        cmbCar.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCar.FormattingEnabled = true;
        cmbCar.Location = new Point(262, 46);
        cmbCar.Name = "cmbCar";
        cmbCar.Size = new Size(170, 23);
        cmbCar.TabIndex = 3;
        // 
        // lblTrack
        // 
        lblTrack.AutoSize = true;
        lblTrack.Location = new Point(452, 24);
        lblTrack.Name = "lblTrack";
        lblTrack.Size = new Size(48, 15);
        lblTrack.TabIndex = 4;
        lblTrack.Text = "Трасса";
        // 
        // cmbTrack
        // 
        cmbTrack.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTrack.FormattingEnabled = true;
        cmbTrack.Location = new Point(452, 46);
        cmbTrack.Name = "cmbTrack";
        cmbTrack.Size = new Size(180, 23);
        cmbTrack.TabIndex = 5;
        // 
        // lblSeconds
        // 
        lblSeconds.AutoSize = true;
        lblSeconds.Location = new Point(652, 24);
        lblSeconds.Name = "lblSeconds";
        lblSeconds.Size = new Size(59, 15);
        lblSeconds.TabIndex = 6;
        lblSeconds.Text = "Время, с";
        // 
        // txtSeconds
        // 
        txtSeconds.Location = new Point(652, 46);
        txtSeconds.MaxLength = 4;
        txtSeconds.Name = "txtSeconds";
        txtSeconds.Size = new Size(100, 23);
        txtSeconds.TabIndex = 7;
        // 
        // btnAdd
        // 
        btnAdd.Location = new Point(770, 45);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(105, 30);
        btnAdd.TabIndex = 8;
        btnAdd.Text = "Добавить";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        // 
        // btnUpdate
        // 
        btnUpdate.Location = new Point(885, 45);
        btnUpdate.Name = "btnUpdate";
        btnUpdate.Size = new Size(150, 30);
        btnUpdate.TabIndex = 9;
        btnUpdate.Text = "Сохранить правку";
        btnUpdate.UseVisualStyleBackColor = true;
        btnUpdate.Click += btnUpdate_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(1045, 45);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(110, 30);
        btnDelete.TabIndex = 10;
        btnDelete.Text = "Удалить";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // lblSearch
        // 
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(12, 114);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(105, 15);
        lblSearch.TabIndex = 1;
        lblSearch.Text = "Поиск по пилоту:";
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(140, 110);
        txtSearch.Name = "txtSearch";
        txtSearch.Size = new Size(220, 23);
        txtSearch.TabIndex = 2;
        // 
        // lblFilter
        // 
        lblFilter.AutoSize = true;
        lblFilter.Location = new Point(372, 114);
        lblFilter.Name = "lblFilter";
        lblFilter.Size = new Size(48, 15);
        lblFilter.TabIndex = 3;
        lblFilter.Text = "Трасса:";
        // 
        // cmbTrackFilter
        // 
        cmbTrackFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTrackFilter.FormattingEnabled = true;
        cmbTrackFilter.Location = new Point(432, 110);
        cmbTrackFilter.Name = "cmbTrackFilter";
        cmbTrackFilter.Size = new Size(180, 23);
        cmbTrackFilter.TabIndex = 4;
        // 
        // btnFilter
        // 
        btnFilter.Location = new Point(622, 108);
        btnFilter.Name = "btnFilter";
        btnFilter.Size = new Size(95, 29);
        btnFilter.TabIndex = 5;
        btnFilter.Text = "Найти";
        btnFilter.UseVisualStyleBackColor = true;
        btnFilter.Click += btnFilter_Click;
        // 
        // btnResetFilter
        // 
        btnResetFilter.Location = new Point(725, 108);
        btnResetFilter.Name = "btnResetFilter";
        btnResetFilter.Size = new Size(120, 29);
        btnResetFilter.TabIndex = 6;
        btnResetFilter.Text = "Сброс поиска";
        btnResetFilter.UseVisualStyleBackColor = true;
        btnResetFilter.Click += btnResetFilter_Click;
        // 
        // btnReload
        // 
        btnReload.Location = new Point(853, 108);
        btnReload.Name = "btnReload";
        btnReload.Size = new Size(105, 29);
        btnReload.TabIndex = 7;
        btnReload.Text = "Обновить";
        btnReload.UseVisualStyleBackColor = true;
        btnReload.Click += btnReload_Click;
        // 
        // btnClear
        // 
        btnClear.Location = new Point(966, 108);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(105, 29);
        btnClear.TabIndex = 8;
        btnClear.Text = "Очистить";
        btnClear.UseVisualStyleBackColor = true;
        btnClear.Click += btnClear_Click;
        // 
        // gridResults
        // 
        gridResults.AllowUserToAddRows = false;
        gridResults.AllowUserToDeleteRows = false;
        gridResults.AllowUserToResizeRows = false;
        gridResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        gridResults.AutoGenerateColumns = false;
        gridResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        gridResults.BackgroundColor = Color.White;
        gridResults.BorderStyle = BorderStyle.Fixed3D;
        gridResults.ColumnHeadersHeight = 32;
        gridResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        gridResults.Columns.AddRange(new DataGridViewColumn[] { colId, colDriver, colCar, colTrack, colLapSeconds });
        gridResults.DataSource = source;
        gridResults.Location = new Point(12, 148);
        gridResults.MultiSelect = false;
        gridResults.Name = "gridResults";
        gridResults.ReadOnly = true;
        gridResults.RowHeadersVisible = false;
        gridResults.RowTemplate.Height = 26;
        gridResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridResults.Size = new Size(1176, 380);
        gridResults.TabIndex = 9;
        gridResults.SelectionChanged += gridResults_SelectionChanged;
        // 
        // colId
        // 
        colId.DataPropertyName = "Id";
        colId.FillWeight = 40F;
        colId.HeaderText = "Id";
        colId.MinimumWidth = 55;
        colId.Name = "colId";
        colId.ReadOnly = true;
        colId.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // colDriver
        // 
        colDriver.DataPropertyName = "Driver";
        colDriver.FillWeight = 100F;
        colDriver.HeaderText = "Пилот";
        colDriver.Name = "colDriver";
        colDriver.ReadOnly = true;
        colDriver.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // colCar
        // 
        colCar.DataPropertyName = "Car";
        colCar.FillWeight = 80F;
        colCar.HeaderText = "Автомобиль";
        colCar.Name = "colCar";
        colCar.ReadOnly = true;
        colCar.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // colTrack
        // 
        colTrack.DataPropertyName = "Track";
        colTrack.FillWeight = 100F;
        colTrack.HeaderText = "Трасса";
        colTrack.Name = "colTrack";
        colTrack.ReadOnly = true;
        colTrack.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // colLapSeconds
        // 
        colLapSeconds.DataPropertyName = "LapSeconds";
        colLapSeconds.FillWeight = 60F;
        colLapSeconds.HeaderText = "Время, с";
        colLapSeconds.Name = "colLapSeconds";
        colLapSeconds.ReadOnly = true;
        colLapSeconds.SortMode = DataGridViewColumnSortMode.NotSortable;
        // 
        // source
        // 
        source.AllowNew = false;
        // 
        // btnSaveJson
        // 
        btnSaveJson.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnSaveJson.Location = new Point(12, 540);
        btnSaveJson.Name = "btnSaveJson";
        btnSaveJson.Size = new Size(130, 30);
        btnSaveJson.TabIndex = 10;
        btnSaveJson.Text = "Сохранить JSON";
        btnSaveJson.UseVisualStyleBackColor = true;
        btnSaveJson.Click += btnSaveJson_Click;
        // 
        // btnLoadJson
        // 
        btnLoadJson.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnLoadJson.Location = new Point(150, 540);
        btnLoadJson.Name = "btnLoadJson";
        btnLoadJson.Size = new Size(130, 30);
        btnLoadJson.TabIndex = 11;
        btnLoadJson.Text = "Загрузить JSON";
        btnLoadJson.UseVisualStyleBackColor = true;
        btnLoadJson.Click += btnLoadJson_Click;
        // 
        // btnSaveXml
        // 
        btnSaveXml.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnSaveXml.Location = new Point(288, 540);
        btnSaveXml.Name = "btnSaveXml";
        btnSaveXml.Size = new Size(130, 30);
        btnSaveXml.TabIndex = 12;
        btnSaveXml.Text = "Сохранить XML";
        btnSaveXml.UseVisualStyleBackColor = true;
        btnSaveXml.Click += btnSaveXml_Click;
        // 
        // btnLoadXml
        // 
        btnLoadXml.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnLoadXml.Location = new Point(426, 540);
        btnLoadXml.Name = "btnLoadXml";
        btnLoadXml.Size = new Size(130, 30);
        btnLoadXml.TabIndex = 13;
        btnLoadXml.Text = "Загрузить XML";
        btnLoadXml.UseVisualStyleBackColor = true;
        btnLoadXml.Click += btnLoadXml_Click;
        // 
        // lblNote
        // 
        lblNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblNote.AutoSize = true;
        lblNote.ForeColor = SystemColors.GrayText;
        lblNote.Location = new Point(570, 547);
        lblNote.Name = "lblNote";
        lblNote.Size = new Size(321, 15);
        lblNote.TabIndex = 14;
        lblNote.Text = "Копия всегда содержит весь журнал, даже при активном поиске.";
        // 
        // lblStatus
        // 
        lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblStatus.ForeColor = Color.DarkGreen;
        lblStatus.Location = new Point(12, 580);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(1176, 22);
        lblStatus.TabIndex = 15;
        lblStatus.Text = "Готово.";
        // 
        // lblStorage
        // 
        lblStorage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblStorage.AutoEllipsis = true;
        lblStorage.ForeColor = SystemColors.GrayText;
        lblStorage.Location = new Point(12, 606);
        lblStorage.Name = "lblStorage";
        lblStorage.Size = new Size(1176, 20);
        lblStorage.TabIndex = 16;
        lblStorage.Text = "База данных:";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 650);
        Controls.Add(lblStorage);
        Controls.Add(lblStatus);
        Controls.Add(lblNote);
        Controls.Add(btnLoadXml);
        Controls.Add(btnSaveXml);
        Controls.Add(btnLoadJson);
        Controls.Add(btnSaveJson);
        Controls.Add(gridResults);
        Controls.Add(btnClear);
        Controls.Add(btnReload);
        Controls.Add(btnResetFilter);
        Controls.Add(btnFilter);
        Controls.Add(cmbTrackFilter);
        Controls.Add(lblFilter);
        Controls.Add(txtSearch);
        Controls.Add(lblSearch);
        Controls.Add(grpInput);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(1216, 690);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "RaceLog — журнал заездов (ЛР 2)";
        grpInput.ResumeLayout(false);
        grpInput.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gridResults).EndInit();
        ((System.ComponentModel.ISupportInitialize)source).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private GroupBox grpInput;
    private Label lblDriver;
    private TextBox txtDriver;
    private Label lblCar;
    private ComboBox cmbCar;
    private Label lblTrack;
    private ComboBox cmbTrack;
    private Label lblSeconds;
    private TextBox txtSeconds;
    private Button btnAdd;
    private Button btnUpdate;
    private Button btnDelete;
    private Label lblSearch;
    private TextBox txtSearch;
    private Label lblFilter;
    private ComboBox cmbTrackFilter;
    private Button btnFilter;
    private Button btnResetFilter;
    private Button btnReload;
    private Button btnClear;
    private DataGridView gridResults;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colDriver;
    private DataGridViewTextBoxColumn colCar;
    private DataGridViewTextBoxColumn colTrack;
    private DataGridViewTextBoxColumn colLapSeconds;
    private BindingSource source;
    private Button btnSaveJson;
    private Button btnLoadJson;
    private Button btnSaveXml;
    private Button btnLoadXml;
    private Label lblNote;
    private Label lblStatus;
    private Label lblStorage;
}
