using FluentNHibernate.Mapping;
using OnlineTheater.Logic.Entities;

namespace OnlineTheater.Logic.Mappings;

public class PurchasedMovieMap : ClassMap<PurchasedMovie>
{
    public PurchasedMovieMap()
    {
        Id(x => x.Id);

        Map(x => x.Price);
        Map(x => x.PurchaseDate);
        Map(x => x.ExpirationDate).Nullable();
        Map(x => x.MovieId);
        Map(x => x.CustomerId);

        References(x => x.Movie).LazyLoad(Laziness.False).ReadOnly();
    }
}