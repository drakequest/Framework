using Autofac;

namespace DrakeQuest.Data.Oracle
{
  public class DatabaseModule : Data.DatabaseModule
  {
    protected override void Load(ContainerBuilder builder)
    {
      base.Load(builder);

#if !NETCOREAPP
      builder.RegisterType<Oracle.OracleClient>()
            .Keyed<IDatabaseProvider>(DatabaseProviderEnum.OracleClient);
      builder.RegisterType<Oracle.ManagedOracleClient>()
          .Keyed<IDatabaseProvider>(DatabaseProviderEnum.ManagedOracleClient);
#else
      builder.RegisterType<Oracle.ManagedOracleClient>()
                .Keyed<IDatabaseProvider>(DatabaseProviderEnum.OracleClient)
                .Keyed<IDatabaseProvider>(DatabaseProviderEnum.ManagedOracleClient);
#endif
    }
  }
}
