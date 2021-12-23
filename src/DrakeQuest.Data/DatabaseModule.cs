using Autofac;
using DrakeQuest.Configuration;
using DrakeQuest.Data.Mapper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DrakeQuest.Data
{
	/// <summary>
	/// The <c>DatabaseModule</c> class is the base registration for default implementations of the listed  parameters
	/// <list type="bullet">
	/// <listheader><c>DrakeQuest.Data.Mapper</c> Interfaces Registered</listheader>
	/// <item>
	/// <term><see cref="DrakeQuest.Data.Mapper.IMapperPropertyInfo"/></term>
	/// <description>Metadata wrapper for propertyinfo and the target property
	///  See  <see cref="DrakeQuest.Data.Mapper.MapperPropertyInfo"/> for the registered implementation
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>Func&lt;PropertyInfo, IMapperPropertyInfo&gt;</c></term>
	/// <description>Delegate Factory for get a new <c>DrakeQuest.Data.Mapper.IMapperPropertyInfo</c></description>
	/// </item>
	/// <item>
	/// <term><see cref="DrakeQuest.Data.Mapper.IDataReaderObjectMapper"/></term>
	/// <description>Data mapper that will be map an object from a given DataReader
	///  See  <see cref="DrakeQuest.Data.Mapper.DataReaderMapperFactory"/> for the registered implementation
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="DrakeQuest.Data.IDatabaseProvider"/></term>
	/// <description></description>
	/// </item>
	/// <item>
	/// <term><see cref="DrakeQuest.Data.IObjectDataReader"/></term>
	/// <description>
	/// 
	///  See  <see cref="DrakeQuest.Data.ObjectDataReader"/> for the registered implementation
	/// </description>
	/// </item>
	/// </list>
	/// </summary>
	public class DatabaseModule : Autofac.Module
	{
		/// <summary>
		/// Override to add registrations to the container.
		/// </summary>
		/// <remarks>
		/// Note that the ContainerBuilder parameter is unique to this module.
		/// </remarks>
		/// <param name="builder">The builder through which components can be registered.</param>
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<DapperMapper>()
				.As<IDbDataMapper>()
				.SingleInstance();

			builder.RegisterType<MapperPropertyInfo>()
				.As<IMapperPropertyInfo>();

			builder.Register<Func<PropertyInfo, IMapperPropertyInfo>>(
			   c => (propertyInfo) => new MapperPropertyInfo(propertyInfo));

			builder.RegisterType<DataReaderObjectMapper>()
				.As<IDataReaderObjectMapper>();

			builder.RegisterType<DataReaderMapperFactory>()
				.As<IDataReaderMapperFactory>();

			//Custom logic for database provider
			builder.Register(ResolveIDatabaseProvider)
				.As<IDatabaseProvider>();

			builder.RegisterType<ObjectDataReader>()
				.AsSelf()
				.As<IObjectDataReader>();
		}

		/// <summary>
		/// Resolve the <c>IDatabaseProvider</c> depending one the parameters.The purpose will be to get the database provider depending of the configurations
		/// </summary>
		/// <param name="context">The context in which a service can be accessed or a component's dependencies resolved. Disposal of a context will dispose any owned components.</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private IDatabaseProvider ResolveIDatabaseProvider(IComponentContext context, IEnumerable<Autofac.Core.Parameter> parameters)
		{
			var environmentName = parameters.Named<string>("name");
			var connectionName = parameters.Named<string>("connectionName");


			var conProvider = context.Resolve<IConnectionProvider>();
			DatabaseProviderEnum providerEnum;
			ConnectionContext connectionContext = new ConnectionContext(
				connectionName
				, environmentName
				, conProvider.GetProviderName(environmentName, connectionName)
				, conProvider.GetConnectionString(environmentName, connectionName)
				);

			if (string.IsNullOrWhiteSpace(connectionContext.Provider))
				throw new NullReferenceException($"The provider for the connection {connectionName} is not provided");

			if (System.Enum.TryParse(connectionContext.Provider, out providerEnum))
			{
				return context.ResolveKeyed<IDatabaseProvider>(providerEnum,
					new NamedParameter("connectionContext", connectionContext));
			}
			return context.ResolveKeyed<IDatabaseProvider>(connectionContext.Provider,
				new NamedParameter("connectionContext", connectionContext));
		}
	}
}
