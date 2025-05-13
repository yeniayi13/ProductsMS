using ProductsMS.Common.Dtos.Firebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Core.Service.Firebase
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadImageAsync(string filePath);
    }
}
