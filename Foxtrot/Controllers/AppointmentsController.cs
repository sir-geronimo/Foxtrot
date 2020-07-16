﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Foxtrot.Dtos;
using Foxtrot.Models;
using Microsoft.AspNetCore.Mvc;
using Foxtrot.Repositories.Contracts;

namespace Foxtrot.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository,
            IServiceRepository serviceRepository, IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            return View(await _appointmentRepository.Get());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Services = await _serviceRepository.Get();
            ViewBag.Providers = await _userRepository.Get();
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AppointmentDto appointmentDto)
        {
            if (ModelState.IsValid)
            {
                await _appointmentRepository.Insert(await ConvertToEntity(appointmentDto));
                await _appointmentRepository.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetById(id);
            ViewBag.Appointment = appointment;
            ViewBag.Services = await _serviceRepository.Get();
            ViewBag.Providers = await _userRepository.Get();
            if (appointment == null)
            {
                return NotFound();
            }
            return View();
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] AppointmentDto appointmentDto)
        {
            if (id != appointmentDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Appointment appointment = await _appointmentRepository.GetById(id);
                appointment.Note = appointmentDto.Note;
                appointment.StartDate = appointmentDto.StartDate;
                appointment.EndDate = appointmentDto.EndDate;
                appointment.Service = await _serviceRepository.GetById(appointmentDto.ServiceId);
                appointment.Provider = await _userRepository.GetById(appointmentDto.ProviderId);
                
                await _appointmentRepository.Update(appointment);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private async Task<Appointment> ConvertToEntity(AppointmentDto appointmentDto)
        {
            return new Appointment
            {
                Note = appointmentDto.Note,
                //Creator = await _userRepository.GetById(entity.CreatorId),
                Creator = _userRepository.Get().Result.First(),
                Provider = await _userRepository.GetById(appointmentDto.ProviderId),
                Service = await _serviceRepository.GetById(appointmentDto.ServiceId),
                StartDate = appointmentDto.StartDate,
                EndDate = appointmentDto.EndDate
            };
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _appointmentRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
