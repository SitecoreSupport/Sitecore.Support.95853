using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Telerik.Web.UI;
using Telerik.Web.UI.Widgets;

namespace Sitecore.Support.Shell.Controls.RichTextEditor
{
    class DBContentProvider : Sitecore.Shell.Controls.RichTextEditor.DBContentProvider
    {
        public DBContentProvider(HttpContext context, string[] searchPatterns, string[] viewPaths, string[] uploadPaths, string[] deletePaths, string selectedUrl, string selectedItemTag) : base(context, searchPatterns, viewPaths, uploadPaths, deletePaths, selectedUrl, selectedItemTag)
        {
        }

        private static DirectoryItem[] GetChildDirectories(Item item)
        {
            Assert.ArgumentNotNull(item, "item");

            List<Item> array = new List<Item>();
            foreach (Item item2 in item.Children)
            {
                if (item2.TemplateID == TemplateIDs.MediaFolder || item2.Template.BaseTemplates.Any((TemplateItem baseTemplate) => baseTemplate.ID == TemplateIDs.MediaFolder))
                {
                    array.Add(item2);
                }
            }

            if (array == null || array.Count == 0)
            {
                return new DirectoryItem[0];
            }
            return (from child in array
                    let permissions = DBContentProvider.GetPermissions(child)
                    select new DirectoryItem(child.GetUIDisplayName(), string.Empty, DBContentProvider.GetPath(child), string.Empty, permissions, null, null)).ToArray<DirectoryItem>();
        }

        private static FileItem[] GetChildFiles(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            List<FileItem> list = new List<FileItem>();

            foreach (Item item2 in item.Children)
            {
                if (!(item2.TemplateID == TemplateIDs.MediaFolder || item2.Template.BaseTemplates.Any((TemplateItem baseTemplate) => baseTemplate.ID == TemplateIDs.MediaFolder)))
                {
                    PathPermissions permissions = DBContentProvider.GetPermissions(item2);
                    MediaItem mediaItem = item2;
                    MediaUrlOptions shellOptions = MediaUrlOptions.GetShellOptions();
                    string mediaUrl = MediaManager.GetMediaUrl(item2, shellOptions);
                    FileItem item3 = new FileItem(item2.Name + "." + mediaItem.Extension, mediaItem.Extension, mediaItem.Size, string.Empty, mediaUrl, string.Empty, permissions);
                    list.Add(item3);
                }
            }
            return list.ToArray();
        }

        private static Item GetItem(string path)
        {
            Assert.ArgumentNotNull(path, "path");
            if ((path == string.Empty) || (path == "/"))
            {
                path = "/media library";
            }
            if (!path.StartsWith("/sitecore", StringComparison.InvariantCulture))
            {
                path = "/sitecore" + path;
            }
            return Sitecore.Context.ContentDatabase?.GetItem(path);
        }

        private string GetLocation(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            if (string.Compare(item.Paths.FullPath, "/sitecore/media library") != 0)
            {
                return string.Empty;
            }
            return item.Paths.FullPath;
        }

        private static string GetPath(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return item.Paths.FullPath;
        }

        private static PathPermissions GetPermissions(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            PathPermissions pathPermissions = (PathPermissions)0;
            if (item.Access.CanRead())
            {
                pathPermissions |= PathPermissions.Read;
            }
            if (item.Access.CanDelete())
            {
                pathPermissions |= PathPermissions.Delete;
            }
            if (item.Access.CanCreate())
            {
                pathPermissions |= PathPermissions.Upload;
            }
            return pathPermissions;
        }

        public override DirectoryItem ResolveRootDirectoryAsTree(string path)
        {
            Assert.ArgumentNotNull(path, "path");
            Sitecore.Data.Items.Item item = GetItem(path);
            if (item == null)
            {
                return null;
            }
            return new DirectoryItem(item.GetUIDisplayName(), this.GetLocation(item), GetPath(item), string.Empty, GetPermissions(item), GetChildFiles(item), GetChildDirectories(item));
        }

        public override DirectoryItem ResolveDirectory(string path)
        {
            Assert.ArgumentNotNull(path, "path");
            Sitecore.Data.Items.Item item = GetItem(path);
            if (item == null)
            {
                return null;
            }
            DirectoryItem[] childDirectories = GetChildDirectories(item);
            return new DirectoryItem(item.GetUIDisplayName(), string.Empty, GetPath(item), string.Empty, GetPermissions(item), GetChildFiles(item), childDirectories);
        }
    }
}
