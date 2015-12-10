using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SoftuniTwitter.Web.Startup))]
namespace SoftuniTwitter.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
