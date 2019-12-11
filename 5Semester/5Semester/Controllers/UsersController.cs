﻿using System;
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

namespace _5Semester.Controllers
{
    public class UsersController : Controller
    {    
        private readonly _5SemesterContext _context;
        const string SessionName = "_Name";
        const string SessionAge = "_Age";

        public UsersController(_5SemesterContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
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
                StringBuilder Sb = new StringBuilder();
                using (var hash = SHA256.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    Byte[] result = hash.ComputeHash(enc.GetBytes(pass));

                    foreach (Byte b in result)
                        Sb.Append(b.ToString("x2"));
                }
                pass = Sb.ToString();

            
                if (userinfo.Password == pass)
                {
                    HttpContext.Session.SetString("sessionName", userinfo.DisplayName);
                    HttpContext.Session.SetString("sessionStatus", userinfo.Status);
                    HttpContext.Session.SetInt32("sessionAge", 1576000000);

                    TempData["name"] = userinfo.DisplayName;
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login", "Users");
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin"){
                return View(await _context.User.ToListAsync());
            }
            return RedirectToAction("Login", "Users");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
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
        public async Task<IActionResult> Create([Bind("UserId,Username,Password,DisplayName")] User user)
        {
            if (ModelState.IsValid)
            {
                StringBuilder Sb = new StringBuilder();

                using (var hash = SHA256.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    Byte[] result = hash.ComputeHash(enc.GetBytes(user.Password));

                    foreach (Byte b in result)
                        Sb.Append(b.ToString("x2"));
                }
                user.Password = Sb.ToString();

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,DisplayName")] User user)
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

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
