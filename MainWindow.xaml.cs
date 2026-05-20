using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LetterGenerator.Models;
using LetterGenerator.Services;

namespace LetterGenerator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateForm())
        {
            return;
        }

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

    private void AddAppendixButton_Click(object sender, RoutedEventArgs e)
    {
        AddAppendixBlock();
    }

    private void RequiredTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.BorderBrush = SystemColors.ControlDarkBrush;
        }
    }

    private bool ValidateForm()
    {
        var errors = new List<string>();

        CheckRequired(RecipientNameTextBox, "ФИО адресата", errors);
        CheckRequired(SubjectTextBox, "Тема письма", errors);
        CheckRequired(BodyTextBox, "Текст письма", errors);
        CheckRequired(SenderNameTextBox, "ФИО отправителя", errors);

        if (errors.Count == 0)
        {
            return true;
        }

        var message = "Заполните обязательные поля:\n" + string.Join('\n', errors);
        StatusTextBlock.Text = "Не все обязательные поля заполнены.";
        MessageBox.Show(message, "Проверка данных");
        return false;
    }

    private static void CheckRequired(TextBox textBox, string fieldName, List<string> errors)
    {
        if (!string.IsNullOrWhiteSpace(textBox.Text))
        {
            return;
        }

        textBox.BorderBrush = Brushes.Red;
        errors.Add($"- {fieldName}");
    }

    private List<AppendixItem> ReadAppendices()
    {
        var appendices = new List<AppendixItem>();

        foreach (var child in AppendicesPanel.Children)
        {
            if (child is not Border border || border.Child is not StackPanel panel)
            {
                continue;
            }

            var titleTextBox = (TextBox)panel.Children[1];
            var pagesTextBox = (TextBox)panel.Children[3];
            var textTextBox = (TextBox)panel.Children[5];

            if (string.IsNullOrWhiteSpace(titleTextBox.Text) &&
                string.IsNullOrWhiteSpace(textTextBox.Text))
            {
                continue;
            }

            appendices.Add(new AppendixItem
            {
                Title = string.IsNullOrWhiteSpace(titleTextBox.Text) ? "Без названия" : titleTextBox.Text,
                Pages = string.IsNullOrWhiteSpace(pagesTextBox.Text) ? "1" : pagesTextBox.Text,
                Text = textTextBox.Text
            });
        }

        return appendices;
    }

    private void AddAppendixBlock(string title = "", string pages = "1", string text = "")
    {
        var block = new Border
        {
            BorderBrush = Brushes.LightGray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12),
            Margin = new Thickness(0, 0, 0, 10),
            Background = Brushes.WhiteSmoke
        };

        var panel = new StackPanel();

        panel.Children.Add(new TextBlock { Text = "Заголовок приложения:" });
        panel.Children.Add(new TextBox { Text = title });

        panel.Children.Add(new TextBlock { Text = "Количество листов:" });
        panel.Children.Add(new TextBox { Text = pages });

        panel.Children.Add(new TextBlock { Text = "Текст приложения:" });
        panel.Children.Add(new TextBox
        {
            Text = text,
            Height = 70,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        });

        block.Child = panel;
        AppendicesPanel.Children.Add(block);
    }
}
