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
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkScheduleService _workScheduleService;
        private readonly ISMSService _smsService;

        public AppointmentService(
            ApplicationDbContext context,
            IWorkScheduleService workScheduleService,
            ISMSService smsService)
        {
            _context = context;
            _workScheduleService = workScheduleService;
            _smsService = smsService;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);
        }

        public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsAsync(int doctorId, DateTime? date = null)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.IsActive);

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.IsActive)
                .ToListAsync();
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            // Check if the time slot is available
            var isAvailable = await _workScheduleService.IsTimeSlotAvailableAsync(
                appointment.DoctorId,
                appointment.AppointmentDate,
                appointment.StartTime,
                appointment.EndTime);

            if (!isAvailable)
                return null;

            appointment.CreatedAt = DateTime.UtcNow;
            appointment.IsActive = true;
            appointment.Status = AppointmentStatus.Pending;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Send confirmation SMS
            await _smsService.SendAppointmentConfirmationAsync(appointment);

            return appointment;
        }

        public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
        {
            var existingAppointment = await _context.Appointments.FindAsync(appointment.Id);
            if (existingAppointment == null)
                return null;

            // Check if the new time slot is available
            var isAvailable = await _workScheduleService.IsTimeSlotAvailableAsync(
                appointment.DoctorId,
                appointment.AppointmentDate,
                appointment.StartTime,
                appointment.EndTime);

            if (!isAvailable)
                return null;

            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.StartTime = appointment.StartTime;
            existingAppointment.EndTime = appointment.EndTime;
            existingAppointment.Notes = appointment.Notes;
            existingAppointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Send update SMS
            await _smsService.SendAppointmentConfirmationAsync(existingAppointment);

            return existingAppointment;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return false;

            appointment.IsActive = false;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Send cancellation SMS
            await _smsService.SendAppointmentCancellationAsync(appointment);

            return true;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return false;

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Send appropriate SMS based on status
            if (status == AppointmentStatus.Confirmed)
                await _smsService.SendAppointmentConfirmationAsync(appointment);
            else if (status == AppointmentStatus.Cancelled)
                await _smsService.SendAppointmentCancellationAsync(appointment);

            return true;
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            var workSchedules = await _workScheduleService.GetDoctorWorkSchedulesAsync(doctorId);
            var dayOfWeek = date.DayOfWeek;
            var workSchedule = workSchedules.FirstOrDefault(w => w.DayOfWeek == dayOfWeek);

            if (workSchedule == null || !workSchedule.IsAvailable)
                return new List<TimeSpan>();

            var existingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate.Date == date.Date &&
                           a.IsActive)
                .ToListAsync();

            var availableSlots = new List<TimeSpan>();
            var currentTime = workSchedule.StartTime;
            var endTime = workSchedule.EndTime;
            var duration = TimeSpan.FromMinutes(workSchedule.AppointmentDurationMinutes);

            while (currentTime.Add(duration) <= endTime)
            {
                var isSlotAvailable = !existingAppointments.Any(a =>
                    a.StartTime <= currentTime && currentTime < a.EndTime);

                if (isSlotAvailable)
                    availableSlots.Add(currentTime);

                currentTime = currentTime.Add(duration);
            }

            return availableSlots;
        }
    }
} 