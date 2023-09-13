using WebApi;
using WebAppStarter.Middleware;

namespace WebAppStarter {

    public class WebAppStarter {

        public static void Main(string[] args) {

            Environment.CurrentDirectory =
                Path.GetDirectoryName(Environment.ProcessPath) ?? AppDomain.CurrentDomain.BaseDirectory;

            WebApiRunner.BeforeRun += (WebApplication app) => {
                app.UseVueRewriteMiddleware();
                app.UseStaticFiles();
            };
            WebApiRunner.Run(args);

        }

    }

}


