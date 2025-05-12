using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AppointmentSystem.Shared.Models;
using AppointmentSystem.Shared.Interfaces;

namespace AppointmentSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetDoctorAppointments(
            int doctorId,
            [FromQuery] DateTime? date = null)
        {
            var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId, date);
            return Ok(appointments);
        }

        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetPatientAppointments(int patientId)
        {
            var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
            return Ok(appointments);
        }

        [HttpGet("available-slots/{doctorId}")]
        public async Task<ActionResult<IEnumerable<TimeSpan>>> GetAvailableTimeSlots(
            int doctorId,
            [FromQuery] DateTime date)
        {
            var timeSlots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, date);
            return Ok(timeSlots);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointment);
            if (createdAppointment == null)
                return BadRequest("The requested time slot is not available.");

            return CreatedAtAction(nameof(GetAppointmentById), new { id = createdAppointment.Id }, createdAppointment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> UpdateAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id)
                return BadRequest();

            var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(appointment);
            if (updatedAppointment == null)
                return NotFound();

            return Ok(updatedAppointment);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var result = await _appointmentService.DeleteAppointmentAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] AppointmentStatus status)
        {
            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, status);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 