namespace LetterGenerator.Models;

public sealed class LetterData
{
    public string RecipientPost { get; init; } = string.Empty;

    public string RecipientName { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public string SenderName { get; init; } = string.Empty;

    public List<AppendixItem> Appendices { get; init; } = new();

    public Dictionary<string, string> ToPlaceholders()
    {
        return new Dictionary<string, string>
        {
            ["{DATE}"] = DateTime.Now.ToString("dd.MM.yyyy"),
            ["{RECIPIENT_POST}"] = RecipientPost,
            ["{RECIPIENT_NAME}"] = RecipientName,
            ["{SUBJECT}"] = Subject,
            ["{LETTER_BODY}"] = Body,
            ["{SENDER_NAME}"] = SenderName,
            ["{APPENDIX_LIST}"] = BuildAppendixList()
        };
    }

    private string BuildAppendixList()
    {
        if (Appendices.Count == 0)
        {
            return string.Empty;
        }

        var lines = Appendices
            .Select((appendix, index) => $"{index + 1}. {appendix.Title} на {appendix.Pages} л.");

        return "Приложения:\n" + string.Join('\n', lines);
    }
}
