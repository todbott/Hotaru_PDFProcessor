using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iText;
using iText.Kernel.Pdf;
using System.IO;
using System.Text;


namespace ElasticBeanstalk_pdf.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        [HttpPost]
        public ActionResult ChoosePdf()
        {
            Session["fileName"] = Request.Files["pdfPath"].FileName;
            Session["forTextFile"] = "";

            var inputPDF = Request.Files["pdfPath"].InputStream;
            var outputPDF = new MemoryStream();
            
            int furibans = 1;
            Boolean thisPageHasOne = false;
            List<int> pagesToRemove = new List<int>();

            PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputPDF), new PdfWriter(outputPDF));
            
            for (var page = 1; page < pdfDoc.GetNumberOfPages(); page++)
            {
                thisPageHasOne = false;

                List<iText.Kernel.Pdf.Annot.PdfAnnotation> annotArray = (List<iText.Kernel.Pdf.Annot.PdfAnnotation>)pdfDoc.GetPage(page).GetAnnotations();

                for (var a = 0; a < annotArray.Count(); a++)
                {
                    if (annotArray[a].GetType().Name == "PdfFreeTextAnnotation")
                    {
                        var c = annotArray[a].GetContents().ToUnicodeString();
                        if (c.IndexOf("###") == -1)
                        {
                            thisPageHasOne = true;
                            Session["forTextFile"] = Session["forTextFile"].ToString() + furibans.ToString() + Environment.NewLine + c + Environment.NewLine + Environment.NewLine;
                            furibans++;
                        } else
                        {
                            pdfDoc.GetPage(page).RemoveAnnotation(annotArray[a]);
                        }
                    } else
                    {
                        pdfDoc.GetPage(page).RemoveAnnotation(annotArray[a]);
                    }
                }
                if (!thisPageHasOne)
                {
                    pagesToRemove.Add(page);
                }
            }

            int equalizer = 0;
            foreach (int pp in pagesToRemove)
            {
                pdfDoc.RemovePage(pp-equalizer);
                equalizer++;
            }
            
            pdfDoc.Close();

            byte[] hereComes = outputPDF.ToArray();
            
            return File(hereComes, "application/pdf", Session["fileName"].ToString().Replace(".pdf", "_編集済.pdf"));
        }

        public ActionResult Download_Genko()
        {
            var tt = Session["forTextFile"].ToString();

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(tt);
            writer.Flush();
            stream.Position = 0;

            byte[] hereComes = stream.ToArray();
            var fn = Session["fileName"].ToString().Replace(".pdf", "_原稿.txt");
            
            return File(hereComes, "text/plain", fn);
        }
    }
}
