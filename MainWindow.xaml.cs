using System.IO;
using System.Windows;
using LetterGenerator.Models;
using LetterGenerator.Services;

namespace LetterGenerator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        FillExampleData();
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        var data = new LetterData
        {
            RecipientPost = RecipientPostTextBox.Text,
            RecipientName = RecipientNameTextBox.Text,
            Subject = SubjectTextBox.Text,
            Body = BodyTextBox.Text,
            SenderName = SenderNameTextBox.Text,
            Appendices = ReadAppendices()
        };

        var outputPath = Path.Combine(AppContext.BaseDirectory, "GeneratedLetter.docx");
        DocxLetterGenerator.Generate(LetterTemplateService.TemplatePath, outputPath, data);

        StatusTextBlock.Text = $"Документ создан: {outputPath}";
        MessageBox.Show("DOCX-файл успешно сформирован.");
    }

    private List<AppendixItem> ReadAppendices()
    {
        var appendices = new List<AppendixItem>();
        var lines = AppendicesTextBox.Text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parts = line.Split('|');

            appendices.Add(new AppendixItem
            {
                Title = parts.ElementAtOrDefault(0)?.Trim() ?? "Без названия",
                Pages = parts.ElementAtOrDefault(1)?.Trim() ?? "1",
                Text = parts.ElementAtOrDefault(2)?.Trim() ?? string.Empty
            });
        }

        return appendices;
    }

    private void FillExampleData()
    {
        RecipientPostTextBox.Text = "Директору";
        RecipientNameTextBox.Text = "Петрову Петру Викторовичу";
        SubjectTextBox.Text = "лабораторной работе";
        BodyTextBox.Text = "Это письмо автоматически сформировано из DOCX-шаблона с использованием WPF и Open XML.";
        SenderNameTextBox.Text = "Иванов Роман Валерьевич";
        AppendicesTextBox.Text =
            "Причина отмены лабораторной | 3 | Здесь находится текст первого приложения." + Environment.NewLine +
            "Почему всё плохо | 2 | Здесь находится текст второго приложения.";
    }
}
