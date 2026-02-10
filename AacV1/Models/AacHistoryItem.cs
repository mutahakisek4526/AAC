namespace AacV1.Models;

public class AacHistoryItem
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Text { get; set; } = string.Empty;
    public int UseCount { get; set; }
}
