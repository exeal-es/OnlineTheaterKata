using FluentNHibernate.Mapping;
using OnlineTheater.Logic.Entities;

namespace OnlineTheater.Logic.Mappings;

public class MovieMap : ClassMap<Movie>
{
    public MovieMap()
    {
        Id(x => x.Id);

        Map(x => x.Name);
        Map(x => x.LicensingModel).CustomType<int>();
    }
}