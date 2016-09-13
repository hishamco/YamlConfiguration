using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using YamlDotNet.RepresentationModel;

namespace YamlConfiguration
{
    public class YamlConfigurationProvider : FileConfigurationProvider
    {
        private Dictionary<string, string> _data;
        
        public YamlConfigurationProvider(YamlConfigurationSource source) : base(source) { }

        public override void Load(Stream stream)
        {
           _data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);  
                        
			var yaml = new YamlStream();
            
			yaml.Load(new StreamReader(stream));

			var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
 
            foreach (var entry in mapping.Children)
			{
                if (entry.Value is YamlScalarNode)
                {
                    var key = ((YamlScalarNode)entry.Key).Value;
                    var value = ((YamlScalarNode)entry.Value).Value;
                    
                    _data[key] = value;
                }
                
                if (entry.Value is YamlMappingNode)
                {
                    Traverse((YamlMappingNode)entry.Value, ((YamlScalarNode)entry.Key).Value);
                }
			}

            Data = _data;
        }
        
        private void Traverse(YamlMappingNode node, string prefix)
        {
            foreach (var item in node.Children)
            {
                if (item.Value is YamlScalarNode)
                {
                    var key = string.Concat(prefix, ConfigurationPath.KeyDelimiter, item.Key);
                    
                    _data[key] = item.Value.ToString();
                }
                
                if (item.Value is YamlMappingNode)
                {
                    prefix+= string.Concat(ConfigurationPath.KeyDelimiter, ((YamlScalarNode)item.Key).Value);
                    Traverse((YamlMappingNode)item.Value, prefix);
                }
            }
        }
    }
}