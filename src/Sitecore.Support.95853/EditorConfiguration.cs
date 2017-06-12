using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Globalization;
using Sitecore.IO;
using Sitecore.Reflection;
using Sitecore.Resources;
using Sitecore.Security.Authentication;
using Sitecore.Web.UI;
using Sitecore.Xml;
using System;
using System.Globalization;
using System.Xml;
using Telerik.Web.UI;

namespace Sitecore.Support.Shell.Controls.RichTextEditor
{
    public class EditorConfiguration : Sitecore.Shell.Controls.RichTextEditor.EditorConfiguration
    {
        public EditorConfiguration(Item profile) : base(profile)
        {
        }

        protected override void SetupEditor()
        {
            if (string.Compare(Settings.HtmlEditor.LineBreak, "br", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                this.Editor.NewLineMode = EditorNewLineModes.Br;
            }
            else if (string.Compare(Settings.HtmlEditor.LineBreak, "p", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                this.Editor.NewLineMode = EditorNewLineModes.P;
            }
            else
            {
                this.Editor.NewLineMode = EditorNewLineModes.Div;
            }
            this.Editor.FlashManager.ContentProviderTypeName = typeof(Sitecore.Support.Shell.Controls.RichTextEditor.DBContentProvider).AssemblyQualifiedName;
            this.Editor.MediaManager.ContentProviderTypeName = typeof(Sitecore.Support.Shell.Controls.RichTextEditor.DBContentProvider).AssemblyQualifiedName;
            this.Editor.DocumentManager.ContentProviderTypeName = typeof(Sitecore.Support.Shell.Controls.RichTextEditor.DBContentProvider).AssemblyQualifiedName;
            this.Editor.ImageManager.ContentProviderTypeName = typeof(Sitecore.Support.Shell.Controls.RichTextEditor.DBContentProvider).AssemblyQualifiedName;
            this.Editor.Style["padding"] = "0px";
            this.Editor.Style["border"] = "none";
            this.Editor.Style["margin"] = "0px";
            AuthenticationHelper.SaveActiveUser("sc_rte_shuser", true);
        }

        private static void SetChildren(EditorDropDownItemCollection items, Item parent)
        {
            Assert.ArgumentNotNull(items, "items");
            Assert.ArgumentNotNull(parent, "parent");
            foreach (Item item in parent.Children)
            {
                if (item.TemplateName == "Html Editor List Item")
                {
                    items.Add(item["Header"], item["Value"]);
                }
            }
        }

        private void SetProperties(EditorTool button, Item child)
        {
            Assert.ArgumentNotNull(button, "button");
            Assert.ArgumentNotNull(child, "child");
            string text = child["Shortcut"];
            if (!string.IsNullOrEmpty(text))
            {
                button.ShortCut = text;
            }
            if (!string.IsNullOrEmpty(child[FieldIDs.DisplayName]))
            {
                button.Text = child.GetUIDisplayName();
            }
            else if (string.IsNullOrEmpty(this.Editor.Localization.Tools.GetString(child["Click"])))
            {
                button.Text = child.Name;
            }
            if (child["Width"].Length > 0)
            {
                button.PopUpWidth = child["Width"];
            }
            if (child["Height"].Length > 0)
            {
                button.PopUpHeight = child["Height"];
            }
        }

        private static void SetSize(EditorTool button, Item child)
        {
            Assert.ArgumentNotNull(button, "button");
            Assert.ArgumentNotNull(child, "child");
            if (child["Width"].Length > 0)
            {
                button.PopUpWidth = child["Width"];
            }
            if (child["Height"].Length > 0)
            {
                button.PopUpHeight = child["Height"];
            }
        }

    }
}
