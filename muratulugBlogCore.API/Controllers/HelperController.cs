using Microsoft.AspNetCore.Mvc;
using muratulugBlogCore.API.Models;
using System.Net.Mail;

namespace muratulugBlogCore.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendConcactEmail(Contact contact)
        {
            try
            {
                System.Threading.Thread.Sleep(2000);
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                mailMessage.From = new MailAddress("ulugyazilim@gmail.com");
                mailMessage.To.Add("mmuratulug@gmail.com");
                mailMessage.Subject = contact.Subject;
                mailMessage.Body = contact.Message;
                mailMessage.IsBodyHtml = true;
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("ulugyazilim@gmail.com", "123Qwe123.");
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
                return Ok();
            }
            catch (System.Exception ex)
            {

                return BadRequest(ex);
            }
         
        }
    }
}