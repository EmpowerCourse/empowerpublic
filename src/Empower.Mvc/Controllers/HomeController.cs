﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Empower.Services;
using NHibernate;
using NHibernate.Criterion;
using Empower.Domain.Client.Requests;
using Empower.Domain.Client.ViewModels;
using Empower.NHibernate.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;

namespace Empower.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private IEmailService _emailService;
        private IRepository<Empower.NHibernate.Entities.Actor> _actorRepository;


        // This is a constructor acting as a recipe.
        // It contains all the ingredients that HomeController
        // needs to do its job.
        //
        public HomeController(
           IEmailService emailService,
           IRepository<Empower.NHibernate.Entities.Actor> actorRepository
        )
        {
            _emailService = emailService;
            _actorRepository = actorRepository;
        }

        public IActionResult Index()
        {
            var allActors = _actorRepository
                .GetQuery().ToList();
            return View(allActors);
        }

        [HttpPost]
        public IActionResult Filter(string firstname, string lastname)
        {
            var filteredActors = _actorRepository.GetQuery();
            if (firstname != null)
            {
                filteredActors = filteredActors
                    .Where(x => x.FirstName.StartsWith(firstname));
            }

            if (lastname != null)
            {
                filteredActors = filteredActors
                    .Where(x => x.LastName.StartsWith(lastname));
            }

            //var filteredActors = _actorRepository
            //    .GetQuery()
            //    .Where(x => (firstname == null || x.FirstName.StartsWith(firstname))
            //                && (lastname == null || x.LastName.StartsWith(lastname)))
            //    .ToList();
            return PartialView("_ActorList", filteredActors.ToList());  
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            // New up a ContactViewModel
            var contact = new ContactViewModel();


            return View(contact);
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var response = _emailService.SendContactEmail
                (
                    viewModel.Name,
                    viewModel.Email,
                    viewModel.Message
                );

                viewModel.CompletedAt = response.CompletedAt;

                if (!viewModel.CompletedAt.HasValue)
                {
                    viewModel.ErrorMessage = "Oops.  We have a problem";
                }
            }
            return View(viewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
