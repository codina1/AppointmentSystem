# سیستم نوبت‌دهی پزشکان

این پروژه یک سیستم نوبت‌دهی آنلاین برای پزشکان است که به بیماران امکان می‌دهد به راحتی نوبت ویزیت پزشک خود را رزرو کنند.

## ویژگی‌ها

- 🔐 احراز هویت و مدیریت کاربران با JWT
- 👥 مدیریت نقش‌ها (Admin, Doctor, Patient)
- 👨‍⚕️ مدیریت اطلاعات پزشکان
- 👤 مدیریت اطلاعات بیماران
- 📅 مدیریت نوبت‌ها
- ⏰ مدیریت برنامه کاری پزشکان
- 📱 ارسال پیامک برای اطلاع‌رسانی
- 🔍 جستجوی پزشکان بر اساس تخصص
- 📊 داشبورد مدیریتی

- 
## ارتباط با توسعه دهنده : 
مختار سهولی : 09150904936 - soholiinfo@gmail.com


## پیش‌نیازها

- .NET 6.0 SDK یا بالاتر
- SQL Server یا SQL Server Express
- Visual Studio 2022 یا Visual Studio Code

## نصب و راه‌اندازی

1. کلون کردن مخزن:
```bash
git clone https://github.com/yourusername/AppointmentSystem.git
cd AppointmentSystem
```

2. تنظیمات دیتابیس:
- فایل `appsettings.json` را باز کنید
- رشته اتصال دیتابیس را در بخش `ConnectionStrings` تنظیم کنید

3. اجرای مایگریشن‌ها:
```bash
dotnet ef database update
```

4. اجرای پروژه:
```bash
dotnet run
```

## ساختار پروژه

```
AppointmentSystem/
├── AppointmentSystem.API/           # پروژه API
├── AppointmentSystem.Shared/        # پروژه مشترک
│   ├── Models/                      # مدل‌های داده
│   └── Interfaces/                  # رابط‌های سرویس
└── AppointmentSystem.Tests/         # پروژه تست
```

## API Endpoints

### احراز هویت
- `POST /api/auth/register` - ثبت‌نام کاربر جدید
- `POST /api/auth/login` - ورود کاربر

### پزشکان
- `GET /api/doctor` - دریافت لیست پزشکان
- `GET /api/doctor/{id}` - دریافت اطلاعات یک پزشک
- `GET /api/doctor/specialty/{specialty}` - جستجوی پزشکان بر اساس تخصص
- `POST /api/doctor` - افزودن پزشک جدید
- `PUT /api/doctor/{id}` - بروزرسانی اطلاعات پزشک
- `DELETE /api/doctor/{id}` - حذف پزشک

### بیماران
- `GET /api/patient` - دریافت لیست بیماران
- `GET /api/patient/{id}` - دریافت اطلاعات یک بیمار
- `POST /api/patient` - افزودن بیمار جدید
- `PUT /api/patient/{id}` - بروزرسانی اطلاعات بیمار
- `DELETE /api/patient/{id}` - حذف بیمار

### نوبت‌ها
- `GET /api/appointment` - دریافت لیست نوبت‌ها
- `GET /api/appointment/{id}` - دریافت اطلاعات یک نوبت
- `GET /api/appointment/doctor/{doctorId}` - دریافت نوبت‌های یک پزشک
- `GET /api/appointment/patient/{patientId}` - دریافت نوبت‌های یک بیمار
- `GET /api/appointment/available-slots/{doctorId}` - دریافت زمان‌های خالی پزشک
- `POST /api/appointment` - ثبت نوبت جدید
- `PUT /api/appointment/{id}` - بروزرسانی نوبت
- `DELETE /api/appointment/{id}` - لغو نوبت
- `PUT /api/appointment/{id}/status` - تغییر وضعیت نوبت

### برنامه کاری
- `GET /api/workschedule/doctor/{doctorId}` - دریافت برنامه کاری پزشک
- `GET /api/workschedule/{id}` - دریافت اطلاعات یک برنامه کاری
- `POST /api/workschedule` - افزودن برنامه کاری جدید
- `PUT /api/workschedule/{id}` - بروزرسانی برنامه کاری
- `DELETE /api/workschedule/{id}` - حذف برنامه کاری

## کاربران پیش‌فرض

### ادمین
- ایمیل: admin@appointmentsystem.com
- رمز عبور: Admin123!

### پزشک
- ایمیل: john.doe@example.com
- رمز عبور: Doctor123!

### بیمار
- ایمیل: alice.johnson@example.com
- رمز عبور: Patient123!

## امنیت

- احراز هویت با JWT
- رمزنگاری رمز عبور
- مدیریت نقش‌ها و دسترسی‌ها
- محافظت از API با CORS

## تست

برای اجرای تست‌ها:
```bash
dotnet test
```

## مشارکت

1. Fork کردن پروژه
2. ایجاد شاخه جدید (`git checkout -b feature/AmazingFeature`)
3. Commit تغییرات (`git commit -m 'Add some AmazingFeature'`)
4. Push به شاخه (`git push origin feature/AmazingFeature`)
5. ایجاد Pull Request

## لایسنس

این پروژه تحت لایسنس MIT منتشر شده است. برای اطلاعات بیشتر فایل `LICENSE` را مطالعه کنید.

## تماس

برای پرسش‌ها و پیشنهادات، لطفاً یک Issue ایجاد کنید. 
