using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _5Semester.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MailKit.Net.Smtp;
using MimeKit;
using System.Net.Mail;
using MimeKit.Text;

namespace _5Semester.Controllers
{
    public class UsersController : Controller
    {    
        private readonly _5SemesterContext _context;

        //const string SessionName = "_Name";
        //const string SessionAge = "_Age";

        public UsersController(_5SemesterContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult PasswordRecoveryPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordRecoveryBtn(string email)
        {
            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        //instantiate a new MimeMessage
            //        var message = new MimeMessage();
            //        //Setting the To e-mail address
            //        message.To.Add(new MailboxAddress("E-mail Recipient Name", "recipient@domail.com"));
            //        //Setting the From e-mail address
            //        message.From.Add(new MailboxAddress("E-mail From Name", "from@domain.com"));
            //        //E-mail subject 
            //        message.Subject = contactViewModel.Subject;
            //        //E-mail message body
            //        message.Body = new TextPart(TextFormat.Html)
            //        {
            //            Text = contactViewModel.Message + " Message was sent by: " + contactViewModel.Name + " E-mail: " + contactViewModel.Email
            //        };

            //        //Configure the e-mail
            //        using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            //        {
            //            emailClient.Connect("smtp.sparkpostmail.com", 587, false);
            //            emailClient.Authenticate("SMTP_Injection", "76289861fbff7abd93a583e3aeeb5b5bb02e5ce8");
            //            emailClient.Send(message);
            //            emailClient.Disconnect(true);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        ModelState.Clear();
            //        ViewBag.Message = $" Oops! We have a problem here {ex.Message}";
            //    }
            //}

            User userinfo = _context.User
                         .Where(b => b.Email == email)
                    .FirstOrDefault();
            try
            {
                    if (email == userinfo.Email)
                    {
                    
                    }
            }
            catch(NullReferenceException e)
            {
                
            }
                return View();
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LoginBtn(string user, string pass)
        {
            User userinfo = _context.User
                         .Where(b => b.Username == user)
                    .FirstOrDefault();
            if (pass != null && userinfo != null)
            {
                //StringBuilder Sb = new StringBuilder();
                //using (var hash = SHA256.Create())
                //{
                //    Encoding enc = Encoding.UTF8;
                //    Byte[] result = hash.ComputeHash(enc.GetBytes(pass));

                //    foreach (Byte b in result)
                //        Sb.Append(b.ToString("x2"));
                //}
                //pass = Sb.ToString();
                
                pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: pass,
                salt: userinfo.Salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

                if (userinfo.Password == pass)
                {
                    HttpContext.Session.SetString("sessionName", userinfo.DisplayName);
                    HttpContext.Session.SetString("sessionStatus", userinfo.Status);
                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                                           /////2 hours in seconds//////                                                                            
                    int sessiontime = Convert.ToInt32(unixTimestamp + 7200);
                    HttpContext.Session.SetInt32("sessionAge", sessiontime);

                    //TempData["name"] = userinfo.DisplayName;
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login", "Users");
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                return View(await _context.User.ToListAsync());
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.User
                    .FirstOrDefaultAsync(m => m.UserId == id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Status,Password,DisplayName,Salt,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////
                //StringBuilder Sb = new StringBuilder();

                //using (var hash = SHA256.Create())
                //{
                //    Encoding enc = Encoding.UTF8;
                //    Byte[] result = hash.ComputeHash(enc.GetBytes(user.Password));

                //    foreach (Byte b in result)
                //        Sb.Append(b.ToString("x2"));
                //}
                //user.Password = Sb.ToString();
                //user.Status = "User";
                ////////////////////////////////////////////////////////////////////////////////////
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: user.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                user.Password = hashed;
                user.Status = "User";

                //StringBuilder Sb = new StringBuilder();
                
                //foreach (Byte b in salt)
                //{
                //    Sb.Append(b.ToString("x2"));
                //}
                
                //user.Salt = Sb.ToString();

                user.Salt = salt;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,DisplayName")] User user)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.User
                    .FirstOrDefaultAsync(m => m.UserId == id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
                }
            return RedirectToAction("Index", "Home");
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin") {
                    var user = await _context.User.FindAsync(id);
                _context.User.Remove(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
