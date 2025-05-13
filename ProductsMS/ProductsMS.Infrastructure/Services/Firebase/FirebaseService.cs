using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Firebase.Auth.Providers;
using Microsoft.EntityFrameworkCore.Metadata;
using ProductsMS.Core.Service.Firebase;


namespace ProductsMS.Infrastructure.Services.Firebase
{
    
public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly ILogger<FirebaseStorageService> _logger;
        private readonly FirebaseStorageSettings _firebaseStorageSettings;
        private readonly FirebaseAuthClient _authClient;

        public FirebaseStorageService(ILogger<FirebaseStorageService> logger, IOptions<FirebaseStorageSettings> firebaseStorageSettings)
        {
            _logger = logger;
            _firebaseStorageSettings = firebaseStorageSettings.Value;

            var authConfig = new FirebaseAuthConfig
            {
                ApiKey = _firebaseStorageSettings.ApiKey,
               // Email = _firebaseStorageSettings.AuthEmail,
               // Password = _firebaseStorageSettings.AuthPassword,
               // AuthType = AuthType.EmailAndPassword
            };

            _authClient = new FirebaseAuthClient(authConfig);
        }

        private async Task<string> GetAuthTokenAsync()
        {
            try
            {
                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(
                    _firebaseStorageSettings.AuthEmail,
                    _firebaseStorageSettings.AuthPassword
                );

                return userCredential.User.Credential.IdToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la autenticación con Firebase.");
                throw;
            }
        }

        public async Task<string> UploadImageAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Iniciando subida de imagen: {FilePath}", filePath);

                using var stream = File.OpenRead(filePath);
                var authToken = await GetAuthTokenAsync();

                var storage = new FirebaseStorage(
                    _firebaseStorageSettings.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(authToken),
                        ThrowOnCancel = true
                    });

                var uploadTask = storage.Child("imagenes")
                                       .Child(Path.GetFileName(filePath))
                                       .PutAsync(stream);

                var url = await uploadTask;
                _logger.LogInformation("Imagen subida con éxito: {Url}", url);
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imagen.");
                throw;
            }
        }
    }
}
