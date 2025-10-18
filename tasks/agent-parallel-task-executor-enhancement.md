# Feature Request: Auto-Update Task Breakdown File in parallel-task-executor Agent

## Overview
The `parallel-task-executor` agent should automatically update the task breakdown markdown file as it completes each task, marking them with checkboxes and status indicators.

## Current Behavior

Currently, the agent:
1. Reads the task breakdown file (e.g., `tasks/2025-01-18-logging-standardization-simplified.md`)
2. Executes all tasks
3. Returns a summary report
4. **Does NOT update the original task file**

After the agent finishes, a human must manually:
- Read through the task file
- Change `- [ ]` to `- [x]` for completed tasks
- Add status indicators like `✅ COMPLETED`, `⏭️ SKIPPED`, `❌ FAILED`
- Add execution summaries

## Desired Behavior

The agent should:
1. Read the task breakdown file
2. **As each task is completed**, update the file in real-time:
   - Change `- [ ]` to `- [x]` for the completed sub-task
   - Mark task sections with status: `✅ COMPLETED`, `⏭️ SKIPPED`, `❌ FAILED`, `⚠️ PARTIAL`
   - Add timestamps if needed
3. At the end, append an execution summary section
4. Commit the updated task file (optional, based on configuration)

## Implementation Requirements

### 1. Task File Parsing
The agent needs to:
- Parse markdown checkbox syntax: `- [ ]` (unchecked) and `- [x]` (checked)
- Identify task sections (e.g., `### 1. Task Name`)
- Understand task hierarchy (main tasks and sub-tasks)

### 2. Real-Time Updates
After completing each task or sub-task:
```markdown
# Before
### 1. Extend ILogService Interface
- [ ] Add general logging methods
- [ ] Update LogService.cs implementation

# After (when first sub-task completes)
### 1. Extend ILogService Interface
- [x] Add general logging methods
- [ ] Update LogService.cs implementation

# After (when all sub-tasks complete)
### 1. Extend ILogService Interface ✅ COMPLETED
- [x] Add general logging methods
- [x] Update LogService.cs implementation
```

### 3. Status Indicators
Use these status markers at the end of task section headers:
- `✅ COMPLETED` - All sub-tasks finished successfully
- `⏭️ SKIPPED` - Task was skipped (e.g., file not found, not applicable)
- `❌ FAILED` - Task failed with errors
- `⚠️ PARTIAL` - Some sub-tasks completed, some failed
- `🔄 IN PROGRESS` - Currently working on this task

### 4. Execution Summary
At the end of the file, append a summary section:
```markdown
---

## 📊 Execution Summary

**Status:** ✅ **COMPLETED** (X of Y tasks completed, Z skipped, W failed)
**Date Completed:** 2025-01-18
**Execution Time:** 15 minutes 32 seconds
**Agent:** parallel-task-executor

### What Was Accomplished:
1. ✅ Task 1 description
2. ✅ Task 2 description
3. ⏭️ Task 3 description (skipped - reason)

### Statistics:
- **Total Tasks:** Y
- **Completed:** X
- **Skipped:** Z
- **Failed:** W
- **Files Modified:** N files
```

### 5. Configuration Options
Add optional parameters to the agent:
```typescript
{
  "update_task_file": true,           // Enable/disable auto-update
  "update_frequency": "per_subtask",  // "per_subtask" | "per_task" | "at_end"
  "add_summary": true,                // Add execution summary at end
  "commit_on_complete": false,        // Auto-commit updated file
  "timestamp_format": "ISO8601"       // Timestamp format for logs
}
```

## Example Workflow

### Input Task File (before execution):
```markdown
### 1. Extend ILogService Interface
**Parallelizable**: No
**Dependencies**: None

- [ ] Add general logging methods
- [ ] Update LogService.cs implementation
- [ ] Keep existing ServiceCallLog methods

### 2. Convert Infrastructure Layer
**Parallelizable**: No
**Dependencies**: Task 1

- [ ] Replace ILogger<T> with ILogService
- [ ] Update logging calls
```

### During Execution:
Agent updates file after completing Task 1:
```markdown
### 1. Extend ILogService Interface ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Completed At:** 2025-01-18 14:30:45

- [x] Add general logging methods
- [x] Update LogService.cs implementation
- [x] Keep existing ServiceCallLog methods

### 2. Convert Infrastructure Layer 🔄 IN PROGRESS
**Parallelizable**: No
**Dependencies**: Task 1

- [x] Replace ILogger<T> with ILogService
- [ ] Update logging calls
```

### Final Output:
```markdown
### 1. Extend ILogService Interface ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: None
**Completed At:** 2025-01-18 14:30:45

- [x] Add general logging methods
- [x] Update LogService.cs implementation
- [x] Keep existing ServiceCallLog methods

### 2. Convert Infrastructure Layer ✅ COMPLETED
**Parallelizable**: No
**Dependencies**: Task 1
**Completed At:** 2025-01-18 14:35:12

- [x] Replace ILogger<T> with ILogService
- [x] Update logging calls

---

## 📊 Execution Summary

**Status:** ✅ **COMPLETED** (2 of 2 tasks completed)
**Date Completed:** 2025-01-18
**Execution Time:** 4 minutes 27 seconds
**Agent:** parallel-task-executor
```

## Benefits

1. ✅ **Real-time Progress Tracking**: See which tasks are done without waiting for completion
2. ✅ **Automatic Documentation**: Task file stays up-to-date automatically
3. ✅ **Less Manual Work**: No need to manually update checkboxes and status
4. ✅ **Historical Record**: Timestamps show when each task completed
5. ✅ **Better Collaboration**: Team members can see progress in real-time
6. ✅ **Easier Debugging**: If agent fails, can see exactly where it stopped

## Technical Considerations

### File Access
- Agent needs **Write** tool access to update the task file
- Should handle concurrent access if multiple agents run simultaneously
- Should backup original file before starting (optional)

### Error Handling
- If file update fails, should continue execution but log warning
- Should not fail entire task execution if just the status update fails
- Validate markdown syntax after each update

### Performance
- Updating file after each sub-task might be slow for large task lists
- Consider batching updates (e.g., update every 5 sub-tasks)
- Option to only update at task-level (not sub-task level)

### Compatibility
- Should work with existing task breakdown format
- Should not break if task file has non-standard formatting
- Should preserve comments and custom sections

## Alternative Approach: Separate Progress File

Instead of modifying the original task file, create a separate progress file:
- `tasks/2025-01-18-logging-standardization-simplified.md` (original, unchanged)
- `tasks/2025-01-18-logging-standardization-simplified-progress.md` (auto-generated)

This keeps the original clean and allows multiple execution runs.

## Priority
**Medium-High** - This significantly improves developer experience and reduces manual work.

## Related Tools
- TodoWrite tool (for in-session task tracking)
- Task breakdown architect agent (creates the initial file)
- This enhancement makes parallel-task-executor more autonomous

---

**Created:** 2025-01-18
**Author:** Claude Code User
**Purpose:** Enhancement request for parallel-task-executor agent
