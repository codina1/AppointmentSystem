using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AppointmentSystem.API.Data;
using AppointmentSystem.Shared.Models;
using AppointmentSystem.Shared.Interfaces;

namespace AppointmentSystem.API.Services
{
    public class SMSService : ISMSService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly SMSSettings _smsSettings;

        public SMSService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _smsSettings = _configuration.GetSection("SMSSettings").Get<SMSSettings>();
        }

        public async Task<bool> SendAppointmentConfirmationAsync(Appointment appointment)
        {
            if (!_smsSettings.IsEnabled)
                return true;

            var message = $"Your appointment with Dr. {appointment.Doctor.FirstName} {appointment.Doctor.LastName} " +
                         $"has been confirmed for {appointment.AppointmentDate:MM/dd/yyyy} at {appointment.StartTime:hh\\:mm tt}. " +
                         $"Please arrive 10 minutes before your appointment time.";

            return await SendSMSAsync(appointment.Patient.PhoneNumber, message, appointment.Id);
        }

        public async Task<bool> SendAppointmentCancellationAsync(Appointment appointment)
        {
            if (!_smsSettings.IsEnabled)
                return true;

            var message = $"Your appointment with Dr. {appointment.Doctor.FirstName} {appointment.Doctor.LastName} " +
                         $"for {appointment.AppointmentDate:MM/dd/yyyy} at {appointment.StartTime:hh\\:mm tt} " +
                         "has been cancelled. Please contact the clinic to reschedule.";

            return await SendSMSAsync(appointment.Patient.PhoneNumber, message, appointment.Id);
        }

        public async Task<bool> SendAppointmentReminderAsync(Appointment appointment)
        {
            if (!_smsSettings.IsEnabled)
                return true;

            var message = $"Reminder: You have an appointment with Dr. {appointment.Doctor.FirstName} {appointment.Doctor.LastName} " +
                         $"tomorrow at {appointment.StartTime:hh\\:mm tt}. " +
                         "Please arrive 10 minutes before your appointment time.";

            return await SendSMSAsync(appointment.Patient.PhoneNumber, message, appointment.Id);
        }

        public async Task<bool> SendCustomMessageAsync(string phoneNumber, string message)
        {
            if (!_smsSettings.IsEnabled)
                return true;

            return await SendSMSAsync(phoneNumber, message, null);
        }

        private async Task<bool> SendSMSAsync(string phoneNumber, string message, int? appointmentId)
        {
            try
            {
                // In a real application, you would integrate with an SMS provider here
                // For now, we'll just log the message to the database
                var smsNotification = new SMSNotification
                {
                    PhoneNumber = phoneNumber,
                    Message = message,
                    Status = SMSStatus.Sent,
                    CreatedAt = DateTime.UtcNow,
                    SentAt = DateTime.UtcNow,
                    AppointmentId = appointmentId
                };

                _context.SMSNotifications.Add(smsNotification);
                await _context.SaveChangesAsync();

                // Simulate SMS sending
                await Task.Delay(100);

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                var smsNotification = new SMSNotification
                {
                    PhoneNumber = phoneNumber,
                    Message = message,
                    Status = SMSStatus.Failed,
                    CreatedAt = DateTime.UtcNow,
                    ErrorMessage = ex.Message,
                    AppointmentId = appointmentId
                };

                _context.SMSNotifications.Add(smsNotification);
                await _context.SaveChangesAsync();

                return false;
            }
        }
    }
} 