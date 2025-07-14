using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using System.Threading.Tasks;
using ColorMode = DinkToPdf.ColorMode;
using PaperKind = DinkToPdf.PaperKind;

namespace CommonLib.Utils
{
    public static class PdfGenerator
    {
        private static readonly IConverter _converter;

        static PdfGenerator()
        {
            _converter = new SynchronizedConverter(new PdfTools());
        }

        /// <summary>
        /// Generates a PDF file from HTML content asynchronously.
        /// </summary>
        /// <param name="htmlContent">The HTML content to convert.</param>
        /// <returns>PDF as a byte array</returns>
        public static async Task<byte[]> GeneratePdfAsync(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new ArgumentException("HTML content cannot be empty.");
            }

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10, Bottom = 10 },
                },
                Objects = {
                    new ObjectSettings
                    {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            try
            {
                return await Task.Run(() => _converter.Convert(doc)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate PDF.", ex);
            }
        }
    }
}