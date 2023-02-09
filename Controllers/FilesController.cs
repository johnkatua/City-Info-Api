using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfoAPI.Controllers {
  [Route("api/files")]
  [ApiController]
  public class FilesController : ControllerBase {

    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider) {
      _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
    }
    [HttpGet("{fileId}")]
    public ActionResult GetFile(string fileId) {

      // Look up the actual file, depending on the fieldId...
      var pathToFile = "../Qara-Database-Design.pdf";
      // var pathToFile = "https://international-review.icrc.org/sites/default/files/S0020860400007129a.pdf";

      if (!System.IO.File.Exists(pathToFile)) {
        return NotFound();
      }

      // if (!pathToFile) {
      //   return NotFound();
      // }

      if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType)) {
        contentType = "application/octet-stream";
      }
      var bytes = System.IO.File.ReadAllBytes(pathToFile);
      // var bytes = File.ReadAllBytes(pathToFile);
      return File(bytes, contentType, Path.GetFileName(pathToFile));
    }
  }
}