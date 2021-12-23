using Autofac;
using Microsoft.Extensions.Configuration;
using DrakeQuest.Log;
using DrakeQuest.Log.Generic;
using Autofac.Configuration;
using System;
using Autofac.Builder;

namespace DrakeQuest.DependencyInjection
{
	/// <summary>
	///<c>Bootstrapper</c> for autofac, must be used in the entry point of project, avoid to use it in others steps
	/// </summary>
	/// <summary>
	/// This bootstrapper is reponsible of the Managemement of a container:
	/// </summary>
	/// <example>
	/// </example>
	public class Bootstrapper
	{
		#region fields

		/// <summary>
		/// Lock object in case of multit thread
		/// </summary>
		private static readonly object Locker = new object();

		private static Bootstrapper Instance;

		#endregion

		#region Properties

		/// <summary>
		/// Container reference in case of use externaly
		/// </summary>
		public IContainer Container { get; private set; }

		#endregion

		#region Constructor

		private Bootstrapper() { }

		#endregion

		#region Methods

		/// <summary>
		/// Get current bootstrapper or initialized a new one
		/// </summary>
		/// <returns></returns>
		public static Bootstrapper GetInstance()
		{
			lock (Locker)
			{
				if (Instance == null)
				{
					Instance = new Bootstrapper();
				}
	
			}
			return Instance;
		}


		/// <summary>
		/// Get the Container defined previously by the first call to GetInstance the bootstrapper
		/// </summary>
		/// <returns></returns>
		public static IContainer GetContainer()
		{
			return GetInstance().Container;
		}

		/// <summary>
		/// Generate a default ContainerBuilder
		/// </summary>
		/// <returns>ContainerBuilder with the startup module registered</returns>
		public static ContainerBuilder ContainerBuilderFactory(){
			//create minimal initializer
			var builder = new ContainerBuilder();
			builder.RegisterModule<StartupModule>();
			return builder;
		}

		/// <summary>
		/// Initialize the builer container with specific factory
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="factory"></param>
		protected void Initialize(ContainerBuilder builder, Func<ContainerBuilder> factory)
		{

			if (builder == null && factory == null)
			{
				throw new AggregateException(new Exception[]{
					new ArgumentNullException("builder", "The builder in parameter is null, is mandatory for initializing modules"),
					new ArgumentNullException("factory", "The container factory is mandatory, call this.ContainerBuilderFactory() if default is enought")
				});

			}

			if (builder == null)
			{
				throw new ArgumentNullException("builder", "The builder in parameter is null, is mandatory for initializing modules");
			}

			if (factory == null)
			{
				throw new ArgumentNullException("factory", "The container factory is mandatory, call this.ContainerBuilderFactory() if default is enought");
			}

			//create minimal initializer
			var innerbuilder = factory();
			//To ensure all the mandatory registration has been done
			innerbuilder.RegisterModule<StartupModule>();

			using (var container = innerbuilder.Build())
			{
				Container = container;
				ILogger log = container.Resolve<ILogger<Bootstrapper>>(new NamedParameter("loggerName", "Bootstrapper"));
				log.Configure();
				log.Log("Registering StartupModule");
				//container.Resolve<IL>
				builder.RegisterModule<StartupModule>();
				
				//Update current builder with autofacs config file
				var configProvider = container.Resolve<Configuration.IConfigurationProvider>();
				var DirectoryService = container.Resolve<IO.IDirectoryService>();
				//only load configuration if config folder is set
				string folder = configProvider.GetAppSettings(Configuration.Constants.ConfigFolderKey);
				log.Log($"Checking config folder : {folder}");

				if (DirectoryService.Exists(folder))
				{
					foreach (var file in DirectoryService.GetFiles(folder, $"*{Configuration.Constants.AutofacConfigExtension}"))
					{
						var config = new ConfigurationBuilder();
						log.Log($"Registering configuration in file : {file}");
						config.AddXmlFile(file);
						var module = new ConfigurationModule(config.Build());
						builder.RegisterModule(module);
					}
				}else
				{
					log.Log(LevelEnum.Warn, $"Config foler<{folder}> not exists or missing");
				}
				Container = null;
			}
		}

		/// <summary>
		/// Build and initialize the container for the current module
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public IContainer Build(ContainerBuilder builder, Func<ContainerBuilder> factory = null)
		{
			Initialize(builder, factory ?? new Func<ContainerBuilder>(ContainerBuilderFactory));
			Container = builder.Build(ContainerBuildOptions.None);
			return Container;
		}

		#endregion
	}
}
