using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
    public interface ITagRepository
    {
        // Interfaces only have the definition of the methods. They do not have the implementation of the methods.
        // In here we want to create definitions of how are we going to access the database and the Tags table from the database.
        // That means we need to perfrom CRUD operations on the Tags table.
        // So, we want to have methods fro getting all the tags, getting a single tag, creating a tag, updating a tag and deleting a tag.
        // We can implement asynchronous programming over here as well.
        Task<IEnumerable<Tag>> GetAllAsync(string? searchQuery = null, string? sortBy = null, string? sortDirection = null, int pageSize = 100 ,int pageNumber = 1);
        Task<Tag?> GetAsync(Guid id);
        Task<Tag> AddAsync(Tag tag);
        // Id found or not found. (Nullable)
        Task<Tag?> UpdateAsync(Tag tag);
        // Id found or not found return null. (Nullable)
        Task<Tag?> DeleteAsync(Guid id);
        Task<int> CountAsync();
    }
}
