namespace Cells.Common.Spreadsheet
{
    public static class Helpers
    {
        public static bool IsExpression(this string text)
        {
            return !string.IsNullOrWhiteSpace(text) && text.StartsWith("=") && text.Trim().Length > 1;
        }
    }
}
