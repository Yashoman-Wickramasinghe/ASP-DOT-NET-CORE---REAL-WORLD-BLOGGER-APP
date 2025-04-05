namespace Bloggie.Web.Models.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        //This Tag has multiple BlogPosts
        public ICollection<BlogPost> BlogPosts { get; set; }
    }
}
