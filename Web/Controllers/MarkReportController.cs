using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuestionLib;
using QuestionLib.Entity;
using System.Runtime.Serialization.Formatters.Binary;
using Web.DbConnection;
using Web.Models;
using ProtoBuf;
using System.Text.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    [Route("api/markreport")]
    [ApiController]
    public class MarkReportController : Controller
    {
        private readonly WebContext _context;
        public MarkReportController(WebContext context)
        {
            _context = context;
        }

        [HttpPost("get-mark")]
        public async Task<IActionResult> GetMark([FromBody] MarkReportRequest markReportRequest)
        {
            List<MarkReportDTO> markReportDTOs = new List<MarkReportDTO>();
            MarkReportReponse markReportReponse = new MarkReportReponse();
            string fileUrl = markReportRequest.url;
            int count = 0;
            float mark = 0.000f;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Reading the file for checking scores
                    using (HttpResponseMessage response = await client.GetAsync(fileUrl))
                    {
                        using (Stream scoreFileStream = await response.Content.ReadAsStreamAsync())
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            SubmitPaper d = (SubmitPaper)formatter.Deserialize(scoreFileStream);
                            /*Console.WriteLine("File Name: {0}", d.LoginId);*/
                            string examCode = d.SPaper.ExamCode;
                            /*Console.WriteLine("Test Type: {0}", d.SPaper.TestType.ToString());*/
                            int duration = int.Parse(d.SPaper.Duration.ToString());
                            int totalMark = int.Parse(d.SPaper.Mark.ToString());
                            Console.WriteLine("Login ID: {0}", d.LoginId);

                            scoreFileStream.Close();
                            Console.WriteLine("log.........................");

                            Console.WriteLine("Load du lieu hoan tat");
                            // bat dau check ket qua

                            if (d.SPaper.FillBlankQuestions.Count > 0)
                            {
                                Console.WriteLine("Filling Question...\n");
                            }

                            // su ly ket qua Grammar
                            if (d.SPaper.GrammarQuestions.Count > 0)
                            {
                                Console.WriteLine("Grammar Question...\n");
                                for (int i = 0; i < totalMark; i++)
                                {
                                    Console.WriteLine("------------------ Count : " + count);
                                    int flag = 0;
                                    Question grammarQuestion1 = (Question)d.SPaper.GrammarQuestions[index: i];
                                    int QID = int.Parse(grammarQuestion1.QID.ToString());
                                    int index = 0;
                                    int QAID = 6;
                                    String QAIDX = "";
                                    QuestionTemplate questionTemplateCheck = _context.QuestionTemplates.FirstOrDefault(q => q.QuestionTemplateCode == examCode);
                                    QuestionTemplatesDetail QTcheck = _context.QuestionTemplatesDetails.FirstOrDefault(qtd => qtd.QId == QID);
                                    Multimedium multimedium = _context.Multimedia.FirstOrDefault(q => q.QuestionTemplatesDetailId == QTcheck.QuestionTemplatesDetailId);
                                    if (QTcheck != null && questionTemplateCheck != null)
                                    {
                                        foreach (QuestionAnswer questionAnswer in grammarQuestion1.QuestionAnswers)
                                        {
                                            if (questionAnswer.Selected)
                                            {
                                                if (int.Parse(questionAnswer.QAID.ToString()) == QTcheck.QAid)
                                                {
                                                    QAIDX = questionAnswer.QAID.ToString();
                                                    //Console.WriteLine(QAIDX);
                                                    flag = 1;
                                                    count++;
                                                }
                                            }
                                            ++index;
                                        }
                                    }
                                    // Add MarkReportDTO based on the flag value
                                    string status = flag == 1 ? "True" : "False";
                                    markReportDTOs.Add(new MarkReportDTO
                                    {
                                        imageUrl = multimedium?.MultimediaUrl,
                                        status = status
                                    });
                                }
                            }

                            if (d.SPaper.IndicateMQuestions.Count > 0)
                            {
                                Console.WriteLine("Indicate Question...\n");
                            }
                            mark = ((float)count * 10f) / (float)totalMark;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            
            MarkReport markReport = _context.MarkReports.FirstOrDefault(x => x.MarkReportId == markReportRequest.markReportId);
            markReport.MarkScore = mark;
            _context.MarkReports.Update(markReport);
            _context.SaveChanges();

            markReportReponse.totalMark = mark.ToString("#.###");
            markReportReponse.markReportDTOs = markReportDTOs;

            return Ok(markReportReponse);
        }
    }
}
