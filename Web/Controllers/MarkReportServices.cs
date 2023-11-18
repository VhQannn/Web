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
using Web.DTOs;

namespace Web.Controllers
{
    public class MarkReportServices
    {
        private readonly WebContext _context;
        public MarkReportServices(WebContext context)
        {
            _context = context;
        }

        public async Task<MarkReportReponse> CalculateMark(MarkReportRequest markReportRequest)
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

                            // xu ly ket qua Grammar
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
                                    string QAIDX = "";
                                    QuestionTemplate questionTemplateCheck = _context.QuestionTemplates.FirstOrDefault(q => q.QuestionTemplateCode == examCode);
                                    QuestionTemplatesDetail QTcheck = _context.QuestionTemplatesDetails.FirstOrDefault(qtd => qtd.QId == QID && qtd.QuestionTemplateId == questionTemplateCheck.QuestionTemplateId);
                                    Multimedium multimedium = _context.Multimedia.FirstOrDefault(q => q.QuestionTemplatesDetailId == QTcheck.QuestionTemplatesDetailId);
                                    if (QTcheck != null && questionTemplateCheck != null)
                                    {
                                        var Qaids = _context.QuestionTemplateDetailQaids.Where(q => q.QuestionTemplatesDetailId == QTcheck.QuestionTemplatesDetailId).ToArray();

                                        int[] userAnswers = grammarQuestion1.QuestionAnswers
                                                            .Cast<QuestionAnswer>()
                                                            .Where(qa => qa.Selected)
                                                            .Select(qa => qa.QAID)
                                                            .ToArray();


                                        int[] correctAnswers = Qaids.Select(q => q.QAid).ToArray();

                                        bool areAnswersCorrect = AreArraysEqual(userAnswers, correctAnswers);


                                        if (areAnswersCorrect)
                                        {
                                            flag = 1;
                                            count++;
                                        }
                                    }
                                    // Add MarkReportDTO based on the flag value
                                    string status = flag == 1 ? "Correct" : "Incorrect";
                                    markReportDTOs.Add(new MarkReportDTO
                                    {
                                        imageUrl = multimedium?.MultimediaUrl,
                                        status = status,
                                        qtext = QTcheck.QText
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

            return markReportReponse;
        }

        static bool AreArraysEqual(int[] array1, int[] array2)
        {
            // Kiểm tra độ dài của hai mảng
            if (array1.Length != array2.Length)
            {
                return false;
            }

            // Sử dụng SequenceEqual để so sánh hai mảng
            return array1.SequenceEqual(array2);
        }

        public async Task<string> GetTemplateCodeFromFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is not provided or is empty.");
            }

            try
            {
                string examCode = string.Empty;

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0; // Reset the position to the beginning of the stream

                    BinaryFormatter formatter = new BinaryFormatter();
                    memoryStream.Seek(0, SeekOrigin.Begin); // Ensure the stream position is at the beginning
                    SubmitPaper d = (SubmitPaper)formatter.Deserialize(memoryStream);
                    examCode = d.SPaper.ExamCode;

                    return examCode;
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine("Error in GetTemplateCodeFromFile: " + ex.Message);
                throw;
            }
        }


        public async Task<string> GetTemplateCodeFromLink(string fileUrl)
        {
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
                            string examCode = d.SPaper.ExamCode;
                            return examCode + " \n MSSV: " +d.LoginId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return "Không tìm thấy mã môn";

        }
    }
}
