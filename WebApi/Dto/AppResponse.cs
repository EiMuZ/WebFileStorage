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
            public static AppResponse Build(string msg = "OK") => new(0, msg);
            public static AppResponse<T> Build<T>(string msg, T data) => new(0, msg, data);
            public static AppResponse<T> Build<T>(T data) => new(0, "OK", data);
        }

        public static class AuthenticationFailed {
            public static AppResponse Build(string msg = "Authentication Failed") => new(10, msg);
            public static AppResponse<T> Build<T>(string msg, T data) => new(10, msg, data);
            public static AppResponse<T> Build<T>(T data) => new(0, "Authentication Failed", data);
        }

        public static class AuthorizationFailed {
            public static AppResponse Build(string msg = "Authorization Failed") => new(20, msg);
            public static AppResponse<T> Build<T>(string msg, T data) => new(20, msg, data);
            public static AppResponse<T> Build<T>(T data) => new(0, "Authorization Failed", data);
        }

        public static class InvalidParameter {
            public static AppResponse Build(string msg = "Invalid Parameter") => new(30, msg);
            public static AppResponse<T> Build<T>(string msg, T data) => new(30, msg, data); 
            public static AppResponse<T> Build<T>(T data) => new(0, "Invalid Parameter", data);
        }

        public static class FileOrDirectoryNotFound {
            public static AppResponse Build(string msg = "File Or Directory Not Found") => new(40, msg);
            public static AppResponse<T> Build<T>(string msg, T data) => new(40, msg, data);
            public static AppResponse<T> Build<T>(T data) => new(40, "File Or Directory Not Found", data);
        }

        public static class InternalError {
            public static AppResponse Build(string msg = "Internal Error") => new(3000, msg);
            public static AppResponse<T> Build<T>(string msg, T data) => new(3000, msg, data);
            public static AppResponse<T> Build<T>(T data) => new(3000, "Internal Error", data);
        }

    }

}
