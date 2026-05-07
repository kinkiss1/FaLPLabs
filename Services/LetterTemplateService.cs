using System.IO;

namespace LetterGenerator.Services;

public static class LetterTemplateService
{
    public static readonly string TemplatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Template.docx");
}
