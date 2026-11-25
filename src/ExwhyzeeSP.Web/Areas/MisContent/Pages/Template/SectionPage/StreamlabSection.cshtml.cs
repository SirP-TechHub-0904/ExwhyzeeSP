using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExwhyzeeSP.Web.Areas.MisContent.Pages.Template.SectionPage
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,SubAdmin,mSuperAdmin")]
    public class StreamlabSectionModel : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public StreamlabSectionModel(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public List<string> ImagePaths { get; private set; }

        public void OnGet()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string folderPath = Path.Combine(webRootPath, "templatesample/streamlab");
            ImagePaths = new List<string>();
            if (Directory.Exists(folderPath))
            {
                IEnumerable<string> paths = Directory.GetFiles(folderPath);
                foreach (var d in paths)
                {
                    ImagePaths.Add(Path.GetFileName(d));
                }
                ImagePaths.Sort();
            }
        }
    }
}
