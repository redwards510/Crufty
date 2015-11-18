using Microsoft.Owin;
using Owin;
using Crufty;

[assembly: OwinStartupAttribute(typeof(CruftyWeb.Startup))]
namespace CruftyWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
