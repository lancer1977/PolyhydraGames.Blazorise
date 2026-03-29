# MudBlazor Dialog Shim Checklist

## Discovery
- [x] Locate existing dialog abstraction and rendering flow.
- [x] Confirm current Blazorise dependencies in this package.
- [x] Identify a compatibility-preserving shim point.

## Documentation
- [x] Create feature documentation entry.
- [x] Add migration progress checklist.

## Implementation
- [x] Remove Blazorise `@using` from `_Imports.razor`.
- [x] Add MudBlazor package reference.
- [x] Keep `IDialogService` API stable.
- [x] Add `MudDialogShimService` and register it as default `IDialogService`.
- [x] Add custom Mud dialog components for prompt/int and selection flows.

## Validation
- [ ] Build package successfully.
- [ ] Verify runtime behavior for Alert, Confirm, Prompt, PromptInt, and Selection dialogs.

## Follow-up
- [ ] Add automated tests around cancel and close semantics.
- [ ] Confirm all consuming apps are no longer depending on deleted Blazorise dialog components.
