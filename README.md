# Nice Dependency Injection Package

## Description

This package provides minimal setup dependency injection for static MonoBehavior scripts. All you need is `[Injectable]` attribute on injected class and `[Inject]` on a target field.

## Usage

Add `[Injectable]` attribute to a class declaration.
Add `[Inject]` attribute to a field.
Initial injection happens after scene is loaded, after all `Awake` methods and before all `Start` methods.
If your object spawns after initial injection but also needs it, add `DependancyInjector` to it.
If `[Inject]` field is found befor the corresponding `[Injectable]` type is bound, an error will be produced. 

## Example

```csharp
using UnityEngine;

[Injectable]
public class SomeService : MonoBehaviour
{
    public void Initialize()
    {
        Debug.Log("PlayerService initialized!");
    }
}
```

```csharp
using UnityEngine;

public class SomeScript : MonoBehaviour
{
    [Inject] private SomeService _someService;

    private void Start()
    {
        _someService.SomeMethod();
    }
}
```

## Limitations

- Currently only meant for injection of global static MonoBehavior classes into fields Injection is initialized on scene load
- Should work with any types if DependancyInjectionManager is called manually but tested only with MonoBehavior scripts

## Roadmap

- Injection in the editor from the same prefab
