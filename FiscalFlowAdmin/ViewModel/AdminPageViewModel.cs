// AdminPageViewModel.cs
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FiscalFlowAdmin.Database;
using FiscalFlowAdmin.ViewModel.Commands;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Npgsql;

namespace FiscalFlowAdmin.ViewModel
{
    public sealed class AdminPageViewModel : ViewModelBase, IDisposable
    {
        private readonly DataManager _dataManager;
        private readonly string _connectionString;

        // Свойства для мониторинга нагрузки
        private string _databaseLoad;
        public string DatabaseLoad
        {
            get => _databaseLoad;
            set { _databaseLoad = value; OnPropertyChanged(nameof(DatabaseLoad)); }
        }

        private string _queriesPerMinute;
        public string QueriesPerMinute
        {
            get => _queriesPerMinute;
            set { _queriesPerMinute = value; OnPropertyChanged(nameof(QueriesPerMinute)); }
        }

        private string _transactionsPerMinute;
        public string TransactionsPerMinute
        {
            get => _transactionsPerMinute;
            set { _transactionsPerMinute = value; OnPropertyChanged(nameof(TransactionsPerMinute)); }
        }

        private string _tuplesInsertedPerMinute;
        public string TuplesInsertedPerMinute
        {
            get => _tuplesInsertedPerMinute;
            set { _tuplesInsertedPerMinute = value; OnPropertyChanged(nameof(TuplesInsertedPerMinute)); }
        }

        private string _tuplesUpdatedPerMinute;
        public string TuplesUpdatedPerMinute
        {
            get => _tuplesUpdatedPerMinute;
            set { _tuplesUpdatedPerMinute = value; OnPropertyChanged(nameof(TuplesUpdatedPerMinute)); }
        }

        private string _tuplesDeletedPerMinute;
        public string TuplesDeletedPerMinute
        {
            get => _tuplesDeletedPerMinute;
            set { _tuplesDeletedPerMinute = value; OnPropertyChanged(nameof(TuplesDeletedPerMinute)); }
        }

        private string _cacheHitRatio;
        public string CacheHitRatio
        {
            get => _cacheHitRatio;
            set { _cacheHitRatio = value; OnPropertyChanged(nameof(CacheHitRatio)); }
        }

        // Команды
        public ICommand BackupDatabaseCommand { get; }
        public ICommand RestoreDatabaseCommand { get; }
        public ICommand ExportToSqlCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand ImportFromSqlCommand { get; }
        public ICommand ImportFromCsvCommand { get; }

        // Данные для диаграмм
        public SeriesCollection ChartSeries { get; set; }
        public string[] ChartLabels { get; set; }

        // Поля для хранения предыдущих значений метрик
        private long _prevXactCommit;
        private long _prevXactRollback;
        private long _prevTuplesInserted;
        private long _prevTuplesUpdated;
        private long _prevTuplesDeleted;
        private long _prevBlksRead;
        private long _prevBlksHit;
        private DateTime _prevTime;

        // Токен для отмены мониторинга при закрытии приложения
        private CancellationTokenSource _cancellationTokenSource;

        // Семафор для предотвращения одновременного выполнения мониторинга
        private SemaphoreSlim _monitoringSemaphore = new SemaphoreSlim(1, 1);

        public AdminPageViewModel()
        {
            _dataManager = new DataManager();
            _connectionString = "Host=127.0.0.1;Database=fiscal_flow;Username=fiscal_flow_superuser;Password=fiscAl_fLow#4134^54;Port=5432;";
           // _connectionString = "Host=junction.proxy.rlwy.net;Database=railway;Username=postgres;Password=iOQSzuEAJtEbaIAPZWkiuuHywqmaMszQ;Port=45507;";

            // Инициализация команд
            BackupDatabaseCommand = new RelayCommand(BackupDatabase);
            RestoreDatabaseCommand = new RelayCommand(RestoreDatabase);
            ExportToSqlCommand = new RelayCommand(ExportToSql);
            ExportToCsvCommand = new RelayCommand(ExportToCsv);
            ImportFromSqlCommand = new RelayCommand(ImportFromSql);
            ImportFromCsvCommand = new RelayCommand(ImportFromCsv);

            // Загрузка данных
            LoadChartData();

            // Инициализация логирования
            ConfigureLogging();

            // Инициализация токена отмены
            _cancellationTokenSource = new CancellationTokenSource();

            // Запуск мониторинга нагрузки на БД
            StartDatabaseMonitoringAsync(_cancellationTokenSource.Token);
        }

        #region Методы резервного копирования и восстановления

        private void BackupDatabase(object parameter)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "SQL Files (*.sql)|*.sql",
                    Title = "Сохранить резервную копию базы данных"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var backupFilePath = saveFileDialog.FileName;
                    var builder = new NpgsqlConnectionStringBuilder(_connectionString);
                    var host = builder.Host;
                    var port = builder.Port;
                    var database = builder.Database;
                    var username = builder.Username;
                    var password = builder.Password;

                    // Создание файла для хранения пароля (pgpass.conf)
                    var pgpassFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "pgpass.conf");
                    File.WriteAllText(pgpassFilePath, $"{host}:{port}:{database}:{username}:{password}");
                    Environment.SetEnvironmentVariable("PGPASSFILE", pgpassFilePath);

                    var psi = new ProcessStartInfo
                    {
                        FileName = "pg_dump",
                        Arguments = $"-h {host} -p {port} -U {username} -F c -b -v -f \"{backupFilePath}\" {database}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Установка переменной окружения PGPASSWORD
                    psi.EnvironmentVariables["PGPASSWORD"] = password;

                    var process = new Process { StartInfo = psi };

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Trace.TraceInformation(e.Data);
                        }
                    };

                    // Подключение общего обработчика ошибок
                    process.ErrorDataReceived += HandleProcessErrorDataReceived;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    // Удаление файла с паролем
                    File.Delete(pgpassFilePath);
                    Environment.SetEnvironmentVariable("PGPASSFILE", null);

                    if (process.ExitCode == 0)
                    {
                        MessageBox.Show("Резервное копирование успешно завершено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        Trace.TraceInformation("Резервное копирование успешно завершено.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при резервном копировании. Проверьте журнал ошибок для деталей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Trace.TraceError("Ошибка при резервном копировании. Код выхода: " + process.ExitCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при резервном копировании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"Ошибка при резервном копировании: {ex}");
            }
        }

        private void RestoreDatabase(object parameter)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Custom Backup Files (*.sql)|*.sql|All Files (*.*)|*.*",
                    Title = "Выберите файл резервной копии базы данных"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var backupFilePath = openFileDialog.FileName;
                    var builder = new NpgsqlConnectionStringBuilder(_connectionString);
                    var host = builder.Host;
                    var port = builder.Port;
                    var database = builder.Database;
                    var username = builder.Username;
                    var password = builder.Password;

                    // Создание файла для хранения пароля (pgpass.conf)
                    var pgpassFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "pgpass.conf");
                    File.WriteAllText(pgpassFilePath, $"{host}:{port}:*:{username}:{password}");
                    Environment.SetEnvironmentVariable("PGPASSFILE", pgpassFilePath);

                    var psi = new ProcessStartInfo
                    {
                        FileName = "pg_restore",
                        Arguments = $"-h {host} -p {port} -U {username} -d {database} -v \"{backupFilePath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Установка переменной окружения PGPASSWORD
                    psi.EnvironmentVariables["PGPASSWORD"] = password;

                    var process = new Process { StartInfo = psi };

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Trace.TraceInformation(e.Data);
                        }
                    };

                    // Подключение общего обработчика ошибок
                    process.ErrorDataReceived += HandleProcessErrorDataReceived;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    // Удаление файла с паролем
                    File.Delete(pgpassFilePath);
                    Environment.SetEnvironmentVariable("PGPASSFILE", null);

                    if (process.ExitCode == 0)
                    {
                        MessageBox.Show("Восстановление из резервной копии успешно завершено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        Trace.TraceInformation("Восстановление из резервной копии успешно завершено.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при восстановлении. Проверьте журнал ошибок для деталей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Trace.TraceError("Ошибка при восстановлении. Код выхода: " + process.ExitCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при восстановлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"Ошибка при восстановлении: {ex}");
            }
        }
        #endregion

        #region Методы экспорта и импорта

        private void ExportToSql(object parameter)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "SQL Files (*.sql)|*.sql",
                    Title = "Экспортировать базу данных в SQL"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var exportFilePath = saveFileDialog.FileName;
                    var builder = new NpgsqlConnectionStringBuilder(_connectionString);
                    var host = builder.Host;
                    var port = builder.Port;
                    var database = builder.Database;
                    var username = builder.Username;
                    var password = builder.Password;

                    // Создание файла для хранения пароля (pgpass.conf)
                    var pgpassFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "pgpass.conf");
                    File.WriteAllText(pgpassFilePath, $"{host}:{port}:{database}:{username}:{password}");
                    Environment.SetEnvironmentVariable("PGPASSFILE", pgpassFilePath);

                    var psi = new ProcessStartInfo
                    {
                        FileName = "pg_dump",
                        Arguments = $"--file \"{exportFilePath}\" --dbname \"{_connectionString}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Установка переменной окружения PGPASSWORD
                    psi.EnvironmentVariables["PGPASSWORD"] = password;

                    var process = new Process { StartInfo = psi };

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Trace.TraceInformation(e.Data);
                        }
                    };

                    // Подключение общего обработчика ошибок
                    process.ErrorDataReceived += HandleProcessErrorDataReceived;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    // Удаление файла с паролем
                    File.Delete(pgpassFilePath);
                    Environment.SetEnvironmentVariable("PGPASSFILE", null);

                    if (process.ExitCode == 0)
                    {
                        MessageBox.Show("Экспорт в SQL успешно завершен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        Trace.TraceInformation("Экспорт в SQL успешно завершен.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при экспорте в SQL. Проверьте журнал ошибок для деталей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Trace.TraceError("Ошибка при экспорте в SQL. Код выхода: " + process.ExitCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в SQL: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"Ошибка при экспорте в SQL: {ex}");
            }
        }

        private async void ExportToCsv(object parameter)
        {
            try
            {
                var folderDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Выберите папку для сохранения CSV-файлов"
                };

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var exportDirectory = folderDialog.FileName;

                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        var tables = new[]
                        {
                            "authentication_user", "authentication_profile", "authentication_devicetoken",
                            "finances_app_bill", "finances_app_credit", "finances_app_currency",
                            "finances_app_dailycategoryexpense", "finances_app_dailyreport", "finances_app_debt",
                            "finances_app_monthlyexpense", "finances_app_rate", "finances_app_transaction",
                            "finances_app_transactioncategory"
                        };

                        foreach (var table in tables)
                        {
                            var filePath = Path.Combine(exportDirectory, $"{table}.csv");
                            using (var writer = new StreamWriter(filePath))
                            using (var copy = connection.BeginTextExport($"COPY {table} TO STDOUT WITH (FORMAT CSV, HEADER)"))
                            {
                                char[] buffer = new char[8192];
                                int read;
                                while ((read = await copy.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await writer.WriteAsync(buffer, 0, read);
                                }
                            }
                            Trace.TraceInformation($"Экспорт таблицы {table} в CSV завершен.");
                        }
                    }

                    MessageBox.Show("Экспорт в CSV успешно завершен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Trace.TraceInformation("Экспорт в CSV успешно завершен.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в CSV: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"Ошибка при экспорте в CSV: {ex}");
            }
        }

        private void ImportFromSql(object parameter)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "SQL Files (*.sql)|*.sql",
                    Title = "Импортировать данные из SQL файла"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var importFilePath = openFileDialog.FileName;
                    var builder = new NpgsqlConnectionStringBuilder(_connectionString);
                    var host = builder.Host;
                    var port = builder.Port;
                    var database = builder.Database;
                    var username = builder.Username;
                    var password = builder.Password;

                    // Создание файла для хранения пароля (pgpass.conf)
                    var pgpassFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "pgpass.conf");
                    File.WriteAllText(pgpassFilePath, $"{host}:{port}:{database}:{username}:{password}");
                    Environment.SetEnvironmentVariable("PGPASSFILE", pgpassFilePath);

                    var psi = new ProcessStartInfo
                    {
                        FileName = "psql",
                        Arguments = $"--dbname \"{_connectionString}\" --file \"{importFilePath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    // Установка переменной окружения PGPASSWORD
                    psi.EnvironmentVariables["PGPASSWORD"] = password;

                    var process = new Process { StartInfo = psi };

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Trace.TraceInformation(e.Data);
                        }
                    };

                    // Подключение общего обработчика ошибок
                    process.ErrorDataReceived += HandleProcessErrorDataReceived;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    // Удаление файла с паролем
                    File.Delete(pgpassFilePath);
                    Environment.SetEnvironmentVariable("PGPASSFILE", null);

                    if (process.ExitCode == 0)
                    {
                        MessageBox.Show("Импорт из SQL успешно завершен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        Trace.TraceInformation("Импорт из SQL успешно завершен.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при импорте из SQL. Проверьте журнал ошибок для деталей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Trace.TraceError("Ошибка при импорте из SQL. Код выхода: " + process.ExitCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте из SQL: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"Ошибка при импорте из SQL: {ex}");
            }
        }

        private async void ImportFromCsv(object parameter)
        {
            try
            {
                var folderDialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Выберите папку с CSV-файлами для импорта"
                };

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var importDirectory = folderDialog.FileName;

                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        var tables = new[]
                        {
                            "authentication_user", "authentication_profile", "authentication_devicetoken",
                            "finances_app_bill", "finances_app_credit", "finances_app_currency",
                            "finances_app_dailycategoryexpense", "finances_app_dailyreport", "finances_app_debt",
                            "finances_app_monthlyexpense", "finances_app_rate", "finances_app_transaction",
                            "finances_app_transactioncategory"
                        };

                        foreach (var table in tables)
                        {
                            var filePath = Path.Combine(importDirectory, $"{table}.csv");
                            if (!File.Exists(filePath))
                            {
                                Trace.TraceWarning($"Файл {filePath} не найден. Пропуск таблицы {table}.");
                                continue;
                            }

                            using (var reader = new StreamReader(filePath))
                            using (var copy = connection.BeginTextImport($"COPY {table} FROM STDIN WITH (FORMAT CSV, HEADER)"))
                            {
                                char[] buffer = new char[8192];
                                int read;
                                while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await copy.WriteAsync(buffer, 0, read);
                                }
                            }
                            Trace.TraceInformation($"Импорт таблицы {table} из CSV завершен.");
                        }
                    }

                    MessageBox.Show("Импорт из CSV успешно завершен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Trace.TraceInformation("Импорт из CSV успешно завершен.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте из CSV: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"Ошибка при импорте из CSV: {ex}");
            }
        }

        #endregion

        #region Методы мониторинга БД

        private async void StartDatabaseMonitoringAsync(CancellationToken cancellationToken)
        {
            try
            {
                await InitializePreviousMetricsAsync();

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Попытка войти в семафор
                    if (await _monitoringSemaphore.WaitAsync(0, cancellationToken))
                    {
                        try
                        {
                            await LoadDatabasePerformanceAsync();
                        }
                        finally
                        {
                            _monitoringSemaphore.Release();
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("Пропуск вызова мониторинга, так как предыдущий вызов еще выполняется.");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                // Ожидаемое исключение при отмене задачи
                Trace.TraceInformation("Мониторинг базы данных отменен.");
            }
            catch (Exception ex)
            {
                // Обработка неожиданных ошибок
                Trace.TraceError($"Неожиданная ошибка в мониторинге базы данных: {ex}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DatabaseLoad = "Ошибка при мониторинге базы данных";
                    QueriesPerMinute = "Ошибка при мониторинге базы данных";
                    TransactionsPerMinute = "Ошибка при мониторинге базы данных";
                    TuplesInsertedPerMinute = "Ошибка при мониторинге базы данных";
                    TuplesUpdatedPerMinute = "Ошибка при мониторинге базы данных";
                    TuplesDeletedPerMinute = "Ошибка при мониторинге базы данных";
                    CacheHitRatio = "Ошибка при мониторинге базы данных";
                });
            }
        }

        private async Task InitializePreviousMetricsAsync()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                        SELECT 
                            sum(xact_commit) AS xact_commit, 
                            sum(xact_rollback) AS xact_rollback,
                            sum(tup_inserted) AS tup_inserted,
                            sum(tup_updated) AS tup_updated,
                            sum(tup_deleted) AS tup_deleted,
                            sum(blks_read) AS blks_read,
                            sum(blks_hit) AS blks_hit
                        FROM pg_stat_database
                        WHERE datname = 'fiscal_flow';", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                _prevXactCommit = reader.GetInt64(reader.GetOrdinal("xact_commit"));
                                _prevXactRollback = reader.GetInt64(reader.GetOrdinal("xact_rollback"));
                                _prevTuplesInserted = reader.GetInt64(reader.GetOrdinal("tup_inserted"));
                                _prevTuplesUpdated = reader.GetInt64(reader.GetOrdinal("tup_updated"));
                                _prevTuplesDeleted = reader.GetInt64(reader.GetOrdinal("tup_deleted"));
                                _prevBlksRead = reader.GetInt64(reader.GetOrdinal("blks_read"));
                                _prevBlksHit = reader.GetInt64(reader.GetOrdinal("blks_hit"));
                                _prevTime = DateTime.UtcNow;
                                Trace.TraceInformation("Инициализация предыдущих метрик мониторинга завершена.");
                            }
                            else
                            {
                                Trace.TraceWarning("Не удалось прочитать метрики из pg_stat_database.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок и уведомление пользователя
                Trace.TraceError($"Ошибка при инициализации предыдущих метрик: {ex}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DatabaseLoad = "Ошибка при инициализации мониторинга";
                    QueriesPerMinute = "Ошибка при инициализации мониторинга";
                    TransactionsPerMinute = "Ошибка при инициализации мониторинга";
                    TuplesInsertedPerMinute = "Ошибка при инициализации мониторинга";
                    TuplesUpdatedPerMinute = "Ошибка при инициализации мониторинга";
                    TuplesDeletedPerMinute = "Ошибка при инициализации мониторинга";
                    CacheHitRatio = "Ошибка при инициализации мониторинга";
                });
            }
        }

        private async Task LoadDatabasePerformanceAsync()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Выполнение первого запроса
                    long currentXactCommit,
                        currentXactRollback,
                        currentTuplesInserted,
                        currentTuplesUpdated,
                        currentTuplesDeleted,
                        currentBlksRead,
                        currentBlksHit;

                    using (var command = new NpgsqlCommand(@"
                        SELECT 
                            sum(xact_commit) AS xact_commit, 
                            sum(xact_rollback) AS xact_rollback,
                            sum(tup_inserted) AS tup_inserted,
                            sum(tup_updated) AS tup_updated,
                            sum(tup_deleted) AS tup_deleted,
                            sum(blks_read) AS blks_read,
                            sum(blks_hit) AS blks_hit
                        FROM pg_stat_database
                        WHERE datname = 'fiscal_flow';", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                currentXactCommit = reader.GetInt64(reader.GetOrdinal("xact_commit"));
                                currentXactRollback = reader.GetInt64(reader.GetOrdinal("xact_rollback"));
                                currentTuplesInserted = reader.GetInt64(reader.GetOrdinal("tup_inserted"));
                                currentTuplesUpdated = reader.GetInt64(reader.GetOrdinal("tup_updated"));
                                currentTuplesDeleted = reader.GetInt64(reader.GetOrdinal("tup_deleted"));
                                currentBlksRead = reader.GetInt64(reader.GetOrdinal("blks_read"));
                                currentBlksHit = reader.GetInt64(reader.GetOrdinal("blks_hit"));
                            }
                            else
                            {
                                Trace.TraceWarning("Не удалось прочитать метрики из pg_stat_database.");
                                return;
                            }
                        }
                    }

                    // Выполнение второго запроса
                    long activeConnections;
                    using (var activeConnCommand = new NpgsqlCommand(@"
                        SELECT count(*) 
                        FROM pg_stat_activity 
                        WHERE datname = 'fiscal_flow';", connection))
                    {
                        var activeConnectionsObj = await activeConnCommand.ExecuteScalarAsync();
                        activeConnections = activeConnectionsObj != null ? Convert.ToInt64(activeConnectionsObj) : 0;
                    }

                    DateTime currentTime = DateTime.UtcNow;
                    double elapsedMinutes = (currentTime - _prevTime).TotalMinutes;

                    if (elapsedMinutes <= 0)
                    {
                        elapsedMinutes = 1.0 / 60.0; // Минимальный интервал 1 секунда
                    }

                    // Расчёт транзакций в минуту
                    long committedDiff = currentXactCommit - _prevXactCommit;
                    long rolledBackDiff = currentXactRollback - _prevXactRollback;
                    double transactionsPerMinute = (committedDiff + rolledBackDiff) / elapsedMinutes;

                    // Расчёт операций с кортежами в минуту
                    long insertedDiff = currentTuplesInserted - _prevTuplesInserted;
                    long updatedDiff = currentTuplesUpdated - _prevTuplesUpdated;
                    long deletedDiff = currentTuplesDeleted - _prevTuplesDeleted;
                    double tuplesInsertedPerMinute = insertedDiff / elapsedMinutes;
                    double tuplesUpdatedPerMinute = updatedDiff / elapsedMinutes;
                    double tuplesDeletedPerMinute = deletedDiff / elapsedMinutes;

                    // Расчёт кэш-хит ratio
                    long blksReadDiff = currentBlksRead - _prevBlksRead;
                    long blksHitDiff = currentBlksHit - _prevBlksHit;
                    double cacheHitRatio = (blksReadDiff + blksHitDiff) > 0
                        ? ((double)blksHitDiff / (blksReadDiff + blksHitDiff)) * 100
                        : 0;

                    // Обновление свойств в UI-потоке
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TransactionsPerMinute = $"Транзакций в минуту: {transactionsPerMinute:F2}";
                        QueriesPerMinute = $"Запросов в минуту: {transactionsPerMinute:F2}";
                        TuplesInsertedPerMinute = $"Вставлено строк: {tuplesInsertedPerMinute:F2}";
                        TuplesUpdatedPerMinute = $"Обновлено строк: {tuplesUpdatedPerMinute:F2}";
                        TuplesDeletedPerMinute = $"Удалено строк: {tuplesDeletedPerMinute:F2}";
                        CacheHitRatio = $"Кэш-хит: {cacheHitRatio:F2}%";
                        DatabaseLoad = $"Текущая нагрузка: {activeConnections} активных соединений";
                    });

                    // Обновление предыдущих значений
                    _prevXactCommit = currentXactCommit;
                    _prevXactRollback = currentXactRollback;
                    _prevTuplesInserted = currentTuplesInserted;
                    _prevTuplesUpdated = currentTuplesUpdated;
                    _prevTuplesDeleted = currentTuplesDeleted;
                    _prevBlksRead = currentBlksRead;
                    _prevBlksHit = currentBlksHit;
                    _prevTime = currentTime;

                    Trace.TraceInformation("Мониторинг базы данных обновлен успешно: " +
                                           $"Транзакций в минуту: {transactionsPerMinute:F2}, " +
                                           $"Вставлено строк: {tuplesInsertedPerMinute:F2}, " +
                                           $"Обновлено строк: {tuplesUpdatedPerMinute:F2}, " +
                                           $"Удалено строк: {tuplesDeletedPerMinute:F2}, " +
                                           $"Кэш-хит: {cacheHitRatio:F2}%, " +
                                           $"Активных соединений: {activeConnections}");
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Trace.TraceError($"Ошибка при получении данных нагрузки: {ex}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DatabaseLoad = "Ошибка при получении данных нагрузки";
                    QueriesPerMinute = "Ошибка при получении данных запросов";
                    TransactionsPerMinute = "Ошибка при получении данных транзакций";
                    TuplesInsertedPerMinute = "Ошибка при получении данных вставок";
                    TuplesUpdatedPerMinute = "Ошибка при получении данных обновлений";
                    TuplesDeletedPerMinute = "Ошибка при получении данных удалений";
                    CacheHitRatio = "Ошибка при получении данных кэша";
                });
            }
        }

        #endregion

        #region Методы загрузки данных для диаграмм

        private void LoadChartData()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    // Пример: получение суммарных транзакций по месяцам
                    using (var command = new NpgsqlCommand(@"
                        SELECT date_trunc('month', date) AS month, SUM(amount) 
                        FROM finances_app_transaction
                        GROUP BY month 
                        ORDER BY month", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var values = new ChartValues<double>();
                            var labels = new System.Collections.Generic.List<string>();

                            while (reader.Read())
                            {
                                var month = reader.GetDateTime(0);
                                var amount = reader.IsDBNull(1) ? 0.0 : reader.GetDouble(1);

                                values.Add(amount);
                                labels.Add(month.ToString("MMM yyyy"));
                            }

                            ChartSeries = new SeriesCollection
                            {
                                new ColumnSeries
                                {
                                    Title = "Сумма транзакций",
                                    Values = values
                                }
                            };

                            ChartLabels = labels.ToArray();

                            OnPropertyChanged(nameof(ChartSeries));
                            OnPropertyChanged(nameof(ChartLabels));

                            Trace.TraceInformation("Данные для диаграмм загружены успешно.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных для диаграмм: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Trace.TraceError($"Ошибка при загрузке данных для диаграмм: {ex}");
            }
        }

        #endregion

        #region Конфигурация Логирования

        private void ConfigureLogging()
        {
            try
            {
                // Удаление всех существующих слушателей
                Trace.Listeners.Clear();

                // Создание слушателя для записи логов в файл
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.log");
                TextWriterTraceListener fileListener = new TextWriterTraceListener(logFilePath)
                {
                    Name = "FileListener",
                    TraceOutputOptions = TraceOptions.DateTime
                };
                Trace.Listeners.Add(fileListener);

                // Создание слушателя для вывода логов в Output Window (для отладки)
                ConsoleTraceListener consoleListener = new ConsoleTraceListener
                {
                    Name = "ConsoleListener",
                    TraceOutputOptions = TraceOptions.DateTime
                };
                Trace.Listeners.Add(consoleListener);

                Trace.AutoFlush = true;

                Trace.TraceInformation("Логирование настроено успешно.");
            }
            catch (Exception ex)
            {
                // Если настройка логирования не удалась, обработка ошибки
                MessageBox.Show($"Ошибка при настройке логирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Общий обработчик ошибок процессов

        private void HandleProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                // Проверка на наличие префикса ошибки
                if (e.Data.StartsWith("pg_dump: [ERROR]:", StringComparison.OrdinalIgnoreCase) ||
                    e.Data.StartsWith("pg_dump: [FATAL]:", StringComparison.OrdinalIgnoreCase) ||
                    e.Data.StartsWith("pg_dump: [PANIC]:", StringComparison.OrdinalIgnoreCase) ||
                    e.Data.StartsWith("psql: [ERROR]:", StringComparison.OrdinalIgnoreCase) ||
                    e.Data.StartsWith("psql: [FATAL]:", StringComparison.OrdinalIgnoreCase) ||
                    e.Data.StartsWith("psql: [PANIC]:", StringComparison.OrdinalIgnoreCase))
                {
                    Trace.TraceError($"ERROR: {e.Data}");
                }
                else
                {
                    // Остальные сообщения считаются информационными
                    Trace.TraceInformation(e.Data);
                }
            }
        }

        #endregion

        #region Деструктор и IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _monitoringSemaphore.Dispose();
                    Trace.TraceInformation("ViewModel disposed и мониторинг остановлен.");
                }
                _disposed = true;
            }
        }

        ~AdminPageViewModel()
        {
            Dispose(false);
        }

        #endregion
    }
}
