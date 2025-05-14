using OnlineTheater.Logic.Entities;
using OnlineTheater.Logic.Data;

namespace OnlineTheater.Logic.Repositories;

public class MovieRepository(OnlineTheaterDbContext context)
{
    public IReadOnlyList<Movie> GetList()
    {
        return context.Movies.ToList();
    }

    public Movie GetById(long movieId)
    {
        return context.Movies.Find(movieId);
    }
}
