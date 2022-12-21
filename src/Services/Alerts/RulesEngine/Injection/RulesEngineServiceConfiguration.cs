﻿using Google.Api;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Injection
{
    public static class RulesEngineServiceConfiguration
    {
        public static void AddRulesEngineConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var rulesConfig = configuration.GetSection("AlertsClassesConfig").Get<AlertsClassesConfig>();

            AddRulesCommandsDictionary(services);
            AddRulesCommandDictionary(services, configuration);
        }

        private static void AddRulesCommandDictionary(IServiceCollection services, IConfiguration configuration)
        {
            var rulesEngineConfig = configuration.GetSection("AlertsClassesConfig").Get<AlertsClassesConfig>();
            var alertsConfigByClass = new Dictionary<string, List<AlertsConfig>>();
            foreach(var classConfig in rulesEngineConfig.ClassesConfig)
            {
                var alertsConfigs = new List<AlertsConfig>();
                foreach(var alertName in classConfig.Alerts)
                {
                    alertsConfigs.Add(rulesEngineConfig.AlertsConfig.First(alertConfig => alertConfig.AlertName == alertName));
                }
                alertsConfigByClass.Add(classConfig.Name, alertsConfigs);
            }

            services.AddSingleton(alertsConfigDictionary => alertsConfigByClass);
        }

        private static void AddRulesCommandsDictionary(IServiceCollection services)
        {
            var commandsType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IRequest<bool>).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
            var types = new Dictionary<string, Type>();
            commandsType.ForEach(commandType => types.Add(((RuleTagAttribute)commandType.GetCustomAttribute(typeof(RuleTagAttribute))).Name, commandType));

            services.AddSingleton(commandTypesDictionary => types);
        }
    }
}
