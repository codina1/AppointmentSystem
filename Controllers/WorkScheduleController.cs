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
    public class WorkScheduleController : ControllerBase
    {
        private readonly IWorkScheduleService _workScheduleService;

        public WorkScheduleController(IWorkScheduleService workScheduleService)
        {
            _workScheduleService = workScheduleService;
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<IEnumerable<WorkSchedule>>> GetDoctorWorkSchedules(int doctorId)
        {
            var schedules = await _workScheduleService.GetDoctorWorkSchedulesAsync(doctorId);
            return Ok(schedules);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<WorkSchedule>> GetWorkScheduleById(int id)
        {
            var schedule = await _workScheduleService.GetWorkScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound();

            return Ok(schedule);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<WorkSchedule>> CreateWorkSchedule(WorkSchedule workSchedule)
        {
            var createdSchedule = await _workScheduleService.CreateWorkScheduleAsync(workSchedule);
            return CreatedAtAction(nameof(GetWorkScheduleById), new { id = createdSchedule.Id }, createdSchedule);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateWorkSchedule(int id, WorkSchedule workSchedule)
        {
            if (id != workSchedule.Id)
                return BadRequest();

            var updatedSchedule = await _workScheduleService.UpdateWorkScheduleAsync(workSchedule);
            if (updatedSchedule == null)
                return NotFound();

            return Ok(updatedSchedule);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> DeleteWorkSchedule(int id)
        {
            var result = await _workScheduleService.DeleteWorkScheduleAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 