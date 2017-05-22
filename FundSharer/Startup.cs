using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FundSharer.Startup))]
namespace FundSharer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
