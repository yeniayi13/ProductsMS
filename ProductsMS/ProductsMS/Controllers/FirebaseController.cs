using Microsoft.AspNetCore.Mvc;
using ProductsMS.Common.Dtos.Firebase;
using ProductsMS.Core.Service.Firebase;

namespace ProductsMS.Controllers
{

    [ApiController]
    [Route("/auctioner/firebase")]
    public class FirebaseController : ControllerBase
    {
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ILogger<FirebaseController> _logger;

        public FirebaseController(IFirebaseStorageService firebaseStorageService, ILogger<FirebaseController> logger)
        {
            _firebaseStorageService = firebaseStorageService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromBody] ImageDto request)
        {
            if (string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.FileBase64))
            {
                return BadRequest("Debe proporcionar un archivo válido.");
            }

            try
            {
                var filePath = Path.Combine(Path.GetTempPath(), request.FileName);
                var fileBytes = Convert.FromBase64String(request.FileBase64);
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                var imageUrl = await _firebaseStorageService.UploadImageAsync(filePath);
                return Ok(new { message = "Imagen subida con éxito", imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir la imagen.");
                return StatusCode(500, "Ocurrió un error al subir la imagen.");
            }
        }
    }

}
