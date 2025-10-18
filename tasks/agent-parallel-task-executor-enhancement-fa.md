# درخواست قابلیت: بروزرسانی خودکار فایل Task Breakdown در Agent

## خلاصه
Agent باید به‌صورت خودکار فایل task breakdown را در حین اجرا بروزرسانی کند و تیک‌ها را بزند.

---

## رفتار فعلی

در حال حاضر agent:
1. فایل task breakdown را می‌خواند (مثلاً `tasks/2025-01-18-logging-standardization-simplified.md`)
2. تمام تسک‌ها را اجرا می‌کند
3. یک گزارش خلاصه برمی‌گرداند
4. **فایل اصلی را بروز نمی‌کند** ❌

بعد از تمام شدن agent، یک نفر باید دستی:
- فایل را بخواند
- `- [ ]` را به `- [x]` تبدیل کند
- وضعیت‌ها را اضافه کند: `✅ COMPLETED`, `⏭️ SKIPPED`
- خلاصه اجرا را بنویسد

---

## رفتار مورد نیاز

Agent باید:
1. فایل task breakdown را بخواند
2. **در حین اجرای هر تسک**، فایل را real-time بروزرسانی کند:
   - `- [ ]` را به `- [x]` تبدیل کند
   - وضعیت را اضافه کند: `✅ COMPLETED`, `⏭️ SKIPPED`, `❌ FAILED`
   - زمان اتمام را ثبت کند
3. در پایان، بخش خلاصه اجرا را اضافه کند

---

## ⚡ اجرای Parallel - خیلی مهم!

### تشخیص تسک‌های Parallelizable

Agent باید این فرمت را تشخیص دهد:
```markdown
### 3. Convert HTTP and Provider Classes (8 files)
**Parallelizable**: Yes [P]
**Dependencies**: Task 1
```

### استراتژی اجرا

1️⃣ **تسک‌های سریالی (Parallelizable: No)**
   - به ترتیب یکی پس از دیگری اجرا شوند
   - تسک بعدی منتظر بماند تا تسک قبلی تمام شود

2️⃣ **تسک‌های موازی (Parallelizable: Yes [P])**
   - **همه زیرتسک‌ها را همزمان اجرا کند**
   - مثال: اگر 8 فایل provider باشد، هر 8 فایل را همزمان تبدیل کند
   - زمان صرفه‌جویی: به جای 8×زمان، فقط 1×زمان طول بکشد

### مدیریت Dependencies

```markdown
### 1. Extend ILogService
Parallelizable: No
Dependencies: None

### 2. Convert Middleware
Parallelizable: No
Dependencies: Task 1

### 3. Convert Providers [P]
Parallelizable: Yes [P]
Dependencies: Task 1

### 4. Convert Cache [P]
Parallelizable: Yes [P]
Dependencies: Task 1
```

**نحوه اجرا:**
1. Task 1 (سریالی) → اجرا و تمام شود
2. Task 2 (سریالی) → اجرا و تمام شود
3. **Task 3 و Task 4 به‌صورت موازی** → همزمان شروع شوند (چون هر دو فقط وابسته به Task 1 هستند)

### پیاده‌سازی Parallel Execution

```javascript
// Pseudo-code
async function executeTasks(tasks) {
  let completedTasks = [];

  for (let task of tasks) {
    // بررسی dependency ها
    if (!areDependenciesMet(task, completedTasks)) {
      continue; // صبر کن تا dependency ها تمام شوند
    }

    if (task.parallelizable === true) {
      // اجرای موازی زیرتسک‌ها
      await Promise.all(task.subtasks.map(subtask => {
        return executeSubtask(subtask).then(() => {
          updateTaskFile(task, subtask, 'completed'); // ✅ تیک بزن
        });
      }));
    } else {
      // اجرای سریالی
      for (let subtask of task.subtasks) {
        await executeSubtask(subtask);
        updateTaskFile(task, subtask, 'completed'); // ✅ تیک بزن
      }
    }

    completedTasks.push(task);
    updateTaskFile(task, null, 'completed'); // ✅ کل تسک را تمام شده بزن
  }
}
```

### مثال عملی Parallel Execution

**فایل Task:**
```markdown
### 3. Convert HTTP and Provider Classes (8 files)
**Parallelizable**: Yes [P]
**Dependencies**: Task 1

**Files to convert:**
- `HttpProvider.cs`
- `PecProvider.cs`
- `BehPardakhtProvider.cs`
- `NeoBankProvider.cs`
- `IdpProvider.cs`
- `CharisPayProvider.cs`
- `CharismaCardProvider.cs`
- `IpgFactory.cs`

- [ ] Replace ILogger<T> with ILogService
- [ ] Update logging calls
```

**اجرا:**
```
⏱️ 14:30:00 - شروع Task 3
🔄 14:30:01 - همزمان شروع تبدیل 8 فایل:
   - Thread 1: HttpProvider.cs
   - Thread 2: PecProvider.cs
   - Thread 3: BehPardakhtProvider.cs
   - Thread 4: NeoBankProvider.cs
   - Thread 5: IdpProvider.cs
   - Thread 6: CharisPayProvider.cs
   - Thread 7: CharismaCardProvider.cs
   - Thread 8: IpgFactory.cs

✅ 14:30:45 - HttpProvider.cs تمام شد (45 ثانیه)
✅ 14:30:52 - PecProvider.cs تمام شد (52 ثانیه)
✅ 14:30:58 - BehPardakhtProvider.cs تمام شد (58 ثانیه)
...
✅ 14:31:30 - همه فایل‌ها تمام شدند

⏱️ زمان کل: 1.5 دقیقه (به جای 8 دقیقه!)
```

**فایل بروز شده (real-time):**
```markdown
### 3. Convert HTTP and Provider Classes (8 files) 🔄 IN PROGRESS
**Parallelizable**: Yes [P]
**Dependencies**: Task 1
**Started At:** 2025-01-18 14:30:00

**Files to convert:**
- `HttpProvider.cs` ✅ (14:30:45)
- `PecProvider.cs` ✅ (14:30:52)
- `BehPardakhtProvider.cs` ✅ (14:30:58)
- `NeoBankProvider.cs` 🔄 در حال اجرا...
- `IdpProvider.cs` ⏳ در صف...
- `CharisPayProvider.cs` ⏳ در صف...
- `CharismaCardProvider.cs` ⏳ در صف...
- `IpgFactory.cs` ⏳ در صف...

- [x] Replace ILogger<T> with ILogService
- [ ] Update logging calls
```

---

## بروزرسانی Real-Time

### 1. سطح زیرتسک (Sub-task Level)
```markdown
# قبل
- [ ] Replace ILogger<T> with ILogService
- [ ] Update logging calls

# بعد از اتمام اولین زیرتسک
- [x] Replace ILogger<T> with ILogService ✅ (14:30:45)
- [ ] Update logging calls 🔄
```

### 2. سطح تسک (Task Level)
```markdown
# قبل
### 3. Convert HTTP and Provider Classes

# در حین اجرا
### 3. Convert HTTP and Provider Classes 🔄 IN PROGRESS (3/8 files done)

# بعد از اتمام
### 3. Convert HTTP and Provider Classes ✅ COMPLETED
**Completed At:** 2025-01-18 14:31:30
**Duration:** 1 minute 30 seconds
**Parallel Execution:** Yes (8 files in parallel)
```

---

## علامت‌های وضعیت (Status Indicators)

```markdown
✅ COMPLETED      - تسک با موفقیت تمام شد
🔄 IN PROGRESS   - در حال اجرا
⏭️ SKIPPED       - رد شد (مثلاً فایل پیدا نشد)
❌ FAILED        - با خطا مواجه شد
⚠️ PARTIAL       - بخشی انجام شد، بخشی خطا داد
⏳ PENDING       - در صف انتظار
🔀 PARALLEL      - در حال اجرای موازی
```

---

## خلاصه اجرا در انتهای فایل

```markdown
---

## 📊 خلاصه اجرا

**وضعیت:** ✅ **تمام شد**
**تاریخ:** 1403/10/29 (2025-01-18)
**زمان شروع:** 14:30:00
**زمان پایان:** 14:45:30
**مدت کل:** 15 دقیقه و 30 ثانیه
**Agent:** parallel-task-executor

### آمار:
- **تعداد کل تسک‌ها:** 10
- **✅ تمام شده:** 9
- **⏭️ رد شده:** 1
- **❌ خطا:** 0
- **⚡ تسک‌های Parallel:** 6
- **📁 فایل‌های تغییر یافته:** 27

### تسک‌های اجرا شده:
1. ✅ Extend ILogService Interface (2 min) - سریالی
2. ✅ Convert Infrastructure Middleware (3 min) - سریالی
3. ✅ Convert HTTP and Provider Classes (1.5 min) - **موازی ⚡ (8 فایل)**
4. ✅ Convert Storage and Cache (45 sec) - **موازی ⚡ (2 فایل)**
5. ✅ Convert MediatR Behaviors (2 min) - **موازی ⚡ (4 فایل)**
6. ⏭️ Convert Command Handlers - رد شد (فایل‌ها ILogger نداشتند)
7. ✅ Convert Message Queue Consumers (1 min) - **موازی ⚡ (3 فایل)**
8. ✅ Convert API Layer (1 min) - **موازی ⚡ (2 فایل)**
9. ✅ Convert Persistence Layer (1 min) - **موازی ⚡ (2 فایل)**
10. ✅ Update DI Registrations (30 sec) - سریالی

### صرفه‌جویی در زمان با Parallel Execution:
- زمان تخمینی (سریالی): ~35 دقیقه
- زمان واقعی (موازی): 15.5 دقیقه
- **صرفه‌جویی: 19.5 دقیقه (56%)** ⚡

### تغییرات کد:
```diff
 27 فایل تغییر یافت
 +747 خط اضافه شد
 -175 خط حذف شد
 Net: +572 خط
```

### Commit:
```
Commit: d9a68f78
Message: Refactor: Standardize logging to ILogService with extended CallLogModel
Branch: stage
Pushed: Yes ✅
```
```

---

## تنظیمات (Configuration)

```json
{
  "update_task_file": true,
  "update_frequency": "per_subtask",
  "enable_parallel_execution": true,
  "max_parallel_workers": 8,
  "add_summary": true,
  "add_timestamps": true,
  "show_duration": true,
  "commit_on_complete": false,
  "Persian_summary": true
}
```

### توضیح تنظیمات:

- `update_task_file`: فایل را بروز کند یا نه
- `update_frequency`: چه زمانی بروز کند
  - `"per_subtask"`: بعد از هر زیرتسک
  - `"per_task"`: بعد از هر تسک کامل
  - `"at_end"`: فقط در پایان
- `enable_parallel_execution`: **تسک‌های [P] را موازی اجرا کند**
- `max_parallel_workers`: حداکثر تعداد worker های همزمان
- `add_summary`: خلاصه را در انتها اضافه کند
- `add_timestamps`: زمان‌ها را نمایش دهد
- `show_duration`: مدت زمان هر تسک را نشان دهد

---

## مزایا

### 1️⃣ سرعت بیشتر ⚡
- تسک‌های مستقل به‌صورت موازی اجرا می‌شوند
- صرفه‌جویی 50-70% در زمان

### 2️⃣ شفافیت Real-Time 👁️
- هر لحظه می‌بینید کجای کار هستید
- نیازی به انتظار تا پایان نیست

### 3️⃣ کار دستی کمتر ✋
- دیگر نیازی به تیک زدن دستی نیست
- خودکار مستندسازی می‌شود

### 4️⃣ قابل پیگیری 📊
- زمان‌های دقیق ثبت می‌شود
- آمار کامل از اجرا

### 5️⃣ مناسب برای تیم 👥
- اعضای تیم می‌توانند پیشرفت را ببینند
- مشخص است چه کسی چه کاری کرده

---

## اولویت
**🔴 بسیار بالا** - به‌خصوص قسمت Parallel Execution

**دلیل:**
- صرفه‌جویی قابل‌توجه در زمان (50-70%)
- تجربه کاربری بهتر
- کاهش کار دستی

---

**تاریخ ایجاد:** 1403/10/29
**هدف:** بهبود عملکرد parallel-task-executor agent
