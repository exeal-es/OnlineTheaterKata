using Logic.Entities;
using Logic.Utils;

namespace Logic.Repositories;

public class MovieRepository(UnitOfWork unitOfWork) : Repository<Movie>(unitOfWork)
{
    public IReadOnlyList<Movie> GetList()
    {
        return _unitOfWork.Query<Movie>().ToList();
    }
}