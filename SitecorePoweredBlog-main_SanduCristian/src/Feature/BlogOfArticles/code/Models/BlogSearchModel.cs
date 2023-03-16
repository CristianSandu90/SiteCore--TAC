using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using System;
using System.Collections.Generic;

namespace Blog.Feature.Blog.Models
{
    public class BlogSearchModel : SearchResultItem
    {
        [IndexField("_name")]
        public virtual string ItemName { get; set; }
        [IndexField("_uniqueid")]
        public virtual string ItemPath { get; set; }
        [IndexField("title_t")]
        public virtual string Title { get; set; }
        [IndexField("category_t")]
        public virtual string Category { get; set; }
        [IndexField("date_t")]
        public virtual DateTime PostDate { get; set; }
        [IndexField("smallimage")]
        public virtual string SmallImage { get; set; }
        [IndexField("bigimage")]
        public virtual string BigImage { get; set; }
        [IndexField("shorttext_t")]
        public virtual string ShortText { get; set; }
        [IndexField("longtext_t")]
        public virtual string LongText { get; set; }
        [IndexField("tags_t")]
        public virtual string Tags { get; set; }
    }
    public class SearchResult
    {
        public string ItemName { get; set; }
        public string ItemPath { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string PostDate { get; set; }
        public string SmallImage { get; set; }
        public string SmallImageAlt { get; set; }
        public string BigImage { get; set; }
        public string BigImageAlt { get; set; }
        public string ShortText { get; set; }
        public string LongText { get; set; }
        public List<string> Tags { get; set; }
        public string ReadButton { get; set; }
    }
    /// <summary>
    /// Custom search result model for binding to front end
    /// </summary>
    public class SearchResults
    {
        public List<SearchResult> Results { get; set; }
    }
}
