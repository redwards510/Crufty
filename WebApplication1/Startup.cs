using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CruftyWeb.Startup))]
namespace CruftyWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
