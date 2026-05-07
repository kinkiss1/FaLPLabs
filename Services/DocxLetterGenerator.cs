using System.IO;
using DocumentFormat.OpenXml.Packaging;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace LetterGenerator.Services;

public static class DocxLetterGenerator
{
    public static void Generate(string templatePath, string outputPath, Dictionary<string, string> placeholders)
    {
        File.Copy(templatePath, outputPath, overwrite: true);

        using var document = WordprocessingDocument.Open(outputPath, true);
        var mainPart = document.MainDocumentPart!;

        ReplaceText(mainPart.Document!.Descendants<W.Text>(), placeholders);

        foreach (var headerPart in mainPart.HeaderParts)
        {
            ReplaceText(headerPart.RootElement!.Descendants<W.Text>(), placeholders);
        }

        foreach (var footerPart in mainPart.FooterParts)
        {
            ReplaceText(footerPart.RootElement!.Descendants<W.Text>(), placeholders);
        }

        mainPart.Document.Save();
    }

    private static void ReplaceText(IEnumerable<W.Text> texts, Dictionary<string, string> placeholders)
    {
        foreach (var text in texts)
        {
            foreach (var placeholder in placeholders)
            {
                text.Text = text.Text.Replace(placeholder.Key, placeholder.Value);
            }
        }
    }
}
