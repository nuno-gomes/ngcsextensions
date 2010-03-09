using System;
using System.Collections.Generic;
using System.Text;
using CommunityServer.Components;
using CommunityServer.Blogs.Components;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Web;

namespace NunoGomes.CommunityServer.Components
{
    public class WeblogRssHandler : global::CommunityServer.Blogs.Components.WeblogRssHandler
    {

        //private static Dictionary<int, KeyValuePair<int, int>> s_categoryMappings = null;
        protected override CachedFeed BuildFeed()
        {
            BaseRssWriter writer = null;
            ThreadSet blogThreads = WeblogPosts.GetBlogThreads(this.CreateQuery(), true);
            if (this.CategoryID > 0)
            {
                PostCategory category = PostCategories.GetCategory(this.CategoryID, this.CurrentWeblog.SectionID);
                if (category == null)
                {
                    string cacheKey = "WeblogRssHandler_CategoryMappings";
                    Dictionary<int, KeyValuePair<int, int>> categoryMappings = HttpRuntime.Cache.Get(cacheKey) as Dictionary<int, KeyValuePair<int, int>>;
                    if (categoryMappings == null)
                    {
                        categoryMappings = new Dictionary<int, KeyValuePair<int, int>>();

                        //var categories = from element in XElement.Load(this.Context.Server.MapPath(@"~/App_Data/CategoryTranslator.xml")).Elements("Category")
                        //                 select new
                        //                 {
                        //                     OldCategoryID = Globals.SafeInt(element.Attribute("oldID").Value, -1),
                        //                     NewSectionID = Globals.SafeInt(element.Attribute("SectionID").Value, -1),
                        //                     NewCategoryID = Globals.SafeInt(element.Attribute("newID").Value, -1)
                        //                 };

                        //// Execute the query 
                        //foreach (var cat in categories)
                        //{
                        //    if (cat.OldCategoryID != -1 && cat.NewSectionID != -1 && cat.NewCategoryID != -1)
                        //    {
                        //        s_categoryMappings.Add(cat.OldCategoryID, new KeyValuePair<int, int>(cat.NewSectionID, cat.NewCategoryID));
                        //    }
                        //}

                        //s_categoryMappings = XElement.Load(this.Context.Server.MapPath(@"~/App_Data/CategoryTranslator.xml"))
                        //                        .Elements("Category")
                        //                        .Select(p => p)
                        //                        .ToDictionary(
                        //                            p => Globals.SafeInt(p.Attribute("oldID").Value, -1), 
                        //                            p => new KeyValuePair<int, int>(
                        //                                    Globals.SafeInt(p.Attribute("SectionID").Value, -1), 
                        //                                    Globals.SafeInt(p.Attribute("newID").Value, -1))
                        //                        );


                        string path = this.Context.Server.MapPath(@"~/App_Data/CategoryTranslator.xml");

                        categoryMappings = XDocument
                            .Load(path)
                            .Root
                            .Elements("Category")
                            .ToDictionary(
                                categoryElement => XmlConvert.ToInt32(categoryElement.Attribute("oldID").Value),
                                categoryElement => new KeyValuePair<int, int>(
                                        XmlConvert.ToInt32(categoryElement.Attribute("sectionID").Value),
                                        XmlConvert.ToInt32(categoryElement.Attribute("newID").Value)));

                        HttpRuntime.Cache.Insert(cacheKey, categoryMappings, new System.Web.Caching.CacheDependency(path));

                    }
                    if (categoryMappings.ContainsKey(this.CategoryID) && categoryMappings[this.CategoryID].Key.Equals(this.CurrentWeblog.SectionID))
                    {
                        this.CategoryID = categoryMappings[this.CategoryID].Value;

                        category = PostCategories.GetCategory(this.CategoryID, this.CurrentWeblog.SectionID);

                        //this.Context.Response.StatusCode = 301;
                        //this.Context.Response.Status = "301 Permanent Redirect";
                    }
                    if (category == null)
                    {
                        this.Context.Response.StatusCode = 404;
                        this.Context.Response.Status = "404 Not Found";
                        return null;
                    }
                    blogThreads = WeblogPosts.GetBlogThreads(this.CreateQuery(), true);
                }
                writer = new WeblogCategoryRssWriter(blogThreads.Threads, this.CurrentWeblog, this.BaseUrl, category);
            }
            else if (this.Tags != null)
            {
                writer = new WeblogTagsRssWriter(blogThreads.Threads, this.CurrentWeblog, this.BaseUrl, this.Tags);
            }
            else
            {
                writer = new WeblogRssWriter(blogThreads.Threads, this.CurrentWeblog, this.BaseUrl);
            }
            return new CachedFeed((blogThreads.Threads.Count > 0) ? ((WeblogPost)blogThreads.Threads[0]).PostDate : DateTime.Now, null, writer.GetXml());
        }
    }
}
