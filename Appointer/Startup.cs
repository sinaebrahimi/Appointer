using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Appointer.Startup))]
namespace Appointer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}