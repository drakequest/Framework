using Autofac;

namespace DrakeQuest.Data.SqlServer
{
    public class DatabaseModule : Data.DatabaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);
            /** Database default configuration **/
            builder.RegisterType<SqlServerClient>()
                .Keyed<IDatabaseProvider>(DatabaseProviderEnum.SqlServerClient);
        }
	}
}
