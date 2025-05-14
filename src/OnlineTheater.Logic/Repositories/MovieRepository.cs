using OnlineTheater.Logic.Entities;
using OnlineTheater.Logic.Utils;

namespace OnlineTheater.Logic.Repositories;

public class MovieRepository(UnitOfWork unitOfWork) : Repository<Movie>(unitOfWork)
{
    public IReadOnlyList<Movie> GetList()
    {
        return _unitOfWork.Query<Movie>().ToList();
    }
}