# InfoPanel Module Version 1.0
Provides a flexible and dynamic information panel that reserves the top lines of the console for displaying continuously updating information.

## Overview
The InfoPanel module allows you to display important information or dynamic content in a reserved area at the top of the console. This makes it easy to keep essential data visible while still using the rest of the console for other tasks.
You write your own Custom content that inherits from `IInfoPanelContent`. 

### Features:
- Asynchronous updates: Continuously display information without blocking user interactions.
- Modular content: Each project can inject its own custom content.
- Flexible start and stop: Start, update, and stop the panel without affecting the console.
- Manually update content at any time.

## Console Space Reservation
The InfoPanel reserves a fixed area at the **top of the console window** to display its content.
- **Default size:** 2 lines (can be adjusted if needed).
- This reserved area is maintained even when the rest of the console is updated.
- Regular console output will always start **below** this area.

## Registering the InfoPanel

UThe InfoPanel requires manual initialization due to its nature as a visual status display.

### Where to register:
You can register and start the InfoPanel in either:
- `Startup.cs` (recommended for global use).
- A specific Command class in the `OnInitialized()` method.

### Example registration in `Startup.cs`:
```csharp
public class Startup
{
    public void Initialize()
    {
        var content = new DefaultPanelContent();
        InfoPanelService.Instance.RegisterContent(content);
        InfoPanelService.Instance.Start();
    }
}

## Services
- InfoPanelService

## Concepts
The InfoPanel module utilizes the **InfoPanelService**, a Singleton that manages the content and updates of the panel. Content is provided by classes implementing the **IInfoPanelContent** interface.

### IInfoPanelContent Interface:
```csharp
public interface IInfoPanelContent
{
    string GetText();
    string? ShortText { get; }
}
```