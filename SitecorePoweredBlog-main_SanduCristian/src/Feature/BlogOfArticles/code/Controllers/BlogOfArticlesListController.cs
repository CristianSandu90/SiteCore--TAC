using Blog.Feature.Blog.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Blog.Feature.Blog.Controllers
{
    public class BlogOfArticlesListController : Controller
    {
        // GET: BlogList
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DoSearch(string searchText)
        {

            var myResults = new SearchResults
            {
                Results = new List<SearchResult>()
            };
            var searchIndex = ContentSearchManager.GetIndex("sitecore_web_index"); // Get the search index
            var searchPredicate = GetSearchPredicate(searchText); // Build the search predicate
            using (var searchContext = searchIndex.CreateSearchContext()) // Get a context of the search index
            {
                //Select * from Sitecore_web_index Where Author="searchText" OR Description="searchText" OR Title="searchText"
                //var searchResults = searchContext.GetQueryable<SearchModel>().Where(searchPredicate); // Search the index for items which match the predicate
                var searchResults = searchContext.GetQueryable<BlogSearchModel>()
                    .Where(x => x.TemplateName == "BlogArticle" && x.ItemName != "Home" && x.ItemName != "__Standard Values" && x.ItemName != "404" && (x.Title.Contains(searchText) || x.Category.Contains(searchText) || x.ShortText.Contains(searchText) || x.LongText.Contains(searchText) || x.Tags.Contains(searchText)));   //LINQ query

                var fullResults = searchResults.GetResults();

                // This is better and will get paged results - page 1 with 10 results per page
                //var pagedResults = searchResults.Page(1, 10).GetResults();
                foreach (var hit in fullResults.Hits)
                {
                    var item_uniqueid = hit.Document.ItemPath;
                    var cutFrom = item_uniqueid.IndexOf("{");
                    var cutTo = item_uniqueid.LastIndexOf("}") + 1;
                    var item_id = item_uniqueid.Substring(cutFrom, cutTo - cutFrom);

                    var item = Sitecore.Context.Database.GetItem(item_id);

                    Sitecore.Data.Fields.ImageField small_image = item.Fields["SmallImage"];
                    Sitecore.Data.Fields.ImageField big_image = item.Fields["BigImage"];
                    Sitecore.Data.Items.MediaItem small_img = new Sitecore.Data.Items.MediaItem(small_image.MediaItem);
                    string small_img_src = Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(small_img));
                    string small_img_alt = small_img.Alt;
                    Sitecore.Data.Items.MediaItem big_img = new Sitecore.Data.Items.MediaItem(big_image.MediaItem);
                    string big_img_src = Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(big_img));
                    string big_img_alt = big_img.Alt;

                    string category = "";
                    Sitecore.Data.Fields.ReferenceField droplinkField = item.Fields["Category"];
                    if (droplinkField != null && droplinkField.TargetItem != null)
                    {
                        Sitecore.Data.Items.Item targetItem = droplinkField.TargetItem; // here targetietm is the value in the Droplink field "Title" 
                        category = targetItem.Name;
                    }
                    List<string> tags = new List<string>();
                    Sitecore.Data.Fields.MultilistField multilistField = item.Fields["Tags"];
                    if (multilistField != null)
                    {
                        foreach (Sitecore.Data.Items.Item tempItem in multilistField.GetItems())
                        {
                            tags.Add(tempItem.Name);
                        }
                    }

                    string item_url = Sitecore.Links.LinkManager.GetItemUrl(item);

                    string read_button = Sitecore.Globalization.Translate.Text("ArticleReadButtonLabel");

                    myResults.Results.Add(new SearchResult
                    {
                        ReadButton = read_button,
                        Tags = tags,
                        LongText = hit.Document.LongText,
                        ShortText = hit.Document.ShortText,
                        BigImageAlt = big_img_alt,
                        BigImage = big_img_src,
                        SmallImageAlt = small_img_alt,
                        SmallImage = small_img_src,
                        PostDate = hit.Document.PostDate.ToString("MMM d, yyyy"),
                        Category = category,
                        Title = hit.Document.Title,
                        ItemPath = item_url,
                    });
                }
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = myResults };
            }
        }

        /// <summary>
        /// Search logic
        /// </summary>
        /// <param name="searchText">Search term</param>
        /// <returns>Search predicate object</returns>
        public static Expression<Func<BlogSearchModel, bool>> GetSearchPredicate(string searchText)
        {
            var predicate = PredicateBuilder.True<BlogSearchModel>(); // Items which meet the predicate
                                                                  // Search the whole phrase - LIKE
                                                                  // predicate = predicate.Or(x => x.DispalyName.Like(searchText)).Boost(1.2f);
                                                                  // predicate = predicate.Or(x => x.Description.Like(searchText)).Boost(1.2f);
                                                                  // predicate = predicate.Or(x => x.Title.Like(searchText)).Boost(1.2f);
                                                                  // Search the whole phrase - CONTAINS
            predicate = predicate.Or(x => x.LongText.Contains(searchText)); // .Boost(2.0f);
            predicate = predicate.Or(x => x.ShortText.Contains(searchText)); // .Boost(2.0f);
            predicate = predicate.Or(x => x.Category.Contains(searchText)); // .Boost(2.0f);
            predicate = predicate.Or(x => x.Title.Contains(searchText)); // .Boost(2.0f);
            //Where Author="searchText" OR Description="searchText" OR Title="searchText"
            return predicate;
        }
    }
}