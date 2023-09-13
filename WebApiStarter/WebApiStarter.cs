using WebApi;

namespace WebApiStarter {

    public class WebApiStarter {

        public static void Main(string[] args) {

            Environment.CurrentDirectory =
                Path.GetDirectoryName(Environment.ProcessPath) ?? AppDomain.CurrentDomain.BaseDirectory;

            WebApiRunner.Run(args);

        }

    }

}