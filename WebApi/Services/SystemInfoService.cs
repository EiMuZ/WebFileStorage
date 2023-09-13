using WebApi.Configuration;
using WebApi.Dto;

namespace WebApi.Services {

    public class SystemInfoService {

        private readonly AppConfig _appConfig;

        public SystemInfoService(AppConfig appConfig) {
            _appConfig = appConfig;
        }

        public AppResponse GetStorageDiskInfo() {

            var storageRoot = _appConfig.Storage.Root;
            var info = new DriveInfo(Path.GetFullPath(storageRoot));

            return ResponseBuilder.Success.Build(new DiskInfoDto {
                FreeSize = info.TotalFreeSpace,
                TotalSize = info.TotalSize
            });
        }

    }

}
