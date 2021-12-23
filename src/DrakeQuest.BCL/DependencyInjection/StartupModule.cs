using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using DrakeQuest.Configuration;
using DrakeQuest.IO;
using DrakeQuest.Log.Generic;
using System.IO;

namespace DrakeQuest.DependencyInjection
{
	/// <summary>
	/// The <c>StartupModule</c> class is a <c>Autofac</c> module implementation, that will register all the required implementation for the base services
	/// </summary>
	/// <summary>
	/// Below the list of the implementation registered by default if there is not implementation registered before
	/// <list type="table">
	/// <listheader>  
	/// <term><see cref="Configuration"/></term>  
	/// <description><c>DrakeQuest.Configuration</c> Interfaces Registered</description>  
	///</listheader>
	/// <item>
	/// <term><see cref="IConfigurationProvider"/></term>
	/// <description>Facade that will be responsible to provide application configuration<br />
	///  See <see cref="ConfigurationManagerProvider"/> for the registered implementation
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="IConnectionProvider"/></term>
	/// <description>Facade that will be responsible to provide Connection information<br />
	///  See <see cref="ConnectionProvider"/> for the registered implementation
	/// </description>
	/// </item>
	/// </list>
	/// <br />
	/// <list type="table">
	/// <listheader>  
	/// <term><see cref="IO"/></term>  
	/// <description><c>DrakeQuest.Configuration</c> Interfaces Registered</description>  
	///</listheader>
	/// <item>
	/// <term><see cref="IDirectoryService"/></term>
	/// <description>Facade for the default io directory orperation
	///  See <see cref="DirectoryService"/> for the registered implementation
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="IFileService"/></term>
	/// <description>Facade for the default io file orperation
	///  See <see cref="IFileService"/> for the registered implementation
	/// </description>
	/// </item>
	/// </list>
	/// <list type="table">
	/// <listheader>  
	/// <term><see cref="Log"/></term>  
	/// <description><c>DrakeQuest.Log</c> Interfaces Registered</description>  
	///</listheader>
	/// <item>
	/// <term><see cref="DrakeQuest.Log.Generic.ILogger{T}"/></term>
	/// <description>Logger facade<br />
	///  See <see cref="DrakeQuest.Log.Generic.SystemDiagnosticTraceLogger{T}"/> for the registered implementation
	/// </description>
	/// </item>
	/// </list>
	///
	/// </summary>
	///
	/// <remarks>
	/// This module is not intended to be call directly, <see cref="DependencyInjection.Bootstrapper"/>. This module is always registered by the bootstrapper, but all the service are only added if no other implementation have been registered.
	/// <br />
	/// This module will check if a <b>Config/autofac.config</b> exist. It will load it as an <c>XmlModule</c>. <see href="https://autofaccn.readthedocs.io/en/latest/configuration/xml.html">Autofact Xml Configuration</see>
	/// </remarks>
	class StartupModule : Module
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
			//First build default autofac defined in config
			//register by default configuration provider
			builder.RegisterType<ConfigurationManagerProvider>()
				.IfNotRegistered(typeof(Configuration.IConfigurationProvider))
				.As<Configuration.IConfigurationProvider>()
				.SingleInstance();

			//First build default autofac defined in config
			//register by default configuration provider
			builder.RegisterType<ConnectionProvider>()
				.IfNotRegistered(typeof(Configuration.IConnectionProvider))
				.As<Configuration.IConnectionProvider>()
				.SingleInstance();


			builder.RegisterType<DirectoryService>()
				.As<IO.IDirectoryService>()
				.IfNotRegistered(typeof(IO.IDirectoryService))
				.SingleInstance();

			builder.RegisterType<FileService>()
				.As<IO.IFileService>()
				.IfNotRegistered(typeof(IO.IFileService))
				.SingleInstance();

			//Default trace logger for the api
			builder.RegisterGeneric(typeof(SystemDiagnosticTraceLogger<>))
				.IfNotRegistered(typeof(ILogger<string>))
				.As(typeof(ILogger<>))
				.SingleInstance();

			//Register module defined in application config
			//builder.RegisterModule(new ConfigurationSettingsReader(Config.Constants.AutofacSettingSectionName));
			if (File.Exists("Config/autofac.config"))
			{
				var config = new ConfigurationBuilder();
				config.AddXmlFile("Config/autofac.config");
				var module = new ConfigurationModule(config.Build());
				builder.RegisterModule(module);
			}
		}
	}
}
