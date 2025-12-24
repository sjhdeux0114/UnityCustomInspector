# Universal Custom Inspector for Unity

A powerful, attribute-based custom inspector system for Unity that eliminates the need for writing custom editor scripts. Boost your productivity with enhanced visualization, validation, and workflow automation tools.

![Unity Inspector](https://img.shields.io/badge/Unity-2021.3%2B-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## 🌟 Key Features

* **No Custom Editors:** Just add attributes to your fields.
* **Enhanced Visualization:** Viewers for sprites, audio, and prefabs directly in the Inspector.
* **Workflow Automation:** Auto-link child objects, load assets from folders, and more.
* **Data Validation:** Required fields, read-only mode, and asset/scene object restrictions.
* **Smart Selection:** Dropdowns for scenes, tags, layers, and input axes (no more magic strings!).
* **Layout Control:** Tabs, boxes, horizontal groups, and titles.

## 🚀 Installation

1.  Download the `.cs` files (`InspectorAttributes.cs`, `InspectorCore.cs`, `InspectorEditors.cs`, `InspectorBase.cs`, `ExtendedTooltipAttribute.cs`, `ExtendedTooltipDrawer.cs`) and place them anywhere in your Unity project (e.g., inside a `Scripts/Editor` folder).
2.  (Optional) If you want to use the inheritance method, ensure `InspectorBase.cs` is accessible.

## 📖 Usage Guide

You can enable the Universal Custom Inspector in two ways:

### Method A: Inheritance (Recommended)
Inherit from `InspectorBase` instead of `MonoBehaviour`.

```csharp
using UnityEngine;

public class MyScript : InspectorBase
{
    [Title("My Settings")]
    public float speed;
}
```

### Method B: Attribute
Add the `[ExtendedInspector]` attribute to your class.

```csharp
using UnityEngine;

[ExtendedInspector]
public class MyScript : MonoBehaviour
{
    [Title("My Settings")]
    public float speed;
}
```

---

## 🎨 Feature Showcase

### 1. Layout & Design

Organize your inspector with tabs, groups, and titles.

* **`[TabGroup("Name")]`**: Group fields into tabs.
* **`[BoxGroup("Name")]`**: Group fields into a box with a label.
* **`[HorizontalGroup]`**: Align fields horizontally.
* **`[Title("Text", true)]`**: Add a bold header and separator line.
* **`[Suffix("Unit")]`**: Add a unit label after the field.

```csharp
[TabGroup("Stats")] [BoxGroup("Movement")] [Suffix("m/s")]
public float moveSpeed;
```

### 2. Productivity Tools (Workflow)

Automate tedious tasks.

* **`[FindChild("Name")]`**: Automatically finds and assigns a child object by name.
* **`[AssetList]`**: Adds a "Load" button to populate an array with assets from a selected folder.
* **`[ColorPreset("Name", r, g, b, ...)]`**: Provides clickable color buttons for quick assignment.
* **`[FolderPath]`**: Adds a folder selection button that stores the path as a string.
* **`[OnValueChanged("MethodName")]`**: Invokes a method immediately when the field value changes.

```csharp
[FindChild("TitleText")] public Text title;
[AssetList] public Sprite[] icons;
```

### 3. Smart Selection (No Magic Strings)

Prevent typos with dropdown menus.

* **`[SceneName]`**: Dropdown for scenes in Build Settings.
* **`[Tag]`**: Dropdown for project tags.
* **`[Layer]`**: Dropdown for layers (stored as int).
* **`[SortingLayer]`**: Dropdown for 2D sorting layers (stored as ID).
* **`[InputAxis]`**: Dropdown for Input Manager axes.
* **`[AnimatorParam("AnimatorVar")]`**: Dropdown for Animator parameters.

```csharp
[SceneName] public string nextScene;
[Tag] public string playerTag;
```

### 4. Validation & Constraints

Ensure data integrity.

* **`[Required("Message")]`**: Shows a red error box if the field is null.
* **`[AssetsOnly]`**: Only allows project assets (prefabs), rejects scene objects.
* **`[SceneObjectsOnly]`**: Only allows scene objects, rejects prefabs.
* **`[ReadOnly]`**: Makes the field non-editable.
* **`[ShowIf("BoolName")]`**: Only shows the field if the condition is true.
* **`[MinMaxSlider(min, max)]`**: Range slider for Vector2.

```csharp
[Required] public GameObject prefab;
[ShowIf("isAttacking")] public float damage;
```

### 5. Visualization

Better data representation.

* **`[Viewer]`**: Shows a preview box for Sprites, Textures, AudioClips, and GameObjects.
* **`[ProgressBar(max)]`**: Displays a progress bar for numeric values.
* **`[ExtendedTooltip("Title", "Message")]`**: Adds a collapsible help box with optional documentation link.

```csharp
[Viewer] public Sprite characterIcon;
[ProgressBar(100)] public float health;
```

### 6. Actions

* **`[InspectorButton("Label")]`**: Adds a button to execute a method.

```csharp
[InspectorButton("Reset Stats")]
public void Reset() { ... }
```

## 📝 Example Code

Check out the full example script included in the repository to see all features in action.

```csharp
public class UniversalInspectorExample : InspectorBase
{
    [TabGroup("Design")] [Title("Base Stats")]
    [ProgressBar(100)] public float health = 100;

    [TabGroup("Workflow")]
    [FindChild] public Button closeButton;
    [AssetList] public Sprite[] runFrames;

    [TabGroup("Debug")]
    [InspectorButton("Log Info")]
    public void LogInfo() => Debug.Log("Hello World");
}
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
