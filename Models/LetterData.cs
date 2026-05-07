namespace LetterGenerator.Models;

public sealed class LetterData
{
    public string RecipientPost { get; init; } = string.Empty;

    public string RecipientName { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public string SenderName { get; init; } = string.Empty;

    public Dictionary<string, string> ToPlaceholders()
    {
        return new Dictionary<string, string>
        {
            ["{DATE}"] = DateTime.Now.ToString("dd.MM.yyyy"),
            ["{RECIPIENT_POST}"] = RecipientPost,
            ["{RECIPIENT_NAME}"] = RecipientName,
            ["{SUBJECT}"] = Subject,
            ["{LETTER_BODY}"] = Body,
            ["{SENDER_NAME}"] = SenderName
        };
    }
}
