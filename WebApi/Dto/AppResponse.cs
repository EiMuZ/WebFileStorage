using System.Text.Json.Serialization;

namespace WebApi.Dto {


    public class AppResponse {

        [JsonPropertyOrder(0)]
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyOrder(1)]
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        public AppResponse(int code, string msg = "") {
            Code = code;
            Msg = msg;
        }

    }

    public sealed class AppResponse<T> : AppResponse {

        [JsonPropertyOrder(2)]
        [JsonPropertyName("data")]
        public T Data { get; set; }

        public AppResponse(int code, string msg, T data) : base(code, msg) {
            Data = data;
        }

    }

    public static partial class ResponseBuilder {

        public static class Success {
            public static AppResponse Build(string msg = "") {
                return new AppResponse(0, msg);
            }

            public static AppResponse<T> Build<T>(string msg, T data) {
                return new AppResponse<T>(0, msg, data);
            }
        }

        public static class AuthenticationFailed {
            public static AppResponse Build(string msg = "") {
                return new AppResponse(10, msg);
            }

            public static AppResponse<T> Build<T>(string msg, T data) {
                return new AppResponse<T>(10, msg, data);
            }
        }

        public static class AuthorizationFailed {
            public static AppResponse Build(string msg = "") {
                return new AppResponse(20, msg);
            }

            public static AppResponse<T> Build<T>(string msg, T data) {
                return new AppResponse<T>(20, msg, data);
            }
        }

        public static class InvalidParameter {
            public static AppResponse Build(string msg = "") {
                return new AppResponse(30, msg);
            }

            public static AppResponse<T> Build<T>(string msg, T data) {
                return new AppResponse<T>(30, msg, data);
            }
        }

        public static class FileOrDirectoryNotFound {
            public static AppResponse Build(string msg = "") {
                return new AppResponse(40, msg);
            }

            public static AppResponse<T> Build<T>(string msg, T data) {
                return new AppResponse<T>(40, msg, data);
            }
        }

        public static class InternalError {
            public static AppResponse Build(string msg = "") {
                return new AppResponse(3000, msg);
            }

            public static AppResponse<T> Build<T>(string msg, T data) {
                return new AppResponse<T>(3000, msg, data);
            }
        }

    }

}
