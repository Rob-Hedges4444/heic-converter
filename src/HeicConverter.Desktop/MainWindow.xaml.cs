using HeicConverter.Core;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace HeicConverter.Desktop;

public partial class MainWindow : Window
{
    private readonly List<string> _files = [];
    private readonly IImageConverter _converter = new HeicImageConverter();
    private string? _outputDirectory;

    public MainWindow()
    {
        InitializeComponent();
        if (ManagedOutputDirectory.TryGet(out var directory, out var error))
        {
            _outputDirectory = directory;
            StatusText.Text = $"Approved output location: {_outputDirectory}";
        }
        else
        {
            StatusText.Text = error + " Contact IT; conversion is disabled.";
        }
        UpdateUi();
    }

    private void ChooseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "HEIC images (*.heic;*.heif)|*.heic;*.heif", Multiselect = true };
        if (dialog.ShowDialog() == true) AddFiles(dialog.FileNames);
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _files.Clear();
        StatusText.Text = string.Empty;
        UpdateUi();
    }

    private void DropArea_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void DropArea_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] paths) AddFiles(paths);
    }

    private void AddFiles(IEnumerable<string> paths)
    {
        var additions = paths.Where(IsSupported).Where(p => !_files.Contains(p, StringComparer.OrdinalIgnoreCase)).ToList();
        _files.AddRange(additions);
        StatusText.Text = additions.Count == 0 ? "No new HEIC or HEIF files were added." : $"{_files.Count} image(s) ready. Output: {_outputDirectory}";
        UpdateUi();
    }

    private async void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        var outputDirectory = _outputDirectory;
        if (_files.Count == 0 || outputDirectory is null) return;
        SetBusy(true);
        var completed = 0;
        var failures = new List<string>();
        try
        {
            foreach (var file in _files.ToArray())
            {
                StatusText.Text = $"Converting {++completed} of {_files.Count}: {Path.GetFileName(file)}";
                try { await _converter.ConvertToJpegAsync(file, outputDirectory, (int)QualitySlider.Value); }
                catch (Exception ex) { failures.Add($"{Path.GetFileName(file)}: {ex.Message}"); }
            }
            StatusText.Text = failures.Count == 0
                ? $"Done — {_files.Count} JPEG(s) saved to {outputDirectory}."
                : $"Finished with {failures.Count} failure(s): {string.Join("; ", failures)}";
        }
        finally { SetBusy(false); }
    }

    private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
    {
        if (_outputDirectory is not null)
            Process.Start(new ProcessStartInfo { FileName = _outputDirectory, UseShellExecute = true });
    }

    private void UpdateUi()
    {
        FilesList.ItemsSource = null;
        FilesList.ItemsSource = _files.Select(Path.GetFileName).ToList();
        PlaceholderText.Visibility = _files.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ConvertButton.IsEnabled = _files.Count > 0 && _outputDirectory is not null;
        ClearButton.IsEnabled = _files.Count > 0;
        OpenFolderButton.IsEnabled = _outputDirectory is not null;
    }

    private void SetBusy(bool isBusy)
    {
        ChooseButton.IsEnabled = !isBusy;
        ClearButton.IsEnabled = !isBusy && _files.Count > 0;
        ConvertButton.IsEnabled = !isBusy && _files.Count > 0 && _outputDirectory is not null;
        OpenFolderButton.IsEnabled = !isBusy && _outputDirectory is not null;
        QualitySlider.IsEnabled = !isBusy;
        Cursor = isBusy ? System.Windows.Input.Cursors.Wait : System.Windows.Input.Cursors.Arrow;
    }

    private static bool IsSupported(string path) => File.Exists(path) &&
        (Path.GetExtension(path).Equals(".heic", StringComparison.OrdinalIgnoreCase) ||
         Path.GetExtension(path).Equals(".heif", StringComparison.OrdinalIgnoreCase));
}
