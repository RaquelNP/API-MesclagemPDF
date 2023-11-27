using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using PdfSharp.Pdf.IO;
using iText.IO.Image;
using iText.Kernel.Pdf;
using PdfProperties = iText.Layout.Properties;
using PdfImage = iText.Layout.Element.Image;


namespace ApiMesclarPdf.Model
{
    public class ConverterPDF
    {
        public byte[] ConverterEmByte(ICollection<IFormFile> filePaths)
        {
            try
            {
                List<byte[]> listByte = new List<byte[]>();
                byte[] ConverterByte = null;          
                byte[] result = null;

                foreach (var formFile in filePaths)
                {
                    bool formatPdf = formFile.FileName.Contains("pdf");

                    if (formatPdf)
                    {
                        using (var stream = new MemoryStream())
                        {
                            formFile.CopyTo(stream);
                            listByte.Add(stream.ToArray());
                        }
                    }
                    else
                    {
                        using (var stream = new MemoryStream())
                        {
                            formFile.CopyTo(stream);
                            ConverterByte = stream.ToArray();
                        }

                        //Validação de bytes se for igual a 0
                        int count = 0;
                        while (ConverterByte == null || ConverterByte.Length == 0)
                        {
                            count++;
                            using (var stream = new MemoryStream())
                            {
                                formFile.CopyTo(stream);
                                ConverterByte = stream.ToArray();
                            }

                            if (count >= 10 || ConverterByte.Length > 0)
                                break;                      
                        }

                        var retorno = ConverterImageParaPDF(ConverterByte);
                        listByte.Add(retorno);
                    }                    
                }

                result = MergePdf(listByte);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao converter os arquivos de upload em byte. Mais detalhes " + ex);
            }

        }

        public static byte[] MergePdf(List<byte[]> pdfs)
        {
            try
            {
                byte[] result = null;

                List<PdfSharp.Pdf.PdfDocument> lstDocuments = new List<PdfSharp.Pdf.PdfDocument>();
                foreach (var pdf in pdfs)
                {
                    lstDocuments.Add(PdfSharp.Pdf.IO.PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import));
                }

                using (PdfSharp.Pdf.PdfDocument outPdf = new PdfSharp.Pdf.PdfDocument())
                {
                    for (int i = 1; i <= lstDocuments.Count; i++)
                    {
                        foreach (PdfSharp.Pdf.PdfPage page in lstDocuments[i - 1].Pages)
                        {
                            outPdf.AddPage(page);
                        }
                    }

                    using (MemoryStream mem = new MemoryStream())
                    {
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                        outPdf.Save(mem, false);
                        result = mem.ToArray();
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro realizar o Merge do Pdf. Mais detalhes " + ex);
            }
        }

        public static byte[] ConverterImageParaPDF(byte[] imageBytes)
        {
            try
            {
                using (var me = new MemoryStream())
                {
                    using (var pdfWriter = new PdfWriter(me))
                    {
                        var pdf = new PdfDocument(pdfWriter);
                        var documento = new iText.Layout.Document(pdf);
                        ImageData data = ImageDataFactory.Create(imageBytes);
                        PdfImage img = new PdfImage(data)
                        .SetAutoScale(true)
                        .SetHorizontalAlignment(PdfProperties.HorizontalAlignment.CENTER);

                        documento.Add(img);
                        documento.Close();
                        pdf.Close();
                        return me.ToArray();
                    }
                }

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao converter imagem em PDF. Mais detalhes: " + ex.Message);
            }
             
            
        }
    }
}
