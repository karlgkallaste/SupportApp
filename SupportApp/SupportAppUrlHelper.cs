using Microsoft.AspNetCore.Mvc;

namespace SupportApp
{
    public interface ISupportAppUrlHelper
    {
        string Page(
            string pageName,
            string pageHandler,
            object values,
            string protocol);
    }
    
    public class SupportAppUrlHelper : ISupportAppUrlHelper
    {
        private readonly IUrlHelper _urlHelper;

        public SupportAppUrlHelper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string Page(string pageName, string pageHandler, object values, string protocol)
        {
            return _urlHelper.Page(pageName, pageHandler, values, protocol);
        }
    }
}