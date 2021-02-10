using System.IO;

namespace SupportApp.Infrastructure
{
    public interface IMemoryStreamProvider
    {
        MemoryStream Provide();
    }

    public class MemoryStreamProvider : IMemoryStreamProvider
    {
        public MemoryStream Provide()
        {
            return new MemoryStream();
        }
    }
}