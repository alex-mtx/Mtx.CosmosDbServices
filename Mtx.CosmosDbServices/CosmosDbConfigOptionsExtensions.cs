﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mtx.CosmosDbServices
{
	public static class CosmosDbConfigOptionsExtensions
	{
		/// <summary>
		/// Creates a Cosmos DB database and a container with the specified partition key. 
		/// </summary>
		/// <returns></returns>
		public static void InitializeCosmosDbService(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<CosmosDbOptions>(configuration.GetSection(CosmosDbOptions.CosmosDb));

			services.AddTransient<ICosmosDbService, CosmosDbService>();
			services.AddSingleton<IContainerFactory, ContainerFactory>();
			services.AddSingleton<CosmosClient>(factory =>
			{
				var options = factory.GetRequiredService<IOptions<CosmosDbOptions>>().Value;

				var clientOptions = new CosmosClientOptions
				{
					//SerializerOptions = options,
					Serializer = new CosmosJsonDotNetSerializer(new JsonSerializerSettings
					{
						ContractResolver = new JsonDotNetPrivateResolver(),
						TypeNameHandling = TypeNameHandling.All,
						ReferenceLoopHandling = ReferenceLoopHandling.Error,
						PreserveReferencesHandling = PreserveReferencesHandling.None,
						ConstructorHandling = ConstructorHandling.Default,
					}),
					
				};

				return new CosmosClient(options.Endpoint, options.Key, clientOptions: clientOptions);
			});
		}
	}

}
