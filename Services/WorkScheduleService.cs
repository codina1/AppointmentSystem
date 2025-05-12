using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AppointmentSystem.API.Data;
using AppointmentSystem.Shared.Models;
using AppointmentSystem.Shared.Interfaces;

namespace AppointmentSystem.API.Services
{
    public class WorkScheduleService : IWorkScheduleService
    {
        private readonly ApplicationDbContext _context;

        public WorkScheduleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkSchedule>> GetDoctorWorkSchedulesAsync(int doctorId)
        {
            return await _context.WorkSchedules
                .Where(w => w.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<WorkSchedule> GetWorkScheduleByIdAsync(int id)
        {
            return await _context.WorkSchedules.FindAsync(id);
        }

        public async Task<WorkSchedule> CreateWorkScheduleAsync(WorkSchedule workSchedule)
        {
            workSchedule.CreatedAt = DateTime.UtcNow;

            _context.WorkSchedules.Add(workSchedule);
            await _context.SaveChangesAsync();

            return workSchedule;
        }

        public async Task<WorkSchedule> UpdateWorkScheduleAsync(WorkSchedule workSchedule)
        {
            var existingSchedule = await _context.WorkSchedules.FindAsync(workSchedule.Id);
            if (existingSchedule == null)
                return null;

            existingSchedule.DayOfWeek = workSchedule.DayOfWeek;
            existingSchedule.StartTime = workSchedule.StartTime;
            existingSchedule.EndTime = workSchedule.EndTime;
            existingSchedule.AppointmentDurationMinutes = workSchedule.AppointmentDurationMinutes;
            existingSchedule.IsAvailable = workSchedule.IsAvailable;
            existingSchedule.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingSchedule;
        }

        public async Task<bool> DeleteWorkScheduleAsync(int id)
        {
            var schedule = await _context.WorkSchedules.FindAsync(id);
            if (schedule == null)
                return false;

            _context.WorkSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var workSchedules = await GetDoctorWorkSchedulesAsync(doctorId);
            var dayOfWeek = date.DayOfWeek;
            var workSchedule = workSchedules.FirstOrDefault(w => w.DayOfWeek == dayOfWeek);

            if (workSchedule == null || !workSchedule.IsAvailable)
                return false;

            // Check if the requested time slot is within work hours
            if (startTime < workSchedule.StartTime || endTime > workSchedule.EndTime)
                return false;

            // Check if the duration matches the doctor's appointment duration
            var duration = endTime - startTime;
            var expectedDuration = TimeSpan.FromMinutes(workSchedule.AppointmentDurationMinutes);
            if (duration != expectedDuration)
                return false;

            // Check if there are any overlapping appointments
            var overlappingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate.Date == date.Date &&
                           a.IsActive &&
                           ((a.StartTime <= startTime && startTime < a.EndTime) ||
                            (a.StartTime < endTime && endTime <= a.EndTime) ||
                            (startTime <= a.StartTime && a.EndTime <= endTime)))
                .AnyAsync();

            return !overlappingAppointments;
        }
    }
} 