using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExwhyzeeSP.Web.Areas.UI.Pages.SectionPage
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin,Admin,SubAdmin,mSuperAdmin")]

    public class SenatorySectionModel : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SenatorySectionModel(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public List<string> ImagePaths { get; private set; } // List to temporarily store the image paths

        public void OnGet()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string folderPath = Path.Combine(webRootPath, "templatesample/senatory"); // Path to your image folder
            ImagePaths = new List<string>();
            if (Directory.Exists(folderPath))
            {
                IEnumerable<string> ImagePath = Directory.GetFiles(folderPath);

                foreach (var d in ImagePath.ToArray())
                {

                    string fileName = Path.GetFileName(d);

                    ImagePaths.Add(fileName);
                }
                ImagePaths.Sort();
            }
        }
    }
}
