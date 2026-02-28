# Vlys Scene Notes 1.0.2

**Vlys Scene Notes** allows you to create, organize, and manage contextual notes directly inside the Unity Scene View, improving team communication and scene-level documentation.

---

## Getting Started

1. Open the **Scene View**.
2. Locate the **“SN”** overlay button.
3. Click **Scene Notes**.
4. Press **Create Note**.
5. Click anywhere in the Scene to place the note
   *(You can reposition it later.)*
6. Select the note in the Inspector.
7. Write your content and press **Save**.

---

## Editing Notes

* Select an existing note.
* Click **Enable Editing**.
* Modify:

  * Text
  * Category
  * Status
  * Position
* Click **Apply Changes** to confirm.

---

## Comments System

Each note supports threaded comments that include:

* Author name
* Automatic timestamp
* Change tracking context

This allows teams to maintain communication history directly tied to specific scene locations.

---

## Managing Notes

* Use the Scene View overlay filters to control visible categories.
* Delete individual notes from the Inspector.
* Remove all notes from the current scene via:

```
Vlys → Scene Notes → Delete All Notes
```

A confirmation dialog will appear before permanent deletion.

---

## Style Customization

You can fully customize how notes look both in the **Scene View** and in the **Inspector**.

Go to:

```
Vlys → Scene Notes → Style Settings
```

From there you can modify:

* Colors
* Typography
* Layout appearance
* Visual states

Changes apply immediately across the project.

---

## Default Icon Configuration

Each category supports custom icons.

To modify the default icons:

```
Vlys Notes/DefaultSettings/SceneNoteIconConfig.asset
```

From this asset, you can assign different icons per category to better match your production workflow.

---

## Features Overview

* Scene-based contextual notes
* Category filtering
* Status tracking
* Comment history with author and timestamp
* Visual style customization
* Custom category icons
* Safe edit workflow with unsaved changes protection

---

Designed to streamline communication and production clarity directly inside your Unity scenes.