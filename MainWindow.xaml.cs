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
            SenderName = SenderNameTextBox.Text
        };

        var outputPath = Path.Combine(AppContext.BaseDirectory, "GeneratedLetter.docx");
        DocxLetterGenerator.Generate(LetterTemplateService.TemplatePath, outputPath, data.ToPlaceholders());

        StatusTextBlock.Text = $"Документ создан: {outputPath}";
        MessageBox.Show("DOCX-файл успешно сформирован.");
    }

    private void FillExampleData()
    {
        RecipientPostTextBox.Text = "Директору";
        RecipientNameTextBox.Text = "Петрову Петру Викторовичу";
        SubjectTextBox.Text = "лабораторной работе";
        BodyTextBox.Text = "Это письмо автоматически сформировано из DOCX-шаблона с использованием WPF и Open XML.";
        SenderNameTextBox.Text = "Иванов Роман Валерьевич";
    }
}
