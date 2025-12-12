# 🚀 Quick Start Guide

**A quick and easy guide to running the project**

---

## 📋 Table of Contents

1. [System Requirements](#system-requirements)
2. [C# Edition - Running](#c-edition---running)
3. [Java Edition - Running](#java-edition---running)
4. [Application Usage](#application-usage)
5. [Troubleshooting](#troubleshooting)

---

## 💻 System Requirements

### For C# WinForms Edition

**Minimum:**
- Windows 7 or newer
- .NET 8.0 Runtime or SDK

**Check Installation:**
```bash
dotnet --version
```

**Installation:**
- Go to: https://dotnet.microsoft.com/download
- Select **.NET 8.0**
- Download and install

### For Java JavaFX Edition

**Minimum:**
- Windows, macOS, or Linux
- Java 17+ JDK (not JRE)
- Maven 3.6+

**Check Installation:**
```bash
java -version
mvn -version
```

**Installation:**
- Java: https://adoptium.net/
- Maven: https://maven.apache.org/download.cgi

---

## 🟦 C# Edition - Running

### Method 1: Direct Run (Easiest)

**Steps:**

1. **Unzip the file**
   ```bash
   unzip ImageProcessor.zip
   cd ImageProcessor/csharp
   ```

2. **Run the application**
   ```bash
   dotnet run
   ```

3. **Wait a moment** (~1-2 seconds)

4. **The application window will appear**

### Method 2: Build and Run

```bash
cd ImageProcessor/csharp

# Build the project
dotnet build

# Run the application
dotnet run -c Release
```

### Method 3: Run Executable

```bash
cd ImageProcessor/csharp

# Build an executable file
dotnet publish -c Release -r win-x64

# Find the executable file
# It will be in: bin/Release/net8.0-windows/win-x64/publish/

# Run the executable
./bin/Release/net8.0-windows/win-x64/publish/ImageProcessorWinForms.exe
```

### Method 4: Using Visual Studio (Optional)

1. Open Visual Studio
2. Select "Open a project or solution"
3. Select the `ImageProcessorWinForms.csproj` file
4. Press F5 to run

---

## 🟩 Java Edition - Running

### Method 1: Direct Run (Easiest)

**Steps:**

1. **Unzip the file**
   ```bash
   unzip ImageProcessor.zip
   cd ImageProcessor/java
   ```

2. **Run the application**
   ```bash
   mvn javafx:run
   ```

3. **Wait a moment** (~3-5 seconds)

4. **The application window will appear**

### Method 2: Build and Run

```bash
cd ImageProcessor/java

# Build the project
mvn clean package

# Run the JAR
java -jar target/ImageProcessorJavaFX-2.0.jar
```

### Method 3: Using IDE

**IntelliJ IDEA:**
1. Open the project
2. Select File → Open
3. Select the `java` folder
4. Wait for Maven to load
5. Press Shift+F10 to run

**Eclipse:**
1. File → Import → Existing Maven Projects
2. Select the `java` folder
3. Press Alt+Shift+X, M to run

---

## 📖 Application Usage

### Step 1: Load an Image

1. Click the **"📁 Load Image from Device"** button
2. Select an image from your device (PNG, JPG, BMP)
3. The image will appear in both panels

### Step 2: Manual Processing (Left)

**Manual Mode (Step Mode):**

1. Click the **"⏭️ Step"** button
2. The next filter will be applied to the left image
3. Filters sequence: Original → Grayscale → Invert → Brightness → Contrast → Sepia → Blur
4. Click Step again for the next filter
5. When the last filter is reached, it will cycle back to the first

### Step 3: Automatic Processing (Right)

**Auto Mode (Start/Stop):**

1. Click the **"▶️ Start"** button
2. Filters will start changing automatically every 1.2 seconds
3. The right image will change automatically
4. Click **"⏹️ Stop"** to pause the animation

### Step 4: Saving Images

**Saving Options:**

1. **Save Left Image Only**
   - Click "💾 Save Left"
   - Choose the folder and format

2. **Save Right Image Only**
   - Click "💾 Save Right"
   - Choose the folder and format

3. **Save Both Images**
   - Click "💾 Save All"
   - Choose the folder
   - Both images will be saved together

---

## 🎨 Filter Explanation

| Filter | Description | Use Case |
|--------|--------|-----------|
| **Original** | The original image without modification | Comparison |
| **Grayscale** | Conversion to black and white | Vintage effects |
| **Invert** | Color inversion | Artistic effects |
| **Brightness** | Increase luminosity | Brightening images |
| **Contrast** | Increase contrast | Clarity enhancement |
| **Sepia** | Vintage warm tone | Antique photos |
| **Blur** | Softens the image | Artistic blurring |

---

## 🖥️ Application Interface

```
┌─────────────────────────────────────────────────┐
│  📁 Load Image from Device                      │  ← Click here to load an image
├──────────────────────┬──────────────────────────┤
│                      │                          │
│  Left Image          │  Right Image             │
│  (Step Mode)         │  (Auto Mode)             │
│                      │                          │
│  Filter: Grayscale   │  Filter: Sepia           │
├──────────────────────┴──────────────────────────┤
│ ████████████ Processing Left Image...           │  ← Progress Bar
├──────────────────────┬──────────────────────────┤
│ ⏭️ Step  💾 Save   │ ▶️ Start  💾 Save       │  ← Buttons
│     💾 Save All      │                          │
├──────────────────────┴──────────────────────────┤
│ ✓ Left: Applied Grayscale                      │  ← Status Message
└─────────────────────────────────────────────────┘
```

---

## ⌨️ Keyboard Shortcuts

| Key | Function |
|--------|--------|
| **Ctrl+O** | Load image (in some versions) |
| **Ctrl+S** | Save image (in some versions) |
| **Esc** | Close pop-up windows |
| **Enter** | Confirm selection |

---

## 🎯 Usage Examples

### Example 1: Comparing Filters

1. Load an image
2. Click Step several times to see different filters
3. Compare the original image with the modified copies

### Example 2: Quick Preview

1. Load an image
2. Click Start on the right
3. Watch the filters change automatically
4. Click Stop when you like an image

### Example 3: Saving Different Copies

1. Load an image
2. Click Step on the left until you reach a filter you like
3. Save the left image
4. Click Start on the right
5. Wait until another filter is applied
6. Save the right image
7. Now you have two different copies

---

## 🔧 Troubleshooting

### Problem: Application won't start (C#)

**Solution 1:**
```bash
# Check .NET installation
dotnet --version

# If no version appears, install .NET 8.0 from:
# https://dotnet.microsoft.com/download
```

**Solution 2:**
```bash
# Try cleaning and rebuilding
cd csharp
dotnet clean
dotnet build
dotnet run
```

### Problem: Application won't start (Java)

**Solution 1:**
```bash
# Check Java installation
java -version

# The result should be Java 17 or newer
# If not installed, download from:
# https://adoptium.net/
```

**Solution 2:**
```bash
# Check Maven
mvn -version

# If not installed:
# https://maven.apache.org/download.cgi
```

**Solution 3:**
```bash
# Try cleaning and rebuilding
cd java
mvn clean install
mvn javafx:run
```

### Problem: Image won't load

**Solution:**
- Ensure the image format is PNG, JPG, or BMP
- Ensure the image is not corrupted
- Try another image

### Problem: Filters are not working

**Solution:**
- Ensure an image is loaded first
- Wait a moment for the application to finish loading
- Try restarting the application

### Problem: Cannot save images

**Solution:**
- Ensure there is enough free disk space
- Ensure you have write permissions for the folder
- Try another folder

---

## 📊 Performance Information

### Startup Time

| Edition | Time |
|--------|-------|
| C# WinForms | ~1 second |
| Java JavaFX | ~3-5 seconds |

### Memory Usage

| Edition | Memory |
|--------|--------|
| C# WinForms | 50-100 MB |
| Java JavaFX | 100-150 MB |

### Processing Speed

| Image Size | Time |
|-----------|-------|
| Small (< 1MB) | < 100ms |
| Medium (1-5MB) | 100-500ms |
| Large (> 5MB) | 500ms-2s |

---

## 🎓 Tips for Optimal Use

1. **Start with small images** - Faster processing
2. **Use Step for precise inspection** - To see each filter clearly
3. **Use Auto for quick preview** - To see all filters quickly
4. **Save copies you like** - Before moving to another image
5. **Try different filters** - On the same image to see the differences

---

## 🆘 Support and Help

### If you encounter a problem:

1. **Check System Requirements** - Is .NET or Java installed?
2. **Read the Error Message** - It may contain useful information
3. **Try the solution from the Troubleshooting section**
4. **Restart the application** - May resolve the issue
5. **Reinstall .NET or Java** - If the problem persists

---

## 📚 Additional References

- [README.md](README.md) - General project information
- [BUILD_GUIDE.md](docs/BUILD_GUIDE.md) - Detailed build instructions
- [FEATURES.md](docs/FEATURES.md) - Feature explanation
- [COMPARISON.md](docs/COMPARISON.md) - Comparison between editions

---

## ✅ Checklist

Before starting:
- [ ] Is .NET 8.0 or Java 17+ installed?
- [ ] Is Maven installed (for Java only)?
- [ ] Have you unzipped the file?
- [ ] Are you in the correct folder?

When running:
- [ ] Did the application window appear?
- [ ] Can you load an image?
- [ ] Are the filters working?
- [ ] Can you save images?

---

**You are now ready to start! 🎉**

**Choose your preferred edition and start processing images! 🎨**

---

**Version**: 2.0  
**Last Updated**: 2025  
**Status**: ✅ Ready to Use
