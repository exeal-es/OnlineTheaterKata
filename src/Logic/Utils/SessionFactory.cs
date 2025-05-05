using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;
using NHibernate;

namespace Logic.Utils;

public class SessionFactory(string connectionString)
{
    private readonly ISessionFactory _factory = BuildSessionFactory(connectionString);

    internal ISession OpenSession()
    {
        return _factory.OpenSession();
    }

    private static ISessionFactory BuildSessionFactory(string connectionString)
    {
        FluentConfiguration configuration = Fluently.Configure()
            .Database(SQLiteConfiguration.Standard.ConnectionString(connectionString))
            .Mappings(m => m.FluentMappings
                .AddFromAssembly(Assembly.GetExecutingAssembly())
                .Conventions.Add(
                    ForeignKey.EndsWith("ID"),
                    ConventionBuilder.Property.When(criteria => criteria.Expect(x => x.Nullable, Is.Not.Set), x => x.Not.Nullable()))
                .Conventions.Add<OtherConversions>()
                .Conventions.Add<TableNameConvention>()
                .Conventions.Add<HiLoConvention>()
            );

        var sessionFactory = configuration.BuildSessionFactory();

        // Create the database schema
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            var schema = new NHibernate.Tool.hbm2ddl.SchemaExport(configuration.BuildConfiguration());
            schema.Execute(true, true, false, session.Connection, null);
            transaction.Commit();
        }

        return sessionFactory;
    }


    private class OtherConversions : IHasManyConvention, IReferenceConvention
    {
        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.LazyLoad();
            instance.AsBag();
            instance.Cascade.SaveUpdate();
            instance.Inverse();
        }

        public void Apply(IManyToOneInstance instance)
        {
            instance.LazyLoad(Laziness.Proxy);
            instance.Cascade.None();
            instance.Not.Nullable();
        }
    }


    public class TableNameConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name);
        }
    }


    public class HiLoConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.Column(instance.EntityType.Name + "ID");
            instance.GeneratedBy.Native();
        }
    }
}