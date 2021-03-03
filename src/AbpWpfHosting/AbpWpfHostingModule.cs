using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AbpWpfHosting
{
    [DependsOn(
        typeof(AbpAutofacModule)
        )]
    public class AbpWpfHostingModule:AbpModule
    {
    }
}