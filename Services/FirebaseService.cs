using Firebase.Database;
using Firebase.Database.Query;

namespace web1.Services
{
    public class FirebaseService
    {
        private readonly FirebaseClient _firebase;
        public FirebaseService(IConfiguration config)
        {
            _firebase = new FirebaseClient(config["Firebase:DatabaseUrl"]);
        }

        public async Task<QrSession> GenerateQr(string student_id, string enrollment_id)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var session = new QrSession
            {
                qr_session_id = $"{student_id}_{now}",
                student_id = student_id,
                enrollment_id = enrollment_id,
                qr_value = $"{student_id}|{now}|{Guid.NewGuid().ToString().Substring(0, 6)}",
                generated_at = now,
                expires_at = now + 300,
                is_used = false
            };
            await _firebase.Child("qr_sessions").Child(session.qr_session_id).PutAsync(session);
            return session;
        }

        public async Task<string> ScanQr(string qr_value, string scanner_id)
        {
            var sessions = await _firebase.Child("qr_sessions").OnceAsync<QrSession>();
            var found = sessions.FirstOrDefault(s => s.Object.qr_value == qr_value && !s.Object.is_used);
            if (found == null) return "INVALID QR";
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now > found.Object.expires_at) return "QR EXPIRED";
            found.Object.is_used = true;
            await _firebase.Child("qr_sessions").Child(found.Key).PutAsync(found.Object);
            var att = new Attendance
            {
                attendance_id = $"{DateTime.Now:yyyy-MM-dd}_{found.Object.enrollment_id}",
                enrollment_id = found.Object.enrollment_id,
                scanner_id = scanner_id,
                date = DateTime.Now.ToString("yyyy-MM-dd"),
                time_in = DateTime.Now.ToString("hh:mm:ss tt"),
                timestamp = now,
                status = "Present"
            };
            await _firebase.Child("attendance").Child(att.date).Child(att.enrollment_id).PutAsync(att);
            return "ATTENDANCE OK - " + found.Object.student_id;
        }
    }

    public class QrSession
    {
        public string qr_session_id { get; set; }
        public string student_id { get; set; }
        public string enrollment_id { get; set; }
        public string qr_value { get; set; }
        public long generated_at { get; set; }
        public long expires_at { get; set; }
        public bool is_used { get; set; }
    }
    public class Attendance
    {
        public string attendance_id { get; set; }
        public string enrollment_id { get; set; }
        public string scanner_id { get; set; }
        public string date { get; set; }
        public string time_in { get; set; }
        public long timestamp { get; set; }
        public string status { get; set; }
    }
}