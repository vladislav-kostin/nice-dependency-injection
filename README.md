# Nice Dependency Injection Package

## Description

This package provides minimal-setup global runtime dependency injection primarily for MonoBehavior scripts, and local edit-time injection of components within prefabs. All you need is `[Injectable]` attribute on injected class and `[Inject]` on a target field for global injection, or `[InjectFromPrefab]` and `[InjectFromGameObject]` for local injection in the editor.

## Usage

### Global injection
- Add `[Injectable]` attribute to a class declaration.
- Add `[Inject]` attribute to a field.
- Initial injection happens after scene is loaded, after all `Awake` methods and before all `Start` methods.
- If your object spawns after initial injection but also needs it, add `DependencyInjector` component to it.
- If `[Inject]` field is found befor the corresponding `[Injectable]` type is bound, an error will be produced.

### Local injection
- Use `[InjectFromPrefab]` attribute for any serialized Component fields to have Component automatically assigned from the same prefab
- Use `[InjectFromGameObject]` attribute automatically assigning from the same game object within the prefab
- First component found on the prefab or object of the required type will be assigned
- Works for prefabs in the assets folder, not instances
- Supports arrays and lists for easy gathering of all components of the required type
- To inject into properties, inject into their backing field `[field: InjectFromPrefab] Collider AllColliders {get, private set}`

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

```csharp
using UnityEngine;

public class SomeScript2 : MonoBehaviour
{
    [InjectFromPrefab] private SomeComponent _someComponent;
    [InjectFromGameObject] private BoxCollider _boxCollider;

    private void Start()
    {
        _someComponent.SomeMethod();
    }
}
```

## Limitations

- The objects that are being injected are meant to be present at the time of injection, will produce an error message otherwise
- Objects that were spawned after they were requested for injection will not be injected into those `[Inject]` fields
- For injection of other types, make calls to DependancyInjectionManager manually

## Known issues

- Local injection sometimes doesn't update correctly especially in the hirarchy. Considering a more simple, efficient and reliable `OnValidate()` approach


