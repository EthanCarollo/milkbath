# milkbath

Unity project built with Unity `6000.3.23f1`.

## Project structure

- `Assets/Scripts/Framework`: reusable framework runtime and editor tooling.
- `Assets/Scenes`: project scenes.
- `Assets/Settings`: render pipeline and project settings assets.

The framework is isolated in its own assemblies and does not depend on project-specific namespaces or tweening libraries. Its only non-Unity editor convenience is the declared SerializeReference Extensions package used to select polymorphic `Updatable` implementations in the Inspector.
