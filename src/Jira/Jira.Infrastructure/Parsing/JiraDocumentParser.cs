using System.Text;
using System.Text.Json;
using Jira.Infrastructure.Dtos;

namespace Jira.Infrastructure.Parsing;

internal static class JiraDocumentParser
{
    public static string? ExtractPlainText(JiraDocumentDto? doc)
    {
        if (doc is null)
        {
            return null;
        }

        var json = JsonSerializer.SerializeToElement(doc);
        return ExtractPlainText(json);
    }

    public static string? ExtractPlainText(JsonElement element)
    {
        var builder = new StringBuilder();
        AppendNodeText(element, builder);

        var text = builder.ToString().Trim();
        return string.IsNullOrWhiteSpace(text)
            ? null
            : text.Replace("\r", string.Empty)
                .Replace("\n\n\n", "\n\n")
                .Replace(" \n", "\n")
                .Replace("\n ", "\n");
    }

    private static void AppendNodeText(JsonElement element, StringBuilder builder)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                AppendObjectText(element, builder);
                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    AppendNodeText(item, builder);
                }

                break;
            case JsonValueKind.String:
                AppendText(builder, element.GetString());
                break;
        }
    }

    private static void AppendObjectText(JsonElement element, StringBuilder builder)
    {
        if (element.TryGetProperty("type", out var typeProperty) &&
            string.Equals(typeProperty.GetString(), "hardBreak", StringComparison.OrdinalIgnoreCase))
        {
            builder.AppendLine();
            return;
        }

        if (element.TryGetProperty("text", out var textProperty))
        {
            AppendText(builder, textProperty.GetString());
        }

        if (element.TryGetProperty("content", out var contentProperty) &&
            contentProperty.ValueKind == JsonValueKind.Array)
        {
            foreach (var child in contentProperty.EnumerateArray())
            {
                AppendNodeText(child, builder);
            }
        }

        if (ShouldAddBlockSeparator(element))
        {
            var trimmed = builder.ToString().TrimEnd();
            builder.Clear();
            builder.Append(trimmed);
            builder.AppendLine();
            builder.AppendLine();
        }
    }

    private static bool ShouldAddBlockSeparator(JsonElement element)
    {
        if (!element.TryGetProperty("type", out var typeProperty))
        {
            return false;
        }

        var type = typeProperty.GetString();
        return type is "paragraph" or "heading" or "blockquote" or "codeBlock" or "listItem";
    }

    private static void AppendText(StringBuilder builder, string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (builder.Length > 0 &&
            !char.IsWhiteSpace(builder[^1]) &&
            builder[^1] is not '\n')
        {
            builder.Append(' ');
        }

        builder.Append(text);
    }
}
