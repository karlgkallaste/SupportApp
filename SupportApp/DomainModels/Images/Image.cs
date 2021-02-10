using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SupportApp.Models.Tickets;

namespace SupportApp.Models.Images
{
    public  class Image
    {
        public Image(byte[] data)
        {
            Data = data;
        }
        public int Id { get; private set; }
        public byte[] Data { get; private set; }
        public virtual Ticket Ticket { get; private set; }
        public int TicketId { get; private set; }

    }
}