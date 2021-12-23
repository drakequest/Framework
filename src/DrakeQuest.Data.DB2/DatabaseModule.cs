using Autofac;

namespace DrakeQuest.Data.DB2
{
    public class DatabaseModule : Data.DatabaseModule
    {
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

            builder.RegisterType<DB2Client>()
                .Keyed<IDatabaseProvider>(DatabaseProviderEnum.DB2)
                .Keyed<IDatabaseProvider>("IBM.Data.DB2");
        }
	}
}
