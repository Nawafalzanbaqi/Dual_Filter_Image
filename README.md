# Dual Filter Image Player

A simple C# Windows Forms desktop application with a modern dark UI that lets you apply image filters in two different ways:

- **Left image**: filters change automatically (Start / Stop).
- **Right image**: filters change manually (Next Filter button).

---

## 🖥️ Overview

- Single main window with:
  - **Top**: one large button – *Open Image from Device*.
  - **Middle**: two image panels (left & right) showing the same image.
  - **Bottom**: buttons to save images and a global status label.

All controls are created in code (no Designer file) to keep the layout fully controlled and consistent.

---

## ✨ Features

- Modern **dark theme** (similar to VS Code / Visual Studio).
- Load one image and display it in both left and right panels.
- **Left panel**:
  - `Start Filters` / `Stop` button.
  - Uses a `Timer` to automatically cycle through filters at a fixed interval.
- **Right panel**:
  - `Next Filter` button.
  - Applies the next filter once on each click.
- Built-in filters:
  - Grayscale  
  - Sepia  
  - Invert Colors  
  - Blur  
  - Sharpen  
  - Brightness (increase)  
  - Contrast (increase)  
  - Edge Detection (Sobel)
- Saving options:
  - `Save Left Image`
  - `Save Right Image`
  - `Save All` (saves both images to a folder)
- Status labels:
  - Left / right status labels for current filter.
  - Global status label at the bottom for overall messages.

---

## 🔧 Technologies

- **Language:** C#
- **Framework:** Windows Forms
- **Namespaces used:**
  - `System.Drawing`
  - `System.Windows.Forms`
  - `System.Threading.Tasks`

---

## 🚀 How to Run

1. Clone or download the project.
2. Open the solution in **Visual Studio** (or VS Code with .NET tools).
3. Build the project.
4. Run the application.

---

## 📌 Usage

1. Click **"Open Image from Device"** and choose an image file.
2. Use the **left side**:
   - Click **Start Filters** to begin automatic filter cycling.
   - Click **Stop** to stop.
3. Use the **right side**:
   - Click **Next Filter** to apply the next filter one step at a time.
4. To save:
   - Click **Save Left Image** or **Save Right Image** to save individually.
   - Click **Save All** to save both images to a selected folder.
