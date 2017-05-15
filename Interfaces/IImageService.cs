using System.Threading.Tasks;

namespace restratp.Interfaces
{
    public interface IImageService
    {
         Task<byte[]> GetImage(string image);
    }
}