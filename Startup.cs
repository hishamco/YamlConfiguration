using System;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.RepresentationModel;
using Microsoft.Extensions.Logging;

namespace YamlConfiguration
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddYamlFile("appsettings.yml");
        
            Configuration = builder.Build();
            
            Console.WriteLine(Configuration["version"]);
            Console.WriteLine(Configuration["config:first_property"]);
            Console.WriteLine(Configuration["config:innerconfig:third_property"]);
            Console.WriteLine(Configuration["config:innerconfig:innerconfig:fourth_property"]);
            
            // Setup the input
			var input = new StringReader(@"version: 'the value to override'
config :
 first_property: 'the value'
 second_property: another one
 innerconfig:
  third_property: 'the value'
  innerconfig:
   fourth_property: another one
item: 
 - a value
 - another value
 - the last value
another_item: 
 - id: 1
   value: test
 - id: 2
   value: test2 
nestedlist: 
- 
  - first list
  - first list
-
  - second list
- second list");

			/*var yaml = new YamlStream();
			yaml.Load(input);

			var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

			foreach (var entry in mapping.Children)
			{
                if (entry.Value is YamlScalarNode)
                {
                    Console.WriteLine($"Key:{((YamlScalarNode)entry.Key).Value}, Value:{((YamlScalarNode)entry.Value).Value}");
                }
                if (entry.Value is YamlMappingNode)
                {
                    Traverse((YamlMappingNode)entry.Value, ((YamlScalarNode)entry.Key).Value);
                }
			}*/ 
        }
        
        private void Traverse(YamlMappingNode node, string prefix)
        {
            foreach (var item in node.Children)
            {
                if (item.Value is YamlScalarNode)
                {
                    Console.WriteLine($"Key:{prefix}{ConfigurationPath.KeyDelimiter}{item.Key}, Value:{item.Value}");
                }
                
                if (item.Value is YamlMappingNode)
                {
                    prefix+= string.Concat(ConfigurationPath.KeyDelimiter, ((YamlScalarNode)item.Key).Value);
                    Traverse((YamlMappingNode)item.Value, prefix);
                }
            }
        }
        
        public IConfigurationRoot Configuration { get; set; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync(Configuration["Title"]);
            });
        }
    }
}