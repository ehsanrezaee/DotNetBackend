using Autofac;
using ErSoftDev.Common.Utilities;
using ErSoftDev.Framework.Behaviors;
using ErSoftDev.Framework.Log;
using MediatR;

namespace ErSoftDev.Framework.Configuration
{
    public class AutofacConfigurationExtension : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            containerBuilder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerLifetimeScope();

            var endPointApplicationAssemblies = Tools.GetAllAssemblies().ToArray();

            containerBuilder.RegisterAssemblyTypes(endPointApplicationAssemblies)
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(endPointApplicationAssemblies)
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(endPointApplicationAssemblies)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
