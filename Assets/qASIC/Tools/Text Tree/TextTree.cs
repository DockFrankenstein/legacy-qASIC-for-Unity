using System.Collections.Generic;

namespace qASIC.Tools
{
    public class TextTree
    {
        public TextTreeStyle Style { get; private set; } = new TextTreeStyle();

        public TextTree() { }

        public TextTree(string middleItemBranch, string lastItemBranch, string space, string verticalBranch) =>
            Style = new TextTreeStyle(middleItemBranch, lastItemBranch, space, verticalBranch);

        public TextTree(TextTreeStyle style) =>
            Style = style;

        public string GenerateTree(TextTreeItem list) =>
            GenerateItem(list, "", false, true);

        public string GenerateItem(TextTreeItem item, string indent, bool isLast, bool isFirst = false)
        {
            string text = indent;

            if (!isFirst)
            {
                text += isLast ? Style.Last : Style.Middle;
                indent += isLast ? Style.Space : Style.Vertical;
            }

            text += $"{item.Text}\n";

            int childCount = item.children.Count;
            for (int i = 0; i < childCount; i++)
                text += GenerateItem(item.children[i], indent, i == childCount - 1);

            return text;
        }
    }

    public class TextTreeItem
    {
        public string Text { get; set; }
        public List<TextTreeItem> children = new List<TextTreeItem>();

        public TextTreeItem() { }

        public TextTreeItem(string text)
        {
            Text = text;
        }

        public void Add(TextTreeItem item) =>
            children?.Add(item);

        public void Add(string text) =>
            Add(new TextTreeItem(text));
    }
}