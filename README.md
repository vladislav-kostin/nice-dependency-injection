# Nice Dependency Injection Package

## Description

This package provides minimal setup dependency injection for static MonoBehavior scripts. All you need is `[Injectable]` attribute on injected class and `[Inject]` on a target field.

## Usage

Add `[Injectable]` attribute to a MonoBehavior-based class declaration for it to be injected into any field of that type in MonoBehavior scripts with `[Inject]` attributes. The attribute is inherited.  

The injection is initialized right after scene load, after all `Awake` methods and before all `Start` methods. The classes that are being injected are currently expected to be in the scene when the injection happens.  

If the object is spawned dynamically, it needs a `DependencyInjection` attribute.  

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