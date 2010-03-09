using System;
using System.Collections.Generic;
using System.Text;
using CommunityServer.Blogs.Components;
using CommunityServer.Blogs.Controls;
using CommunityServer.Components;
using CommunityServer.Controls;
using System.Configuration;

namespace NunoGomes.CommunityServer.Blogs.Controls
{
    public class WeblogPostList : global::CommunityServer.Blogs.Controls.WeblogPostList
    {
        private List<WeblogPost> _weblogPosts;
 
        public override object DataSource
        {
            get
            {
/*                BlogThreadQuery query;
                WeblogControlUtility.Instance().GetCurrentCSContext(this.Page);
                Weblog currentWeblog = WeblogControlUtility.Instance().GetCurrentWeblog(this);
                if (currentWeblog != null)
                {
                    query = BlogThreadQuery.CreateNewSingleBlogQuery(currentWeblog.SectionID);
                    if (query == null)
                    {
                        this.Context.Response.StatusCode = 500;
                        this.Context.Response.Status = "500 Internal Error";
                        this.Context.Response.End();
                        return null;
                    }
                }
                return base.DataSource;
*/

                if (_weblogPosts == null)
                {
                    BlogThreadQuery query;
                    int totalRecords;
                    WeblogControlUtility.Instance().GetCurrentCSContext(this.Page);
                    Weblog currentWeblog = WeblogControlUtility.Instance().GetCurrentWeblog(this);
                    if (currentWeblog != null)
                    {
                        query = BlogThreadQuery.CreateNewSingleBlogQuery(currentWeblog.SectionID);
                        query.PostConfig = BlogPostConfig.IsAggregated;
                        PostCategory currentPostCategory = WeblogControlUtility.Instance().GetCurrentPostCategory(this);
                        if (currentPostCategory != null)
                        {
                            query.CategoryID = currentPostCategory.CategoryID;
                        }
                    }
                    else
                    {
                        if (WeblogControlUtility.Instance().GetCurrentHub(this) != null)
                        {
                            this._weblogPosts = new List<WeblogPost>();
                            return this._weblogPosts;
                        }
                        query = BlogThreadQuery.CreateNewAggregateQuery();
                        Group currentGroup = WeblogControlUtility.Instance().GetCurrentGroup(this);
                        if (currentGroup != null)
                        {
                            query.GroupID = currentGroup.GroupID;
                        }
                    }
                    int day = Globals.SafeInt(this.Context.Request.QueryString["d"], -1);
                    int month = Globals.SafeInt(this.Context.Request.QueryString["m"], -1);
                    int year = Globals.SafeInt(this.Context.Request.QueryString["y"], -1);
                    if ((((month > 0) && (month < 13)) && ((day > 0) && (day < 0x20))) && ((year > DateTime.MinValue.Year) && (year <= DateTime.MaxValue.Year)))
                    {
                        try
                        {
                            query.DateFilter = new DateTime(year, month, day);
                        }
                        catch
                        {
                        }
                    }
                    query.PageIndex = 0;
                    query.PageSize = WeblogConfiguration.Instance().IndividualPostCount;
                    query.IncludeCategories = true;
                    query.BlogPostType = BlogPostType.Article | BlogPostType.Post;
                    string[] currentTags = WeblogControlUtility.Instance().GetCurrentTags(this);
                    if (currentTags != null)
                    {
                        query.Tags = currentTags;
                    }
                    if (this.QueryOverrides != null)
                    {
                        this.QueryOverrides.ApplyQueryOverrides(query);
                    }
                    if ((this.QueryOverrides != null) && this.QueryOverrides.HasQueryImplementation)
                    {
                        this._weblogPosts = this.QueryOverrides.QueryImplementation.GetQueryResults<WeblogPost>(query, out totalRecords);
                    }
                    else if ((this.QueryOverrides != null) && this.QueryOverrides.UsePostScoring)
                    {
                        totalRecords = 0;
                        ScoredPostList scoredBlogThreads = WeblogPosts.GetScoredBlogThreads(query, true);
                        if (scoredBlogThreads != null
                            || ConfigurationManager.AppSettings["NunoGomes.CommunityServer.AllowDataSourceException"].Equals("true", StringComparison.InvariantCultureIgnoreCase)) /*NG: Added 24/Sep/2009*/
                        {
                            this._weblogPosts = new List<WeblogPost>();
                            foreach (WeblogPost post in scoredBlogThreads.Posts)
                            {
                                this._weblogPosts.Add(post);
                            }
                            totalRecords = scoredBlogThreads.TotalRecords;
                        }
                    }
                    else
                    {
                        totalRecords = 0;
                        ThreadSet set = WeblogPosts.GetBlogThreads(query, true, (this.QueryOverrides != null) ? this.QueryOverrides.IsAggregate : false);
                        if (set != null
                            || ConfigurationManager.AppSettings["NunoGomes.CommunityServer.AllowDataSourceException"].Equals("true", StringComparison.InvariantCultureIgnoreCase)) /*NG: Added 24/Sep/2009*/
                        {
                            this._weblogPosts = new List<WeblogPost>();
                            foreach (WeblogPost post2 in set.Threads)
                            {
                                this._weblogPosts.Add(post2);
                            }
                            totalRecords = set.TotalRecords;
                        }
                    }
                    if ((this.QueryOverrides != null) && (this.QueryOverrides.Pager != null))
                    {
                        this.QueryOverrides.Pager.PageIndex = query.PageIndex;
                        this.QueryOverrides.Pager.PageSize = query.PageSize;
                        this.QueryOverrides.Pager.TotalRecords = totalRecords;
                        this.QueryOverrides.Pager.OnPageIndexChanged += new PagerEventHandler(this.PageIndexChanged);
                        this.QueryOverrides.Pager.DataBind();
                    }
                }
                return _weblogPosts;
            }
        }
    }
}
