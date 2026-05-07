using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using LetterGenerator.Models;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace LetterGenerator.Services;

public static class DocxLetterGenerator
{
    public static void Generate(string templatePath, string outputPath, LetterData data)
    {
        File.Copy(templatePath, outputPath, overwrite: true);

        using var document = WordprocessingDocument.Open(outputPath, true);
        var mainPart = document.MainDocumentPart!;
        var placeholders = data.ToPlaceholders();

        ReplaceText(mainPart.Document!.Descendants<W.Text>(), placeholders);

        foreach (var headerPart in mainPart.HeaderParts)
        {
            ReplaceText(headerPart.RootElement!.Descendants<W.Text>(), placeholders);
        }

        foreach (var footerPart in mainPart.FooterParts)
        {
            ReplaceText(footerPart.RootElement!.Descendants<W.Text>(), placeholders);
        }

        AddAppendices(mainPart.Document.Body!, data.Appendices);
        mainPart.Document.Save();
    }

    private static void ReplaceText(IEnumerable<W.Text> texts, Dictionary<string, string> placeholders)
    {
        foreach (var text in texts.ToList())
        {
            var newText = text.Text;

            foreach (var placeholder in placeholders)
            {
                newText = newText.Replace(placeholder.Key, placeholder.Value);
            }

            if (newText == text.Text)
            {
                continue;
            }

            var run = text.Parent as W.Run;
            if (run is null)
            {
                text.Text = newText;
                continue;
            }

            run.RemoveAllChildren<W.Text>();
            run.RemoveAllChildren<W.Break>();
            AddTextWithLineBreaks(run, newText);
        }
    }

    private static void AddAppendices(W.Body body, List<AppendixItem> appendices)
    {
        for (var i = 0; i < appendices.Count; i++)
        {
            var appendix = appendices[i];
            var appendixName = appendices.Count == 1 ? "Приложение" : $"Приложение {i + 1}";

            AddBeforeSectionProperties(body, PageBreak());
            AddBeforeSectionProperties(body, Paragraph(appendixName, W.JustificationValues.Right));
            AddBeforeSectionProperties(body, Paragraph(appendix.Title, W.JustificationValues.Center));
            AddBeforeSectionProperties(body, Paragraph(appendix.Text, W.JustificationValues.Both));
        }
    }

    private static void AddBeforeSectionProperties(W.Body body, OpenXmlElement element)
    {
        var sectionProperties = body.Elements<W.SectionProperties>().LastOrDefault();

        if (sectionProperties is null)
        {
            body.Append(element);
        }
        else
        {
            body.InsertBefore(element, sectionProperties);
        }
    }

    private static W.Paragraph PageBreak()
    {
        return new W.Paragraph(new W.Run(new W.Break { Type = W.BreakValues.Page }));
    }

    private static W.Paragraph Paragraph(string text, W.JustificationValues justification)
    {
        var paragraph = new W.Paragraph(
            new W.ParagraphProperties(new W.Justification { Val = justification }),
            new W.Run());

        AddTextWithLineBreaks(paragraph.GetFirstChild<W.Run>()!, text);
        return paragraph;
    }

    private static void AddTextWithLineBreaks(W.Run run, string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        for (var i = 0; i < lines.Length; i++)
        {
            if (i > 0)
            {
                run.Append(new W.Break());
            }

            run.Append(new W.Text(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
        }
    }
}
