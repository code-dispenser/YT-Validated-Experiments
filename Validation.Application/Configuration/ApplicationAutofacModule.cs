using Autofac;
using Validation.Application.Validation;

namespace Validation.Application.Configuration;

public class ApplicationAutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ApplicationFacade>().AsSelf().InstancePerDependency();
        builder.RegisterType<ValueObjectService>().AsSelf().SingleInstance(); 

        base.Load(builder);
    }
}
