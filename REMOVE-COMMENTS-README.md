# Remove Comments Script

این اسکریپت تمام کامنت‌ها را از فایل‌های C# در solution حذف می‌کند.

## انواع کامنت‌هایی که حذف می‌شوند:

1. **XML Documentation Comments**:
   ```csharp
   /// <summary>
   /// This is a summary
   /// </summary>
   /// <param name="x">Parameter description</param>
   ```

2. **Single-line Comments**:
   ```csharp
   // This is a single line comment
   int x = 5; // Inline comment
   ```

3. **Multi-line Comments**:
   ```csharp
   /*
    * This is a
    * multi-line comment
    */
   ```

## نحوه استفاده:

### ✅ Windows (PowerShell):

#### 1. اول یک test run انجام دهید (بدون تغییر فایل‌ها):
```powershell
cd C:\Users\m.mousavi\temp\github_cpg\CPG
.\Remove-Comments.ps1 -WhatIf
```

#### 2. اگر نتیجه را تایید کردید، اجرای واقعی:
```powershell
.\Remove-Comments.ps1
```

#### 3. اگر فقط یک پوشه خاص را می‌خواهید:
```powershell
.\Remove-Comments.ps1 -Path ".\src\CPG.Domain"
```

### ⚠️ هشدارها:

1. **قبل از اجرا حتماً backup بگیرید یا commit کنید**:
   ```bash
   git add .
   git commit -m "Backup before removing comments"
   ```

2. **اول با `-WhatIf` تست کنید** تا ببینید چه تغییراتی اعمال می‌شود

3. **این عملیات قابل بازگشت نیست** (مگر از طریق git)

4. **فایل‌های obj/ و bin/ نادیده گرفته می‌شوند**

## خروجی نمونه:

```
Starting comment removal process...
Target directory: C:\Users\m.mousavi\temp\github_cpg\CPG
Found 450 C# files to process

Processing: ApplicationSettings.cs
  ✓ Comments removed
Processing: User.cs
  ✓ Comments removed
Processing: Program.cs
  - No comments found

========================================
Summary:
  Total files scanned: 450
  Files modified: 347
  Files unchanged: 103
========================================

Done! All comments have been removed.
Don't forget to commit your changes to git!
```

## بعد از اجرا:

1. **بررسی تغییرات در git**:
   ```bash
   git status
   git diff
   ```

2. **اگر نتیجه را تایید کردید، commit کنید**:
   ```bash
   git add .
   git commit -m "Remove all comments and XML documentation from C# files"
   git push origin stage
   ```

3. **اگر مشکلی پیش آمد، برگردانید**:
   ```bash
   git checkout .
   ```

## توضیحات فنی:

- **Encoding**: فایل‌ها با UTF-8 (بدون BOM) ذخیره می‌شوند
- **Line Endings**: خطوط خالی اضافی حذف می‌شوند (حداکثر 2 خط خالی متوالی)
- **URLs**: لینک‌های `http://` و `https://` حفظ می‌شوند
- **Performance**: حدود 100 فایل در ثانیه پردازش می‌شود

## مثال استفاده کامل:

```powershell
# 1. رفتن به پوشه solution
cd C:\Users\m.mousavi\temp\github_cpg\CPG

# 2. Backup با git
git add .
git commit -m "Backup before comment removal"

# 3. تست اجرا (WhatIf mode)
.\Remove-Comments.ps1 -WhatIf

# 4. بررسی نتایج و تایید

# 5. اجرای واقعی
.\Remove-Comments.ps1

# 6. بررسی تغییرات
git diff --stat

# 7. Commit نهایی
git add .
git commit -m "Remove all comments from C# files"
git push origin stage
```

## پشتیبانی:

اگر مشکلی پیش آمد:
- اسکریپت log های مفصل نمایش می‌دهد
- می‌توانید با git تغییرات را برگردانید
- فایل‌های obj/ و bin/ لمس نمی‌شوند
