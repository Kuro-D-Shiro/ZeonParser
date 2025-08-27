using AngleSharp.Dom;

namespace ZeonService.Parser.Extentions
{
    public static class IElementExtentions
    {
        public static Dictionary<string, Dictionary<string, string>>? ParseSpecificationFromHtmlTable(this IElement table)
        {
            Dictionary<string, Dictionary<string, string>> specs = [];

            string currentGroup = null;

            foreach (var row in table.QuerySelectorAll("tr"))
            {
                var groupCell = row.QuerySelector("td.table-part[colspan=\"2\"]");
                if (groupCell != null)
                {
                    currentGroup = groupCell.TextContent.Trim();
                    if (!specs.ContainsKey(currentGroup))
                    {
                        specs[currentGroup] = [];
                    }
                    continue;
                }

                var cells = row.QuerySelectorAll("td").ToArray();
                if (cells.Length == 2 && currentGroup != null)
                {
                    var name = cells[0].TextContent.Trim();
                    var value = cells[1].TextContent.Trim();
                    specs[currentGroup][name] = value;
                }
            }

            return specs;
        }
    }
}
