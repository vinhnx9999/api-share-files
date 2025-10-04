using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using System.Reflection;

namespace VinhSharingFiles.APIs.Extensions;

public static class ConfigurationExtensions
{
    public const string DEFAULT_CONFIG_SECTION = "AWS";

    //public static AWSOptions GetAWSOptions(this IConfiguration config, string configSection)
    //{
    //    AWSOptions aWSOptions = new();
    //    IConfiguration configuration = ((!string.IsNullOrEmpty(configSection)) ? config.GetSection(configSection) : config);
    //    if (configuration == null)
    //    {
    //        return aWSOptions;
    //    }

    //    TypeInfo typeInfo = typeof(ClientConfig).GetTypeInfo();
    //    foreach (IConfigurationSection element in configuration.GetChildren())
    //    {
    //        try
    //        {
    //            PropertyInfo propertyInfo = typeInfo.DeclaredProperties.SingleOrDefault(p => p.Name.Equals(element.Key, StringComparison.OrdinalIgnoreCase));
    //            if (!(propertyInfo == null) && !(propertyInfo.SetMethod == null))
    //            {
    //                if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType.GetTypeInfo().IsPrimitive)
    //                {
    //                    var obj = Convert.ChangeType(element.Value, propertyInfo.PropertyType);
    //                    propertyInfo.SetMethod.Invoke(aWSOptions.DefaultClientConfig, [obj]);
    //                }
    //                else if (propertyInfo.PropertyType == typeof(TimeSpan) || propertyInfo.PropertyType == typeof(TimeSpan?))
    //                {
    //                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(Convert.ToInt64(element.Value));
    //                    propertyInfo.SetMethod.Invoke(aWSOptions.DefaultClientConfig, [timeSpan]);
    //                }
    //            }
    //        }
    //        catch (Exception exception)
    //        {
    //            throw new ConfigurationException("Error reading value for property " + element.Key + ".", exception)
    //            {
    //                PropertyName = element.Key,
    //                PropertyValue = element.Value
    //            };
    //        }
    //    }

    //    if (!string.IsNullOrEmpty(configuration["Profile"]))
    //    {
    //        aWSOptions.Profile = configuration["Profile"];
    //    }
    //    else if (!string.IsNullOrEmpty(configuration["AWSProfileName"]))
    //    {
    //        aWSOptions.Profile = configuration["AWSProfileName"];
    //    }

    //    if (!string.IsNullOrEmpty(configuration["ProfilesLocation"]))
    //    {
    //        aWSOptions.ProfilesLocation = configuration["ProfilesLocation"];
    //    }
    //    else if (!string.IsNullOrEmpty(configuration["AWSProfilesLocation"]))
    //    {
    //        aWSOptions.ProfilesLocation = configuration["AWSProfilesLocation"];
    //    }

    //    if (!string.IsNullOrEmpty(configuration["Region"]))
    //    {
    //        aWSOptions.Region = RegionEndpoint.GetBySystemName(configuration["Region"]);
    //    }
    //    else if (!string.IsNullOrEmpty(configuration["AWSRegion"]))
    //    {
    //        aWSOptions.Region = RegionEndpoint.GetBySystemName(configuration["AWSRegion"]);
    //    }

    //    if (!string.IsNullOrEmpty(configuration["DefaultsMode"]))
    //    {
    //        if (!Enum.TryParse<DefaultConfigurationMode>(configuration["DefaultsMode"], out var result))
    //        {
    //            throw new ArgumentException("Invalid value for DefaultConfiguration. Valid values are: " + string.Join(", ", Enum.GetNames(typeof(DefaultConfigurationMode))) + " ");
    //        }

    //        aWSOptions.DefaultConfigurationMode = result;
    //    }

    //    if (!string.IsNullOrEmpty(configuration["SessionRoleArn"]))
    //    {
    //        aWSOptions.SessionRoleArn = configuration["SessionRoleArn"];
    //    }

    //    if (!string.IsNullOrEmpty(configuration["SessionName"]))
    //    {
    //        aWSOptions.SessionName = configuration["SessionName"];
    //    }

    //    IConfigurationSection section = configuration.GetSection("Logging");
    //    if (section != null)
    //    {
    //        aWSOptions.Logging = new AWSOptions.LoggingSetting();
    //        if (!string.IsNullOrEmpty(section["LogTo"]))
    //        {
    //            if (!Enum.TryParse<LoggingOptions>(section["LogTo"], out var result2))
    //            {
    //                throw new ArgumentException("Invalid value for LogTo. Valid values are: " + string.Join(", ", Enum.GetNames(typeof(LoggingOptions))) + " ");
    //            }

    //            aWSOptions.Logging.LogTo = result2;
    //        }

    //        if (!string.IsNullOrEmpty(section["LogResponses"]))
    //        {
    //            if (!Enum.TryParse<ResponseLoggingOption>(section["LogResponses"], out var result3))
    //            {
    //                throw new ArgumentException("Invalid value for LogResponses. Valid values are: " + string.Join(", ", Enum.GetNames(typeof(ResponseLoggingOption))) + " ");
    //            }

    //            aWSOptions.Logging.LogResponses = result3;
    //        }

    //        if (!string.IsNullOrEmpty(section["LogResponsesSizeLimit"]))
    //        {
    //            if (!int.TryParse(section["LogResponsesSizeLimit"], out var result4))
    //            {
    //                throw new ArgumentException("Invalid integer value for LogResponsesSizeLimit.");
    //            }

    //            aWSOptions.Logging.LogResponsesSizeLimit = result4;
    //        }

    //        if (!string.IsNullOrEmpty(section["LogMetrics"]))
    //        {
    //            if (!bool.TryParse(section["LogMetrics"], out var result5))
    //            {
    //                throw new ArgumentException("Invalid boolean value for LogMetrics.");
    //            }

    //            aWSOptions.Logging.LogMetrics = result5;
    //        }
    //    }

    //    return aWSOptions;
    //}
}
