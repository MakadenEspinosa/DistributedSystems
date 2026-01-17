using Ideas.Models;

namespace Ideas.Services;

public class IdeaService
{
    private readonly List<Idea> _ideas = new();
    private int _nextId = 1;

    public IdeaService() {}

    public IEnumerable<Idea> GetAll() => _ideas;

    public Idea? GetById(int id) => _ideas.FirstOrDefault(i => i.Id == id);

    public Idea Add(Idea idea)
    {
        idea.Id = _nextId++;
        _ideas.Add(idea);
        return idea;
    }

    public bool Update(int id, Idea updatedIdea)
    {
        var idea = GetById(id);
        if (idea == null) return false;

        idea.Title = updatedIdea.Title;
        idea.Description = updatedIdea.Description;
        return true;
    }

    public bool PartialUpdate(int id, Idea updatedIdea)
    {
        var idea = GetById(id);
        if (idea == null) return false;

        if (!string.IsNullOrEmpty(updatedIdea.Title))
        {
            idea.Title = updatedIdea.Title;
        }
        if (!string.IsNullOrEmpty(updatedIdea.Description))
        {
            idea.Description = updatedIdea.Description;
        }
        return true;
    }

    public bool Delete(int id)
    {
        var idea = GetById(id);
        if (idea == null) return false;

        _ideas.Remove(idea);
        return true;
    }
}
