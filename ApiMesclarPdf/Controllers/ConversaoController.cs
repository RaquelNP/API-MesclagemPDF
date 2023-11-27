using ApiMesclarPdf.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ApiMesclarPdf.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConversaoController : ControllerBase
    {
        [HttpPost]
        public ActionResult EnviaArquivo([FromForm] List<IFormFile> arquivo)
        {        
            try
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK);
              

                if (arquivo.Count > 0)
                {
                    ConverterPDF arquivoPdf = new ConverterPDF();

                    var retFormato = arquivoPdf.ConverterEmByte(arquivo);

                    return File(retFormato, arquivo.FirstOrDefault().ContentType, "Arquivo.pdf");
                }

                return Ok("Arquivos meclados com sucesso");

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao realizar a mesclagem. Mais detalhes: " + ex);
            }            
        }
    }
}
