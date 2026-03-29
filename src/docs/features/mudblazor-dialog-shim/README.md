---
title: MudBlazor Dialog Shim
status: in-progress
owner: @DreadBreadcrumb
priority: high
complexity: 2
created: 2026-03-28
updated: 2026-03-28
tags: [dialogs, mudblazor, migration, shim]
---

# MudBlazor Dialog Shim

## Summary
Migrates dialog handling from Blazorise-based components to MudBlazor while preserving the existing `IDialogService` contract used by callers.

## Current State
- Interface contract unchanged for dialog consumers.
- `IDialogService` now resolves to a MudBlazor-backed shim (`MudDialogShimService`).
- Blazorise dialog components are removed from this library package.
- MudBlazor dependency added.

## Integration
- Register dialogs with `services.AddPolyDialogs()`.
- Add Mud providers in your host layout (`MudThemeProvider`, `MudPopoverProvider`, `MudDialogProvider`, `MudSnackbarProvider`) and call `services.AddMudServices()`.

## Follow-up
- Add behavior tests for alert, confirm, prompt, prompt-int, and selection flows.
